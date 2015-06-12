using System;
using System.Linq;

using Hangfire;

using Ica.StackIt.Application.Hangfire;
using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;

namespace Ica.StackIt.Application.Command
{
	[Queue(Constants.UnorderedQueueName)]
	public class RemoveStaleStacks : CommandBase
	{
		private readonly IRepository<Stack> _stackRepository;

		public RemoveStaleStacks(IAwsClientFactory awsClientFactory, IRepository<Stack> stackRepository, IRepository<AwsProfile> profileRepository)
			: base(profileRepository, awsClientFactory)
		{
			_stackRepository = stackRepository;
		}

		public void Execute(Guid profileId)
		{
			IAwsClient awsClient;
			if (!TryInitializeClient(profileId, out awsClient))
			{
				return;
			}

			var awsStackIds = awsClient.StackService.GetAllStacks().Select(x => x.StackId);
			var dbStacks = _stackRepository.FindAll()
				.Where(x => ! x.NeedsRefreshing)
				.Where(x => x.OwnerProfileId == profileId)
				.ToList();

			var stackIdsToRemove = dbStacks.Select(x => x.ResourceId).Except(awsStackIds);

			foreach(var staleResourceId in stackIdsToRemove)
			{
				var staleInstance = dbStacks.FirstOrDefault(x => x.ResourceId == staleResourceId);
				if(staleInstance != null)
				{
					_stackRepository.Delete(staleInstance);
				}
			}
		}
	}
}