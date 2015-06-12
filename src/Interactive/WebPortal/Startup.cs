using Amazon;

using Ica.StackIt.Application;
using Ica.StackIt.Application.AWS;
using Ica.StackIt.Application.Configuration;
using Ica.StackIt.Application.Parser;
using Ica.StackIt.Interactive.WebPortal;

using Microsoft.Owin;

using Owin;

[assembly: OwinStartup(typeof (Startup))]

namespace Ica.StackIt.Interactive.WebPortal
{
	public partial class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			ConfigureAutofac(app);

			ConfigureAuth(app);

			ConfigureAutomapper();

			ConfigureHangfire(app);

			ConfigureSignalR(app);

			ConfigureTemplateExpiry();

			ConfigureEventBus(app);

			ConfigureScheduler();
		}

		public void ConfigureTemplateExpiry()
		{
			// Set up the lifecycle rules for the bucket where we store Cloud Formation templates
			var cloudOptions = new CloudOptions();
			var credentials = AmbientCredentials.GetCredentials();
			var s3Client = AWSClientFactory.CreateAmazonS3Client(credentials);
			var storageService = new StorageService(s3Client, new S3PathParser());

			storageService.CreateExpirationRule(
				cloudOptions.ConfigurationTemplateBucket,
				Conventions.ConfigurationTemplateBucketPrefix,
				7,
				"Cloud Formation Template Cleanup"
				);
		}
	}
}