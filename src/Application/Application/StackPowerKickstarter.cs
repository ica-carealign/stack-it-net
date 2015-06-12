using System;

using Hangfire;

using Ica.StackIt.Application.Command;
using Ica.StackIt.Core.Entities;

namespace Ica.StackIt.Application
{
	public class StackPowerKickstarter : IStackPowerKickstarter
	{
		private readonly IScheduleCalculator _scheduleCalculator;
		private readonly IBackgroundJobClient _backgroundJobClient;

		public StackPowerKickstarter(
			IScheduleCalculator scheduleCalculator,
			IBackgroundJobClient backgroundJobClient)
		{
			_scheduleCalculator = scheduleCalculator;
			_backgroundJobClient = backgroundJobClient;
		}

		public void KickstartSchedule(Stack stack)
		{
			var timeUntilNextStart = _scheduleCalculator.GetNextStart(stack);
			var timeUntilNextStop = _scheduleCalculator.GetNextStop(stack);

			var nextScheduleTypeResult = timeUntilNextStart.CompareTo(timeUntilNextStop);

			if (nextScheduleTypeResult < 0)
			{
				// start time is sooner
				_backgroundJobClient.Schedule<ScheduledStartStack>(x => x.Execute(stack.Id), timeUntilNextStart);
			}
			else if (nextScheduleTypeResult > 0)
			{
				// stop time is sooner
				_backgroundJobClient.Schedule<ScheduledStopStack>(x => x.Execute(stack.Id), timeUntilNextStop);
			}
			else
			{
				throw new InvalidOperationException("Start time and end time are the same.");
			}
		}
	}
}