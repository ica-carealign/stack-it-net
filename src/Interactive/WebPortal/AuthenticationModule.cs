using System;
using System.Configuration;

using Autofac;

using AutoMapper;

using Ica.StackIt.Application.Configuration;
using Ica.StackIt.Application.Enums;
using Ica.StackIt.AspNet.Identity.Crowd;
using Ica.StackIt.Interactive.WebPortal.Authentication;

using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace Ica.StackIt.Interactive.WebPortal
{
	public class AuthenticationModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<CrowdConfiguration>().As<ICrowdConfiguration>();
			builder.Register(context =>
			{
				string authenticationProviderConfiguration = ConfigurationManager.AppSettings["AuthenticationProvider"] ?? "Crowd";
				AuthenticationProvider provider;
				bool parsed = Enum.TryParse(authenticationProviderConfiguration, out provider);
				if (!parsed)
				{
					throw new ApplicationException(string.Format("An invalid authentication provider was specified: {0}", authenticationProviderConfiguration));
				}
				return provider;
			});

			builder.RegisterType<SecurityServer>().As<ISecurityServer>();

			RegisterAuthenticatedUserClient(builder);

			builder.RegisterType<UserStore>().As<IUserStore<IdentityUser>>();
			builder.RegisterType<RoleStore>().As<IRoleStore<IdentityRole>>();

			builder.RegisterType<CrowdUserManager>();
			builder.Register(context =>
			{
				var userManager = context.Resolve<CrowdUserManager>();
				IAuthenticationManager authenticationManager = context.Resolve<IOwinContext>().Authentication;
				return new CrowdSignInManager(userManager, authenticationManager);
			});
			builder.RegisterType<SignInManager>().As<ISignInManager>();

			builder.Register(context => Mapper.Engine).As<IMappingEngine>();
		}

		private static void RegisterAuthenticatedUserClient(ContainerBuilder builder)
		{
			builder.Register(context =>
			{
				var configuration = context.Resolve<IStackItConfiguration>();
				AuthenticationProvider provider = configuration.AuthenticationProvider;

				IAuthenticatedUserClient client = null;
				switch (provider)
				{
					case AuthenticationProvider.Development:
						client = new DevelopmentAuthenticatedUserClient();
						break;
					case AuthenticationProvider.Crowd:
						ICrowdConfiguration crowdConfiguration = configuration.CrowdConfiguration;
						var securityServer = context.Resolve<ISecurityServer>();
						if (crowdConfiguration.Url != null)
						{
							securityServer.Url = crowdConfiguration.Url;
						}
						client = new CrowdAuthenticatedUserClient(
							securityServer,
							crowdConfiguration.ApplicationName,
							crowdConfiguration.ApiPassword);
						break;
				}

				return client;
			});
		}
	}

	public static class AuthContainerBuilderExtensions
	{
		public static ContainerBuilder RegisterAuthentication(this ContainerBuilder self)
		{
			self.RegisterModule<AuthenticationModule>();
			return self;
		}
	}
}