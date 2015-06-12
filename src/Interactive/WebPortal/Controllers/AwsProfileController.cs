using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using Amazon.Runtime;

using AwsContrib.EnvelopeCrypto;

using Hangfire;

using Ica.StackIt.Application;
using Ica.StackIt.Application.Command;
using Ica.StackIt.AspNet.Identity.Crowd;
using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;
using Ica.StackIt.Interactive.WebPortal.Models;

namespace Ica.StackIt.Interactive.WebPortal.Controllers
{
	[Authorize]
	[SessionProfile]
	public class AwsProfileController : Controller
	{
		private readonly IRepository<AwsProfile> _profileRepository;
		private readonly IRepository<IPRange> _ipRangeRepository;
		private readonly ICryptoProvider _cryptoProvider;
		private readonly IBackgroundJobClient _backgroundJobClient;
		private readonly IAuthenticatedUserClient _userClient;
		private readonly IAwsClientFactory _awsClientFactory;

		private const string _encryptedMessagePlaceholder = "(Encrypted)";

		public AwsProfileController(
			IRepository<AwsProfile> profileRepository,
			IRepository<IPRange> ipRangeRepository,
			ICryptoProvider cryptoProvider,
			IBackgroundJobClient backgroundJobClient,
			IAuthenticatedUserClient userClient,
			IAwsClientFactory awsClientFactory)
		{
			_profileRepository = profileRepository;
			_ipRangeRepository = ipRangeRepository;
			_cryptoProvider = cryptoProvider;
			_backgroundJobClient = backgroundJobClient;
			_userClient = userClient;
			_awsClientFactory = awsClientFactory;
		}

		// GET: Profile
		public ActionResult Index()
		{
			IEnumerable<AwsProfile> profiles = _profileRepository.FindAll();
			IEnumerable<AwsProfileViewModel> modelCollection = profiles.Select(x =>
				new AwsProfileViewModel
				{
					Id = x.Id,
					Name = x.Name,
					AccessKeyId = x.AccessKeyId,
					SecretAccessKey = _encryptedMessagePlaceholder
				});
			return View(modelCollection);
		}

		// GET: Profile/Details/5
		public ActionResult Details(Guid id)
		{
			AwsProfile profile = _profileRepository.Find(id);

			var viewModel = new AwsProfileViewModel
			{
				Name = profile.Name,
				AccessKeyId = profile.AccessKeyId,
				SecretAccessKey = _encryptedMessagePlaceholder,
				DefaultVpcId = profile.DefaultVpcId,
				DefaultSubnetId = profile.DefaultSubnetId,
				Id = profile.Id,
				Groups = profile.Groups.ToList(),
				HostedZone = profile.HostedZone,
				DetailedBillingS3Bucket = profile.DetailedBillingS3Bucket,
			};

			return View(viewModel);
		}

		// GET: Profile/Create
		public ActionResult Create()
		{
			IEnumerable<string> activeDirectoryGroups = _userClient.GetAllRoles();

			var model = new AwsProfileViewModel {ServerGroups = activeDirectoryGroups.ToList()};
			return View(model);
		}

		// POST: Profile/Create
		[HttpPost]
		public ActionResult Create(AwsProfileViewModel profileViewModel)
		{
			try
			{
				if (ValidateViewModelAgainstExternalSources(profileViewModel, newProfile: true))
				{
					string dataKey;
					string encryptedStrings = _cryptoProvider.Encrypt(
						out dataKey,
						profileViewModel.SecretAccessKey
						);

					var profile = new AwsProfile
					{
						Name = profileViewModel.Name,
						Account = profileViewModel.Account,
						AccessKeyId = profileViewModel.AccessKeyId,
						EncryptedKey = dataKey,
						EncryptedSecretAccessKey = encryptedStrings,
						DefaultVpcId = profileViewModel.DefaultVpcId,
						DefaultSubnetId = profileViewModel.DefaultSubnetId,
						HostedZone = profileViewModel.HostedZone,
						Groups = profileViewModel.Groups,
						DetailedBillingS3Bucket = profileViewModel.DetailedBillingS3Bucket,
					};

					_profileRepository.Add(profile);

					_backgroundJobClient.Enqueue<CreateDefaultSecurityGroup>(x => x.Execute(profile.Id, profileViewModel.DefaultVpcId));
					_backgroundJobClient.Enqueue<CreateIpRange>(x => x.Execute(profile.Id, profileViewModel.DefaultSubnetId));

					return RedirectToAction("Index");
				}
			}
			catch (Exception e)
			{
				// TODO: Just for debugging! Remove this. Don't show this to the user, but log it somewhere for troubleshooting purposes.
				ModelState.AddModelError("", string.Format("There was a problem creating the profile. {0}", e.Message));
			}

			// If we make it this far, something bad happened. Display the error message to the user.
			return View(profileViewModel);
		}

