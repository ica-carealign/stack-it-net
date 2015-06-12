using System;

using Hangfire;

using Ica.StackIt.Application;
using Ica.StackIt.Application.Command;
using Ica.StackIt.Interactive.WebPortal.Events;

using Owin;

namespace Ica.StackIt.Interactive.WebPortal
{
	public partial class Startup
	{
		private static ThrottledScheduler _refreshEverythingScheduler;

		public void ConfigureEventBus(IAppBuilder app)
		{
			var atLeastOnceEvery = TimeSpan.FromMinutes(5);
			var atMostOncePer = TimeSpan.FromMinutes(1);
			_refreshEverythingScheduler = new ThrottledScheduler(atLeastOnceEvery, atMostOncePer, ScheduleRefreshJob);

			// Every time a heartbeat is received from a connected browser,
			// tell the scheduler that we want a refresh as early as possible.
			EventBus.HeartbeatReceived += (sender, args) => _refreshEverythingScheduler.Trigger();

			_refreshEverythingScheduler.Start();
		}

		// Must remain reentrant!
		private static void ScheduleRefreshJob()
		{
			BackgroundJob.Enqueue<RefreshEverything>(cmd => cmd.Execute());
		}
	}
}