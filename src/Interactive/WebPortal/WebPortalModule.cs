using System.Web;

using Autofac;
using Autofac.Integration.Mvc;

using Ica.StackIt.Application;
using Ica.StackIt.Infrastructure;
using Ica.StackIt.Interactive.WebPortal.Controllers;
using Ica.StackIt.Interactive.WebPortal.ViewModelHelpers;

using Microsoft.Owin;

namespace Ica.StackIt.Interactive.WebPortal
{
	public class WebPortalModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder
				.RegisterInfrastructure()
				.RegisterApplication()
				.RegisterAuthentication();

			builder.Register(context => HttpContext.Current.GetOwinContext()).As<IOwinContext>();
			builder.RegisterControllers(typeof(HomeController).Assembly);
			builder.RegisterFilterProvider();
			builder.RegisterType<UserProfileAccessManager>().As<IUserProfileAccessManager>();
			builder.RegisterType<SessionProfileAttribute>().PropertiesAutowired();
			builder.RegisterType<StackViewModelHelper>().As<IStackViewModelHelper>();
		}
	}

	public static class ContainerBuilderExtensions
	{
		public static ContainerBuilder RegisterWebPortal(this ContainerBuilder self)
		{
			self.RegisterModule<WebPortalModule>();
			return self;
		}
	}
}