		// GET: Profile/Edit/5
		public ActionResult Edit(Guid id)
		{
			AwsProfile profile = _profileRepository.Find(id);
			IEnumerable<string> activeDirectoryGroups = _userClient.GetAllRoles();

			var viewModel = new AwsProfileViewModel
			{
				Name = profile.Name,
				Account = profile.Account,
				AccessKeyId = profile.AccessKeyId,
				SecretAccessKey = _encryptedMessagePlaceholder,
				DefaultVpcId = profile.DefaultVpcId,
				DefaultSubnetId = profile.DefaultSubnetId,
				Groups = profile.Groups.ToList(),
				ServerGroups = activeDirectoryGroups.ToList(),
				HostedZone = profile.HostedZone,
				DetailedBillingS3Bucket = profile.DetailedBillingS3Bucket,
			};

			return View(viewModel);
		}

		// POST: Profile/Edit/5
		[HttpPost]
		public ActionResult Edit(Guid id, AwsProfileViewModel profileViewModel)
		{
			try
			{
				AwsProfile profile = _profileRepository.Find(id);

				if (profileViewModel.SecretAccessKey == _encryptedMessagePlaceholder)
				{
					profileViewModel.SecretAccessKey = _cryptoProvider.Decrypt(profile.EncryptedKey, profile.EncryptedSecretAccessKey);
				}

				if (ValidateViewModelAgainstExternalSources(profileViewModel, newProfile: false))
				{
					profile.Name = profileViewModel.Name;
					profile.Account = profileViewModel.Account;
					profile.AccessKeyId = profileViewModel.AccessKeyId;
					profile.DefaultVpcId = profileViewModel.DefaultVpcId;
					profile.DefaultSubnetId = profileViewModel.DefaultSubnetId;
					profile.HostedZone = profileViewModel.HostedZone;
					profile.Groups = profileViewModel.Groups.ToList();
					profile.DetailedBillingS3Bucket = profileViewModel.DetailedBillingS3Bucket;

					if (profileViewModel.SecretAccessKey != _encryptedMessagePlaceholder)
					{
						string dataKey;
						string encryptedAccessKey = _cryptoProvider.Encrypt(out dataKey, profileViewModel.SecretAccessKey);
						profile.EncryptedSecretAccessKey = encryptedAccessKey;
						profile.EncryptedKey = dataKey;
					}

					_profileRepository.Update(profile);

					_backgroundJobClient.Enqueue<CreateIpRange>(x => x.Execute(profileViewModel.Id, profileViewModel.DefaultSubnetId));

					return RedirectToAction("Index");
				}
			}
			catch
			{
				// err, todo?
				return View(profileViewModel);
			}

			return View(profileViewModel);
		}

		// GET: Profile/Delete/5
		public ActionResult Delete(Guid id)
		{
			AwsProfile profile = _profileRepository.Find(id);

			var viewModel = new AwsProfileViewModel
			{
				Name = profile.Name,
				AccessKeyId = profile.AccessKeyId,
				Id = profile.Id
			};

			return View(viewModel);
		}

		// POST: Profile/Delete/5
		[HttpPost]
		public ActionResult Delete(AwsProfileViewModel profileViewModel)
		{
			try
			{
				_profileRepository.Delete(profileViewModel.Id);

				var ipRange = _ipRangeRepository.FindAll().Where(x => x.AwsProfileId == profileViewModel.Id);
				foreach (var range in ipRange)
				{
					_ipRangeRepository.Delete(range);
				}

				return RedirectToAction("Index");
			}
			catch
			{
				return View();
			}
		}

		/// <summary>
		///     Take a view model (with an account, access key, and secret access key) and do validations.
		///     Specifically, ensure a profile is associated with one account in the db, and
		///     ensure that an the profile actually belongs to the AWS account that the user said it belongs to.
		///     This method will set the model error state for the request appropriately.
		/// </summary>
		/// <param name="profileViewModel"></param>
		/// <param name="newProfile"></param>
		/// <returns></returns>
		private bool ValidateViewModelAgainstExternalSources(AwsProfileViewModel profileViewModel, bool newProfile)
		{
			var valid = true;
			string profileName;
			var isAlreadyAssociated = TryGetAssociatedProfile(profileViewModel.Account, out profileName);
			if ((isAlreadyAssociated && newProfile) || isAlreadyAssociated && profileName != profileViewModel.Name)
			{
				ModelState.AddModelError("", string.Format("The account {0} is already associated with the profile {1}.", profileViewModel.Account, profileName));
				valid = false;
			}
			else if (!ValidateAccountProfileCredentials(profileViewModel.Account, profileViewModel.AccessKeyId, profileViewModel.SecretAccessKey))
			{
				ModelState.AddModelError("", string.Format("The access key {0} does not belong to the account {1}.", profileViewModel.AccessKeyId, profileViewModel.Account));
				valid = false;
			}

			if (!valid)
			{
				profileViewModel.ServerGroups = _userClient.GetAllRoles().ToList();
			}

			return valid;
		}

		private bool TryGetAssociatedProfile(string account, out string profileName)
		{
			var profile = _profileRepository.FindAll().FirstOrDefault(x => x.Account == account);
			if (profile == null)
			{
				profileName = null;
				return false;
			}

			profileName = profile.Name;
			return true;
		}

		private bool ValidateAccountProfileCredentials(string account, string accessKey, string secretKey)
		{
			var client = _awsClientFactory.GetClient(new BasicAWSCredentials(accessKey, secretKey));
			return client.IdentityService.ValidateProfileOwnership(account);
		}
	}
}