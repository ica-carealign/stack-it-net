using System.Configuration;

namespace Ica.StackIt.Application.Configuration
{
	public class HangfireConfiguration : IHangfireConfiguration
	{
		public string ConnectionString
		{
			get { return ConfigurationManager.ConnectionStrings["HangfireDb"].ConnectionString; }
		}
	}
}