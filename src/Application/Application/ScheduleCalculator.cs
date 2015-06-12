using System;
using System.Diagnostics;

using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;

using NCrontab;

namespace Ica.StackIt.Application
{
	public class ScheduleCalculator : IScheduleCalculator
	{
		private readonly IRepository<Schedule> _scheduleRepository;
		private readonly IClock _clock;

		public ScheduleCalculator(IRepository<Schedule> scheduleRepository, IClock clock)
		{
			_scheduleRepository = scheduleRepository;
			_clock = clock;
		}

		public TimeSpan GetNextStart(Stack stack)
		{
			var schedule = _scheduleRepository.Find(stack.ScheduleId);
			return GetTimespanFor(schedule.StartCron);
		}

		public TimeSpan GetNextStop(Stack stack)
		{
			var schedule = _scheduleRepository.Find(stack.ScheduleId);
			return GetTimespanFor(schedule.StopCron);
		}

		private TimeSpan GetTimespanFor(string cron)
		{
			var now = _clock.Now.ToCentralStandardTime();
			var nextOccurence = CrontabSchedule.Parse(cron).GetNextOccurrence(now);
			return nextOccurence - now;
		}
	}
}
