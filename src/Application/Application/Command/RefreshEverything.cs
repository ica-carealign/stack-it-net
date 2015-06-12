using System;

using Hangfire;

using Ica.StackIt.Application.Hangfire;
using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;

namespace Ica.StackIt.Application.Command
{
	[Queue(Constants.UnorderedQueueName)]
	public class RefreshEverything
	{
		private readonly IRepository<AwsProfile> _profileRepository;
		private readonly IBackgroundJobClient _backgroundJobClient;

		public RefreshEverything(
			IRepository<AwsProfile> profileRepository,
			IBackgroundJobClient backgroundJobClient)
		{
			_profileRepository = profileRepository;
			_backgroundJobClient = backgroundJobClient;
		}

		[DisableConcurrentExecution(60)]
		public virtual void Execute()
		{
			foreach (AwsProfile profile in _profileRepository.FindAll())
			{
				Guid profileId = profile.Id;
				_backgroundJobClient.Enqueue<RefreshIpRanges>(x => x.Execute(profileId));
				_backgroundJobClient.Enqueue<UpdateAllImages>(x => x.Execute(profileId));
				_backgroundJobClient.Enqueue<UpdateAllInstances>(x => x.Execute(profileId));
				_backgroundJobClient.Enqueue<UpdateAllStacks>(x => x.Execute(profileId));
				_backgroundJobClient.Enqueue<RemoveStaleInstances>(x => x.Execute(profileId));
				_backgroundJobClient.Enqueue<RemoveStaleStacks>(x => x.Execute(profileId));

				if (! string.IsNullOrEmpty(profile.DetailedBillingS3Bucket))
				{
					if (profile.IsBillingHistoryLoaded)
					{
						_backgroundJobClient.Enqueue<UpdateBillingData>(x => x.LoadDeltas(profileId));
					}
					else
					{
						_backgroundJobClient.Enqueue<UpdateBillingData>(x => x.LoadAllHistory(profileId));
					}
				}
			}
		}
	}
}