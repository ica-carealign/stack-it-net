using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Ica.StackIt.Interactive.WebPortal
{
	public class MvcApplication : HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);

			HangfireBootstrapper.Instance.Start();
		}

		protected void Application_End(object sender, EventArgs e)
		{
			HangfireBootstrapper.Instance.Stop();
		}
	}
}