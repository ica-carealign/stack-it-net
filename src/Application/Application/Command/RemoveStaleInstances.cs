using System;
using System.Collections.Generic;
using System.Linq;

using Hangfire;

using Ica.StackIt.Application.Hangfire;
using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;

using AwsInstance = Amazon.EC2.Model.Instance;

namespace Ica.StackIt.Application.Command
{
	[Queue(Constants.UnorderedQueueName)]
	public class RemoveStaleInstances : CommandBase
	{
		private readonly IRepository<Instance> _instanceRepository;
		private readonly IBackgroundJobClient _backgroundJobClient;

		public RemoveStaleInstances(
			IAwsClientFactory awsClientFactory,
			IRepository<Instance> instanceRepository,
			IRepository<AwsProfile> profileRepository,
			IBackgroundJobClient backgroundJobClient)
			: base(profileRepository, awsClientFactory)
		{
			_instanceRepository = instanceRepository;
			_backgroundJobClient = backgroundJobClient;
		}

		public void Execute(Guid profileId)
		{
			IAwsClient awsClient;
			if (!TryInitializeClient(profileId, out awsClient))
			{
				return;
			}

			IEnumerable<string> awsInstancesIds = awsClient.InstanceService.GetAllInstances().Select(x => x.InstanceId);
			List<Instance> dbInstances = _instanceRepository.FindAll()
				.Where(x => ! x.NeedsRefreshing)
				.Where(x => x.OwnerProfileId == profileId)
				.ToList();

			IEnumerable<string> instanceIdsToRemove = dbInstances.Select(x => x.ResourceId).Except(awsInstancesIds).Where(x => x != default(Guid).ToString());

			foreach (string staleResourceId in instanceIdsToRemove)
			{
				Instance instanceToRemove = dbInstances.FirstOrDefault(x => x.ResourceId == staleResourceId);
				if (instanceToRemove != null)
				{
					_backgroundJobClient.Enqueue<CleanUpPuppet>(x => x.CleanUp(instanceToRemove));
					_instanceRepository.Delete(instanceToRemove);
				}
			}
		}
	}
}