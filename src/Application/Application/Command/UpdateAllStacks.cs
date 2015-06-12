using System;
using System.Collections.Generic;

using Hangfire;

using Ica.StackIt.Application.Hangfire;
using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;

using AwsStack = Amazon.CloudFormation.Model.Stack;
using AwsStackEvent = Amazon.CloudFormation.Model.StackEvent;

namespace Ica.StackIt.Application.Command
{
	[Queue(Constants.UnorderedQueueName)]
	public class UpdateAllStacks : CommandBase
	{
		private readonly StackLoader _stackLoader;

		public UpdateAllStacks(
			IAwsClientFactory awsClientFactory,
			IRepository<AwsProfile> profileRepository,
			StackLoader stackLoader) : base(profileRepository, awsClientFactory)
		{
			_stackLoader = stackLoader;
		}

		[DisableConcurrentExecution(60)]
		public void Execute(Guid profileId)
		{
			IAwsClient awsClient;
			AwsProfile profile;
			if (!TryInitialize(profileId, out awsClient, out profile))
			{
				return;
			}

			IEnumerable<AwsStack> stacks = awsClient.StackService.GetAllStacks();
			foreach (AwsStack awsStack in stacks)
			{
				_stackLoader.LoadStack(awsClient, awsStack, profile.HostedZone, profileId);
			}
		}
	}
}