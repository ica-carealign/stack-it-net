using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using AutoMapper;

using Hangfire;

using Ica.StackIt.Application;
using Ica.StackIt.Application.Billing;
using Ica.StackIt.Application.Command;
using Ica.StackIt.Application.Configuration;
using Ica.StackIt.Application.Model;
using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;
using Ica.StackIt.Interactive.WebPortal.Models;
using Ica.StackIt.Interactive.WebPortal.Extensions;
using Ica.StackIt.Interactive.WebPortal.ViewModelHelpers;

using Microsoft.Owin;

using Tag = Ica.StackIt.Core.Entities.Tag;
using Version = Ica.StackIt.Core.Entities.Version;
using System.Web;
using System.IO;
using System.Web.Routing;

using Instance = Ica.StackIt.Core.Entities.Instance;
using Stack = Ica.StackIt.Core.Entities.Stack;

namespace Ica.StackIt.Interactive.WebPortal.Controllers
{
	[Authorize]
	[SessionProfile]
	public class HomeController : Controller
	{
		private readonly IStackViewModelHelper _stackViewModelHelper;
		private readonly INumberedStringGenerator _numberedStringGenerator;
		private readonly IUserProfileAccessManager _userProfileAccessManager;
		private readonly IStackItConfiguration _stackItConfiguration;
		private readonly CostCalculator _costCalculator;
		private readonly IBackgroundJobClient _backgroundJobClient;
		private readonly IRepository<Instance> _instanceRepository;
		private readonly IRepository<Schedule> _scheduleRepository;
		private readonly IMappingEngine _mapper;
		private readonly IOwinContext _owinContext;
		private readonly IRepository<Product> _productRepository;
		private readonly IRepository<AwsProfile> _profileRepository;
		private readonly IRepository<Stack> _stackRepository;

		public HomeController(
			IRepository<Stack> stackRepository,
			IRepository<AwsProfile> profileRepository,
			IRepository<Product> productRepository,
			IRepository<Instance> instanceRepository,
			IRepository<Schedule> scheduleRepository,
			IBackgroundJobClient backgroundJobClient,
			IMappingEngine mappingEngine,
			IOwinContext owinContext,
			INumberedStringGenerator numberedStringGenerator,
			IUserProfileAccessManager userProfileAccessManager,
			IStackItConfiguration stackItConfiguration,
			CostCalculator costCalculator,
			IStackViewModelHelper stackViewModelHelper)
		{
			_stackRepository = stackRepository;
			_profileRepository = profileRepository;
			_productRepository = productRepository;
			_instanceRepository = instanceRepository;
			_scheduleRepository = scheduleRepository;
			_backgroundJobClient = backgroundJobClient;
			_mapper = mappingEngine;
			_owinContext = owinContext;
			_numberedStringGenerator = numberedStringGenerator;
			_userProfileAccessManager = userProfileAccessManager;
			_stackItConfiguration = stackItConfiguration;
			_costCalculator = costCalculator;
			_stackViewModelHelper = stackViewModelHelper;
		}

		public ActionResult Index()
		{
			Guid profileId = this.GetActiveProfileId();
			IEnumerable<Stack> stacks = _stackRepository.FindAll().Where(x => x.OwnerProfileId == profileId);

			var models = new List<StackOverviewViewModel>();
			foreach (var stack in stacks)
			{
				var model = _mapper.Map<Stack, StackOverviewViewModel>(stack);
				model.StackPowerViewModel = _stackViewModelHelper.CreateStackPowerViewModel(stack);
				model.StackInstanceViewModel = _stackViewModelHelper.CreateStackInstancesViewModel(stack);
				model.TotalCost = _costCalculator.CalculateCost(stack);
				model.ScheduleEnabled = stack.ScheduleEnabled;
				models.Add(model);
			}
			return View(models);
		}

		[HttpGet]
		public ActionResult Create()
		{
			Guid profileId = this.GetActiveProfileId();

			var profile = _profileRepository.Find(profileId);

			List<Product> products = _productRepository.FindAll().ToList();
			var schedules = _scheduleRepository.FindAll();

			var model = new CreateStackViewModel
			{
				SelectedProfileId = profileId,
				Products = products,
				SelectedVpcId = profile.DefaultVpcId,
				SelectedSubnetId = profile.DefaultSubnetId,
				InstanceTypes = _stackItConfiguration.InstanceTypes.Select(x => x.Name).ToList(),
				Schedules = schedules
					.OrderByDescending(x => x.GlobalDefault)
					.ThenBy(x => x.Name)
					.Select(x => new ScheduleViewModel {Id = x.Id, Name = x.Name})
					.ToList(),
				ScheduleEnabled = true
			};

			return View(model);
		}

