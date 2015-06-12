using System;

using Hangfire;

using Ica.StackIt.Application.Hangfire;
using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;

namespace Ica.StackIt.Application.Command
{
	[Queue(Constants.OrderedQueueName)]
	public class DeleteStack : CommandBase
	{
		public DeleteStack(
			IRepository<AwsProfile> profileRepository,
			IAwsClientFactory awsClientFactory) : base(profileRepository, awsClientFactory) {}

		public void Execute(Guid profileId, string stackName)
		{
			IAwsClient awsClient;
			if (!TryInitializeClient(profileId, out awsClient))
			{
				return;
			}

			awsClient.StackService.DeleteStack(stackName);
		}
	}
}
