using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using AutoMapper;

using Ica.StackIt.Application.Command;
using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;
using Ica.StackIt.Interactive.WebPortal.Models;
using Hangfire;

using Ica.StackIt.Interactive.WebPortal.Extensions;
using Ica.StackIt.Interactive.WebPortal.ViewModelHelpers;

namespace Ica.StackIt.Interactive.WebPortal.Controllers
{
	[Authorize]
	[SessionProfile]
	public class InstanceController : Controller
	{
		private readonly IRepository<Stack> _stackRepository;
		private readonly IRepository<Instance> _instanceRepository;
		private readonly IRepository<BaseImage> _baseImageRepository;
		private readonly IMappingEngine _mapper;
		private readonly IBackgroundJobClient _backgroundJobClient;
		private readonly IStackViewModelHelper _stackViewModelHelper;

		public InstanceController(
			IRepository<Stack> stackRepository,
			IRepository<Instance> instanceRepository,
			IRepository<BaseImage> baseImageRepository,
			IMappingEngine mappingEngine,
			IBackgroundJobClient backgroundJobClient,
			IStackViewModelHelper stackViewModelHelper)
		{
			_stackRepository = stackRepository;
			_instanceRepository = instanceRepository;
			_baseImageRepository = baseImageRepository;
			_mapper = mappingEngine;
			_backgroundJobClient = backgroundJobClient;
			_stackViewModelHelper = stackViewModelHelper;
		}

		[HttpGet]
		public ActionResult Index(Guid id)
		{
			Stack stack = _stackRepository.Find(id);
			IEnumerable<Instance> instances = stack.InstanceIds.Select(x => _instanceRepository.Find(x)).Where(x => x != null).ToList();
			var instanceViewModels = new List<InstanceOverviewViewModel>();

			var model = new StackInstancesViewModel
			{
				StackName = stack.Name,
				StackRecordId = id,
				StackPowerViewModel = _stackViewModelHelper.CreateStackPowerViewModel(stack)
			};

			foreach (var instance in instances)
			{
				var instanceOverviewViewModel = _mapper.Map<Instance, InstanceOverviewViewModel>(instance);
				if (instance.Role != null)
				{
					var baseImageId = instance.Role.BaseImageId;
					var baseImage = _baseImageRepository.Find(baseImageId);

					instanceOverviewViewModel.Image = baseImage.Name;
					instanceOverviewViewModel.Platform = baseImage.Platform;

					var httpPort = instance.Role.Ports.FirstOrDefault(x => x.PortNumber == 80 || x.PortNumber == 443);
					if (httpPort != null)
					{
						instanceOverviewViewModel.HttpAccessability = httpPort.PortNumber == 80
							? HttpAccessibility.Http
							: HttpAccessibility.Https;
					}
				}

				instanceViewModels.Add(instanceOverviewViewModel);
			}

			model.Instances = instanceViewModels;

			return View(model);
		}

		[HttpPost]
		public ActionResult StartInstance(string instanceId, Guid stackId)
		{
			return DoInstanceAction<StartInstances>(instanceId, stackId);
		}

		[HttpPost]
		public ActionResult StopInstance(string instanceId, Guid stackId)
		{
			return DoInstanceAction<StopInstances>(instanceId, stackId);
		}

		private ActionResult DoInstanceAction<T>(string instanceId, Guid stackId) where T : IInstancePower
		{
			var profileId = this.GetActiveProfileId();
			_backgroundJobClient.Enqueue<T>(x => x.Execute(profileId, new List<string> { instanceId }));
			return RedirectToAction("Index", "Instance", new { id = stackId });
		}
	}
}