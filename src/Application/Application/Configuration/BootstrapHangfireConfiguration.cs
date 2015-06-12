namespace Ica.StackIt.Application.Configuration
{
	public class BootstrapHangfireConfiguration : IHangfireConfiguration
	{
		public BootstrapHangfireConfiguration(string connectionString)
		{
			ConnectionString = connectionString;
		}

		public string ConnectionString { get; private set; }
	}
}