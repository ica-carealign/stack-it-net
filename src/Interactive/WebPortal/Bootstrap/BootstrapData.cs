namespace Ica.StackIt.Interactive.WebPortal.Bootstrap
{
	public class BootstrapData
	{
		// Crowd
		public string CrowdUrl { get; set; }
		public string CrowdUsername { get; set; }
		public string CrowdPassword { get; set; }

		// Mongo (application database)
		public string ApplicationDatabaseConnectionString { get; set; }

		// Hangfire (RPC job database)
		public string HangfireDatabaseConnectionString { get; set; }
	}
}