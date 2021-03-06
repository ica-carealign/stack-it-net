﻿using System;
using System.Collections.Generic;

using Hangfire;

using Ica.StackIt.Application.Hangfire;
using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;

namespace Ica.StackIt.Application.Command
{
	[Queue(Constants.OrderedQueueName)]
	public class ScheduledStartStack
	{
		private readonly IRepository<Stack> _stackRepository;
		private readonly IRepository<Instance> _instanceRepository;
		private readonly IInstancePower _startInstances;
		private readonly IScheduleCalculator _scheduleCalculator;
		private readonly IBackgroundJobClient _backgroundJobClient;

		public ScheduledStartStack(
			IRepository<Stack> stackRepository,
			IRepository<Instance> instanceRepository,
			IInstancePower startInstances,
			IScheduleCalculator scheduleCalculator,
			IBackgroundJobClient backgroundJobClient)
		{
			_stackRepository = stackRepository;
			_instanceRepository = instanceRepository;
			_startInstances = startInstances;
			_scheduleCalculator = scheduleCalculator;
			_backgroundJobClient = backgroundJobClient;
		}

		public void Execute(Guid stackId)
		{
			var stack = _stackRepository.Find(stackId);
			if (stack == null)
			{
				// nothing to do if the stack doesn't exist anymore
				return;
			}

			var primaryInstances = new List<string>();
			// ReSharper disable once LoopCanBeConvertedToQuery
			foreach (var id in stack.InstanceIds)
			{
				var instance = _instanceRepository.Find(id);
				if (instance != null)
				{
					primaryInstances.Add(instance.ResourceId);
				}
			}

			// Only toggle the stack instances if the stack schedule is enabled
			// We do not end the chain of power on / power off cycles because
			// the enabled flag could be flipped between one run and the next
			if (stack.ScheduleEnabled)
			{
				_startInstances.Execute(stack.OwnerProfileId, primaryInstances);
			}

			var nextStopTimespan = _scheduleCalculator.GetNextStop(stack);
			_backgroundJobClient.Schedule<ScheduledStopStack>(x => x.Execute(stackId), nextStopTimespan);
		}
	}
}
