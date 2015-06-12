using System.Collections.Generic;
using System.Linq;

using Autofac;

using Ica.StackIt.Application;
using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;

using Stack = Ica.StackIt.Core.Entities.Stack;

namespace Ica.StackIt.Interactive.WebPortal
{
	public partial class Startup
	{
		const string _globalScheduleName = "Business Hours";

		public void ConfigureScheduler()
		{
			var defaultSchedule = ConfigureSchedulerSeedData();
			StartStackScheduleJobs(defaultSchedule);
		}

		private Schedule ConfigureSchedulerSeedData()
		{
			var scheduleRepository = Container.Resolve<IRepository<Schedule>>();
			var globalSchedule = scheduleRepository.CreateQuery().FirstOrDefault(x => x.Name == _globalScheduleName);
			if (globalSchedule == null)
			{
				globalSchedule = new Schedule
				{
					GlobalDefault = true,
					Name = _globalScheduleName,
					StartOnWeekends = false,
					StartCron = "* 8 * * 1-5", // 8 AM, Monday through Friday
					StopCron = "* 18 * * 1-5", // 6 PM, Monday through Friday
				};
				scheduleRepository.Add(globalSchedule);
			}

			foreach (var schedule in GetAdditionalSchedules())
			{
				var scopedSchedule = schedule;
				var existingSchedule = scheduleRepository.CreateQuery().FirstOrDefault(x => x.Name == scopedSchedule.Name);
				if (existingSchedule == null)
				{
					scheduleRepository.Add(scopedSchedule);
				}
			}

			return globalSchedule;
		}

		private static IEnumerable<Schedule> GetAdditionalSchedules()
		{
			yield return GetSchedule("Extended Business Hours", "* 6 * * 1-5", "* 20 * * 1-5");
			yield return GetSchedule("Business Hours with Weekends", "* 8 * * *", "* 18 * * *");
			yield return GetSchedule("Extended Business Hours with Weekends", "* 6 * * *", "* 20 * * *");
		}

		private static Schedule GetSchedule(string name, string startCron, string stopCron)
		{
			return new Schedule
			{
				GlobalDefault = false,
				Name = name,
				StartCron = startCron,
				StopCron = stopCron,
				StartOnWeekends = false
			};
		}

		private void StartStackScheduleJobs(Schedule schedule)
		{
			var stackRepository = Container.Resolve<IRepository<Stack>>();
			var scheduleRepository = Container.Resolve<IRepository<Schedule>>();
			var kickstarter = Container.Resolve<IStackPowerKickstarter>();

			foreach (var stack in stackRepository.FindAll())
			{
				var stackSchedule = scheduleRepository.Find(stack.ScheduleId);
				if (stackSchedule == null)
				{
					stack.ScheduleId = schedule.Id;
				}

				stack.ScheduleEnabled = stack.CreatedByApplication;
				stackRepository.Update(stack);

				kickstarter.KickstartSchedule(stack);
			}
		}
	}
}