using System.Configuration;

namespace Ica.StackIt.Infrastructure
{
	public class MongoDatabaseConfiguration : IDatabaseConfiguration
	{
		public string ConnectionString
		{
			get
			{
				var connectionString = ConfigurationManager.ConnectionStrings["StackIt"];
				return connectionString == null ? "mongodb://127.0.0.1/StackItDev" : connectionString.ConnectionString;
			}
		}
	}
}