		[HttpPost]
		public ActionResult CreateStack(CreateStackViewModel viewModel)
		{
			// If the user doesn't have permission to the selected profile, fail early
			string currentUserName = _owinContext.Authentication.User.Identity.Name;
			var userProfiles = _userProfileAccessManager.GetProfilesForUser(currentUserName).ToList();
			var selectedProfile = userProfiles.FirstOrDefault(x => x.Id == viewModel.SelectedProfileId);
			bool userHasPermissionForProfile = selectedProfile != null;

			if (!userHasPermissionForProfile)
			{
				throw new InvalidOperationException("User does not have permission to use this profile.");
			}

			var stackComponentDefinition = viewModel.SelectedProductIds
			                                        .Zip(viewModel.SelectedVersionNames, (productId, versionName) => new {productId, versionName})
			                                        .Zip(viewModel.SelectedRoleNames, (prodVer, roleName) => new {prodVer.productId, prodVer.versionName, roleName})
			                                        .Zip(viewModel.Options, (prodVerRole, options) => new {prodVerRole.productId, prodVerRole.versionName, prodVerRole.roleName, options})
			                                        .Select(x => new {x.productId, x.versionName, x.roleName, x.options}).ToList();

			var configuration = new StackConfiguration
			{
				StackName = viewModel.StackName,
				OwnerProfileId = viewModel.SelectedProfileId,
				OwnerUserName = currentUserName,
				VpcId = viewModel.SelectedVpcId,
				SubnetId = viewModel.SelectedSubnetId,
				HostedZone = userProfiles.Single(x => x.Id == viewModel.SelectedProfileId).HostedZone,
				BootstrapperUrl = _stackItConfiguration.PuppetConfiguration.BootstrapperUrl,
				PuppetInstallerUrl = _stackItConfiguration.PuppetConfiguration.PuppetInstallerUrl,
				PuppetHost = _stackItConfiguration.PuppetConfiguration.PuppetHost,
				DefaultSecurityGroupId = selectedProfile.DefaultSecurityGroupId,
				Notes = viewModel.Notes,
				ScheduleId = viewModel.SelectedScheduleId,
				ScheduleEnabled = viewModel.ScheduleEnabled
			};

			foreach (var entry in stackComponentDefinition)
			{
				var scopedEntry = entry;
				Product product = _productRepository.Find(scopedEntry.productId);
				Version version = product.Versions.Single(x => x.Name == scopedEntry.versionName);
				Role role = version.Roles.Single(x => x.Name == scopedEntry.roleName);
				
				OverwriteRoleOptions(role, scopedEntry.options);

				// If the instance type is not whitelisted, fail.
				// Do so after the role mapping so that website defaults (i.e. when a user doesn't alter the options)
				// values can be handled first.
				if (!_stackItConfiguration.InstanceTypes.Select(x => x.Name).Contains(role.Options.InstanceType))
				{
					throw new InvalidOperationException("Instance type not supported.");
				}

				bool useDefaultName = scopedEntry.options == null || string.IsNullOrEmpty(scopedEntry.options.InstanceName);
				string instanceName = useDefaultName
					? string.Format("{0}{1}{2}", viewModel.StackName, product.Name, role.Name)
					        .RemoveAllWhitespace()
					        .RemoveNonAlphaNumericCharacters()
					: scopedEntry.options.InstanceName;

				instanceName = _numberedStringGenerator.GetNextString(instanceName);

				var instance = new Instance
				{
					Name = instanceName,
					InstanceType = role.Options.InstanceType,
					Role = role,
					Tags = new List<Tag> {new Tag {Name = "Name", Value = instanceName}},
					VpcId = configuration.VpcId,
					SubnetId = configuration.SubnetId,
					ProductName = product.Name,
					VersionName = version.Name,
					NeedsRefreshing = true,
					OwnerProfileId = configuration.OwnerProfileId,
					IamRole = entry.options.IamRole ?? version.IamRole
				};

				configuration.Instances.Add(instance);
			}

			_backgroundJobClient.Enqueue<CreateStack>(x => x.Execute(viewModel.SelectedProfileId, configuration));

			return RedirectToAction("Index");
		}

		public ActionResult Delete(Guid id)
		{
			Guid profileId = this.GetActiveProfileId();

			Stack stack = _stackRepository.Find(id);
			if (stack == null)
			{
				return RedirectToAction("Index");
			}

			IEnumerable<Instance> instances = stack.InstanceIds.Select(x => _instanceRepository.Find(x)).Where(x => x != null);

			var model = new DeleteStackViewModel
			{
				SelectedProfileId = profileId,
				StackModel = new StackOverviewViewModel
				{
					Name = stack.Name,
					Description = stack.Description,
					CreateTime = stack.CreateTime
				},
				InstanceModels = _mapper.Map<IEnumerable<Instance>, IEnumerable<InstanceOverviewViewModel>>(instances),
			};

			return View(model);
		}

