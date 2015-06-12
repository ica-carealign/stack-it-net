using System;
using System.Collections.Generic;
using System.Net.Http;

using Autofac;

using Hangfire;

using Ica.StackIt.Application.AWS;
using Ica.StackIt.Application.Billing;
using Ica.StackIt.Application.Command;
using Ica.StackIt.Application.Configuration;
using Ica.StackIt.Application.Parser;
using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;

namespace Ica.StackIt.Application
{
	public class ApplicationModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<AwsClientFactory>().As<IAwsClientFactory>();

			builder.RegisterType<StackService>().As<IStackService>();
			builder.RegisterType<ImageService>().As<IImageService>();
			builder.RegisterType<InstanceService>().As<IInstanceService>();
			builder.RegisterType<DnsService>().As<IDnsService>();
			builder.RegisterType<SecurityGroupService>().As<ISecurityGroupService>();
			builder.RegisterType<CredentialService>().As<ICredentialService>();
			builder.RegisterType<IdentityService>().As<IIdentityService>();
			builder.RegisterType<BackgroundJobClient>().As<IBackgroundJobClient>();

			// Command registrations
			builder.RegisterType<RefreshEverything>();
			builder.RegisterType<UpdateInstance>();
			builder.RegisterType<UpdateAllInstances>();
			builder.RegisterType<UpdateAllImages>();
			builder.RegisterType<UpdateStack>();
			builder.RegisterType<RemoveStaleInstances>();
			builder.RegisterType<StackLoader>();
			builder.RegisterType<UpdateStack>();
			builder.RegisterType<UpdateAllStacks>();
			builder.RegisterType<RemoveStaleStacks>();
			builder.RegisterType<CreateStack>();
			builder.RegisterType<DeleteStack>();
			builder.RegisterType<CreateIpRange>();
			builder.RegisterType<RefreshIpRanges>();
			builder.RegisterType<StopInstances>();
			builder.RegisterType<StartInstances>();
			builder.RegisterType<UpdateBillingData>();
			builder.RegisterType<CreateDefaultSecurityGroup>();
			builder.RegisterType<CleanUpPuppet>();

			builder.Register(context => new ScheduledStartStack(
				context.Resolve<IRepository<Stack>>(),
				context.Resolve<IRepository<Instance>>(),
				context.Resolve<StartInstances>(),
				context.Resolve<IScheduleCalculator>(),
				context.Resolve<IBackgroundJobClient>()
				)).As<ScheduledStartStack>();

			builder.Register(context => new ScheduledStopStack(
				context.Resolve<IRepository<Stack>>(),
				context.Resolve<IRepository<Instance>>(),
				context.Resolve<StopInstances>(),
				context.Resolve<IScheduleCalculator>(),
				context.Resolve<IBackgroundJobClient>()
				));

			builder.RegisterType<S3PathParser>().As<IS3PathParser>();
			builder.RegisterType<ArnParser>().As<IArnParser>();
			builder.RegisterType<BillingManager>().As<IBillingManager>();
			builder.RegisterType<CostCalculator>();
			builder.RegisterType<ScheduleCalculator>().As<IScheduleCalculator>();
			builder.RegisterType<StackPowerKickstarter>().As<IStackPowerKickstarter>();

			// TODO: We'll replace this hard-coded registration with a config file or a database lookup or something
			// so that we do not have to recompile to whitelist new types
			builder.Register(context =>
			{
				IList<IInstanceTypeConfiguration> instanceTypeConfig = new List<IInstanceTypeConfiguration>
				{
					new InstanceTypeConfiguration {Name = "t2.micro", Description = "1 vCPU, 1 GiB RAM"},
					new InstanceTypeConfiguration {Name = "t2.small", Description = "1 vCPU, 2 GiB RAM"},
					new InstanceTypeConfiguration {Name = "t2.medium", Description = "2 vCPU, 4 GiB RAM"}
				};

				return instanceTypeConfig;
			}).As<IList<IInstanceTypeConfiguration>>();
			builder.RegisterType<HangfireConfiguration>().As<IHangfireConfiguration>();
			builder.RegisterType<QueueingConfiguration>().As<IQueueingConfiguration>();
			builder.RegisterType<CloudOptions>().As<ICloudOptions>();
			builder.RegisterType<PuppetConfiguration>().As<IPuppetConfiguration>();
			builder.RegisterType<StackItConfiguration>().As<IStackItConfiguration>();

			builder.RegisterType<NumberedStringGenerator>().As<INumberedStringGenerator>();

			builder.Register(context =>
			{
				var puppetConfiguration = context.Resolve<IPuppetConfiguration>();
				var url = string.Format("http://{0}:{1}", puppetConfiguration.PuppetCleanupHost, puppetConfiguration.NodeServicePort);
				return new HttpClient
				{
					BaseAddress = new Uri(url)
				};
			}).As<HttpClient>();
		}
	}

	public static class ContainerBuilderExtensions
	{
		public static ContainerBuilder RegisterApplication(this ContainerBuilder self)
		{
			self.RegisterModule<ApplicationModule>();
			return self;
		}
	}
}