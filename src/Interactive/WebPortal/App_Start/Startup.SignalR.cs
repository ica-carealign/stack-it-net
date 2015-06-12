using Owin;

namespace Ica.StackIt.Interactive.WebPortal
{
	public partial class Startup
	{
		public void ConfigureSignalR(IAppBuilder app)
		{
			// Registration of hubs is in Startup.Autofac.cs
			app.MapSignalR();
		}
	}
}