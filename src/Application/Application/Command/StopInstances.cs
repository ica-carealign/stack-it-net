﻿using Hangfire;
using Ica.StackIt.Application.Hangfire;
using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;
using System;
using System.Collections.Generic;

namespace Ica.StackIt.Application.Command
{
	[Queue(Constants.OrderedQueueName)]
	public class StopInstances : CommandBase, IInstancePower
	{
		public StopInstances(
			IRepository<AwsProfile> profileRepository,
			IAwsClientFactory awsClientFactory) : base(profileRepository, awsClientFactory) {}

		public void Execute(Guid profileId, List<string> instanceIds)
		{
			IAwsClient awsClient;
			if (!TryInitializeClient(profileId, out awsClient))
			{
				return;
			}

			awsClient.InstanceService.StopInstances(instanceIds);
		}
	}
}
