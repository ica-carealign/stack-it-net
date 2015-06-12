using Hangfire;

using Owin;

namespace Ica.StackIt.Interactive.WebPortal
{
	public partial class Startup
	{
		public void ConfigureHangfire(IAppBuilder app)
		{
			app.UseHangfireDashboard("/hangfire");
		}
	}
}