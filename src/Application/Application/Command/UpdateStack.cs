using System;

using Hangfire;

using Ica.StackIt.Application.Hangfire;
using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;

using Stack = Amazon.CloudFormation.Model.Stack;

namespace Ica.StackIt.Application.Command
{
	[Queue(Constants.UnorderedQueueName)]
	public class UpdateStack : CommandBase
	{
		private readonly StackLoader _stackLoader;

		public UpdateStack(
			IAwsClientFactory awsClientFactory,
			IRepository<AwsProfile> profileRepository,
			StackLoader stackLoader) : base(profileRepository, awsClientFactory)
		{
			_stackLoader = stackLoader;
		}

		[DisableConcurrentExecution(60)]
		public void Execute(Guid profileId, string stackName)
		{
			IAwsClient awsClient;
			AwsProfile profile;
			if (!TryInitialize(profileId, out awsClient, out profile))
			{
				return;
			}

			// Search by name because AWS SDK can't search for a stack by resource ID
			Stack awsStack = awsClient.StackService.GetStack(stackName);
			if (awsStack == null)
			{
				return;
			}
			_stackLoader.LoadStack(awsClient, awsStack, profile.HostedZone, profileId);
		}
	}
}