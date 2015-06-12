using System.Web.Mvc;

using Autofac;
using Autofac.Integration.Mvc;

using Microsoft.AspNet.SignalR;

using Owin;

namespace Ica.StackIt.Interactive.WebPortal
{
	public partial class Startup
	{
		private IContainer Container { get; set; }

		public void ConfigureAutofac(IAppBuilder app)
		{
			Container = new ContainerGenerator().GetContainer();

			// Register the Autofac middleware FIRST.
			app.UseAutofacMiddleware(Container);
			app.UseAutofacMvc();
			DependencyResolver.SetResolver(new AutofacDependencyResolver(Container));

			// ReSharper disable once RedundantNameQualifier
			GlobalHost.DependencyResolver = new Autofac.Integration.SignalR.AutofacDependencyResolver(Container);
		}
	}
}