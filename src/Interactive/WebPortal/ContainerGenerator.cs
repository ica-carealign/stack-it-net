using System.IO;
using System.Web;

using Amazon;
using Amazon.EC2;
using Amazon.KeyManagementService;
using Amazon.Runtime;
using Amazon.S3;

using Autofac;

using AwsContrib.EnvelopeCrypto;

using Ica.StackIt.Application.AWS;
using Ica.StackIt.Application.Configuration;
using Ica.StackIt.Application.Parser;
using Ica.StackIt.Infrastructure;
using Ica.StackIt.Interactive.WebPortal.Bootstrap;
using Ica.StackIt.Interactive.WebPortal.Events;

namespace Ica.StackIt.Interactive.WebPortal
{
	public class ContainerGenerator
	{
		public static BootstrapData BootstrapData { get; private set; }

		public IContainer GetContainer()
		{
			if (BootstrapData == null)
			{
				BootstrapData = BootstrapApplication();
			}

			var builder = new ContainerBuilder();
			builder.RegisterWebPortal();

			// You'll need to have an AWS profile configured in one of two ways:
			//   1) Ambient environment http://docs.aws.amazon.com/AWSSdkDocsNET/latest/DeveloperGuide/net-dg-config-creds.html
			//   2) EC2 IAM role
			//
			// The profile will need:
			//   * EC2 access to describe the tags of the instance StackIt is running on
			//   * S3 access to the bucket where the bootstrap config lives
			//   * KMS access to the master key that's used for encrypting other AWS profiles
			builder.Register(context => AmbientCredentials.GetCredentials()).As<AWSCredentials>();

			builder.Register(context =>
			{
				var kmsClient = new AmazonKeyManagementServiceClient(context.Resolve<AWSCredentials>());
				var masterKeyAlias = context.Resolve<IStackItConfiguration>().CloudOptions.MasterKeyAlias;
				return new EnvelopeCryptoProvider(kmsClient, string.Format("alias/{0}", masterKeyAlias));
			}).As<ICryptoProvider>();

			// At this point, the application is configured to use values supplied by app.config
			// However, if we're running on EC2 and bootstrap data was provided,
			// we need to overwrite some infrastructure-level registrations
			if (BootstrapData != null)
			{
				builder.RegisterType<BootstrapDatabaseConfiguration>().As<IDatabaseConfiguration>()
					   .WithParameter("connectionString", BootstrapData.ApplicationDatabaseConnectionString);

				builder.RegisterType<BootstrapCrowdConfiguration>().As<ICrowdConfiguration>()
					   .WithParameter("url", BootstrapData.CrowdUrl)
					   .WithParameter("applicationName", BootstrapData.CrowdUsername)
					   .WithParameter("apiPassword", BootstrapData.CrowdPassword);

				builder.RegisterType<BootstrapHangfireConfiguration>().As<IHangfireConfiguration>()
					   .WithParameter("connectionString", BootstrapData.HangfireDatabaseConnectionString);
			}

			builder.Register(_ => EventBus.Instance).SingleInstance();
			builder.RegisterHubWithLifetimeScope<EventBusHub>();

			return builder.Build();
		}

		private static BootstrapData BootstrapApplication()
		{
			// If we're not on EC2, then we'll not even try bootstrapping the application.
			if (!IsEc2)
			{
				return null;
			}

			// Uses ambient AWS credentials, probably from an IAM role.
			// This should be exactly the same as just creating the clients without passing the credentials.
			// It is left explicit to guarantee the same algorithm is used to get credentials here as it is
			// in Startup.Autofac.cs.
			AWSCredentials credentials = AmbientCredentials.GetCredentials();
			IAmazonEC2 ec2Client = AWSClientFactory.CreateAmazonEC2Client(credentials);
			IAmazonS3 s3Client = AWSClientFactory.CreateAmazonS3Client(credentials);

			var instanceService = new InstanceService(ec2Client);
			var storageService = new StorageService(s3Client, new S3PathParser());
			var metadataService = new MetadataService();

			var bootstrapper = new ApplicationBootstrapper(instanceService, storageService, metadataService);
			return bootstrapper.BootstrapApplication();
		}

		private static bool IsEc2
		{
			get
			{
				// By *not* copying this file on application deployment, we know
				// that we're not running on an EC2 instance.
				string flagFile = MapPath(".") + @"\DevFlag.txt";
				return !File.Exists(flagFile);
			}
		}

		private static string MapPath(string path)
		{
			if (HttpContext.Current != null)
				return HttpContext.Current.Server.MapPath(path);

			return HttpRuntime.AppDomainAppPath + path.Replace("~", string.Empty).Replace('/', '\\');
		}
	}
}