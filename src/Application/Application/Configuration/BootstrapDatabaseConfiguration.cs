using Ica.StackIt.Infrastructure;

namespace Ica.StackIt.Application.Configuration
{
	public class BootstrapDatabaseConfiguration : IDatabaseConfiguration
	{
		public BootstrapDatabaseConfiguration(string connectionString)
		{
			ConnectionString = connectionString;
		}

		public string ConnectionString { get; private set; }
	}
}