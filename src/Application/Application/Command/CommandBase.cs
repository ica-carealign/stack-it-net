using System;

using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;

namespace Ica.StackIt.Application.Command
{
	public abstract class CommandBase
	{
		protected readonly IRepository<AwsProfile> ProfileRepository;
		private readonly IAwsClientFactory _clientFactory;

		protected CommandBase(IRepository<AwsProfile> profileRepository, IAwsClientFactory clientFactory)
		{
			ProfileRepository = profileRepository;
			_clientFactory = clientFactory;
		}

		protected bool TryInitializeClient(Guid profileId, out IAwsClient awsClient)
		{
			var profile = ProfileRepository.Find(profileId);
			if (profile == null)
			{
				awsClient = null;
				return false;
			}

			awsClient = _clientFactory.GetClient(profile);
			return true;
		}

		protected bool TryInitialize(Guid profileId, out IAwsClient awsClient, out AwsProfile profile)
		{
			var localProfile = ProfileRepository.Find(profileId);
			if (localProfile == null)
			{
				profile = null;
				awsClient = null;
				return false;
			}

			awsClient = _clientFactory.GetClient(localProfile);
			profile = localProfile;
			return true;
		}
	}
}