		[HttpPost]
		public ActionResult Delete(DeleteStackViewModel viewModel)
		{
			var stack = _stackRepository.FindAll().SingleOrDefault(x => x.Name == viewModel.StackModel.Name);
			if (stack == null)
			{
				// The user was looking at a stale list and the stack isn't even in the database anymore.
				// Redirect to the stack list and, from the user's perspective, the stack will have been deleted as requested.
				return RedirectToAction("Index");
			}

			var currentUserName = _owinContext.Authentication.User.Identity.Name;
			var userHasPermissionForProfile = _userProfileAccessManager.UserHasPermissionForProfile(currentUserName, viewModel.SelectedProfileId);

			if (!userHasPermissionForProfile)
			{
				throw new InvalidOperationException("User does not have permission to use this profile.");
			}

			var profileId = viewModel.SelectedProfileId;
			var stackName = viewModel.StackModel.Name;

			_backgroundJobClient.Enqueue<DeleteStack>(x => x.Execute(viewModel.SelectedProfileId, stackName));

			// Wait 30 seconds before trying to update this stack so that there is time to for the CreateStack command to fire
			// and for AWS to do its thing. This is so we don't have to wait up to 5 minutes for the scheduled job to update the database
			// with the new stack info.
			_backgroundJobClient.Schedule<UpdateStack>(x => x.Execute(profileId, stackName), TimeSpan.FromSeconds(60));

			return RedirectToAction("Index");
		}

		[HttpPost]
		public ActionResult StartStack(Guid stackId)
		{
			StackPower<StartInstances>(stackId);
			var controller = GetReferrerControllerName();

			if(controller == "Instance")
			{
				return RedirectToAction("Index", "Instance", new { id = stackId });
			}

			return RedirectToAction("Index", "Home");
		}

		[HttpPost]
		public ActionResult StopStack(Guid stackId)
		{
			StackPower<StopInstances>(stackId);
			var controller = GetReferrerControllerName();

			if(controller == "Instance")
			{
				return RedirectToAction("Index", "Instance", new { id = stackId });
			}

			return RedirectToAction("Index", "Home");
		}

		[HttpPost]
		public JsonResult UpdateNotes(StackNotesViewModel stackNotesViewModel)
		{
			try
			{
				string currentUsername = _owinContext.Authentication.User.Identity.Name;
				Stack stack = _stackRepository.Find(stackNotesViewModel.StackId);

				bool userHasPermissionForProfile = _userProfileAccessManager.UserHasPermissionForProfile(currentUsername, stackNotesViewModel.OwnerProfileId);
				if (!userHasPermissionForProfile)
				{
					stackNotesViewModel.Error = "User does not have permission to use this profile.";
				}

				if ( ! string.IsNullOrEmpty(stack.OwnerUserName))
				{
					// If the stack was created with StackIt.net (and thus was created with a specific user),
					// only allow that user to update the notes for the stack
					if (stack.OwnerUserName != currentUsername)
					{
						stackNotesViewModel.Error = "Only the user that created this stack may change its notes.";
					}
				}

				if (string.IsNullOrEmpty(stackNotesViewModel.Error))
				{
					stack.Notes = stackNotesViewModel.Notes;
					_stackRepository.Update(stack);
				}
			}
			catch (Exception)
			{
				// TODO: Log the exception somewhere
				stackNotesViewModel.Error = "Something unexpected happened. Contact an administrator.";
			}

			return new JsonResult {Data = stackNotesViewModel};
		}

		public ActionResult StackEvents(Guid id)
		{
			var stack = _stackRepository.Find(id);
			var model = new StackEventViewModel
			{
				StackName = stack.Name,
				StackEvents = stack.StackEvents.ToList()
			};

			return View(model);
		}

		/// <summary>
		/// Project the options sent from the web page onto the role
		/// </summary>
		/// <param name="role"></param>
		/// <param name="options"></param>
		/// <remarks>The mapping is only done if the website sent non-default values</remarks>
		private static void OverwriteRoleOptions(Role role, OptionsViewModel options)
		{
			if (options == null)
			{
				return;
			}

			const string dropDownDefault = "----";

			role.Options.InstanceType = options.InstanceType == dropDownDefault ? role.Options.InstanceType : options.InstanceType;
			role.Options.VolumeType = options.VolumeType == dropDownDefault ? role.Options.VolumeType : options.VolumeType;
			role.Options.VolumeSize = options.VolumeSize == 0 ? role.Options.VolumeSize : options.VolumeSize;
		}

		private void StackPower<T>(Guid stackId) where T : IInstancePower
		{
			var profileId = this.GetActiveProfileId();
			var stack = _stackRepository.Find(stackId);

			IEnumerable<string> instanceIds = stack.InstanceIds.Select(instanceId => _instanceRepository.Find(instanceId))
															   .Where(x => x != null)
															   .Select(inst => inst.ResourceId);

			_backgroundJobClient.Enqueue<T>(x => x.Execute(profileId, instanceIds.ToList()));
		}

		private string GetReferrerControllerName()
		{
			var url = Request.UrlReferrer.ToString();
			var request = new HttpRequest(null, url, null);
			var response = new HttpResponse(new StringWriter());
			var httpContext = new HttpContext(request, response);

			var routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));

			var values = routeData.Values;
			string controllerName = values["controller"].ToString();

			return controllerName;
		}
	}
}