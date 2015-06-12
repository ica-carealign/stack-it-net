using System.Configuration;

namespace Ica.StackIt.Application.Configuration
{
	public class CrowdConfiguration : ICrowdConfiguration
	{
		public CrowdConfiguration()
		{
			Url = ConfigurationManager.AppSettings.Get("CrowdUrl") ?? "https://crowd.example.com:8443/crowd/services/SecurityServer";
			ApplicationName = ConfigurationManager.AppSettings.Get("CrowdApplicationName");
			ApiPassword = ConfigurationManager.AppSettings.Get("CrowdApiPassword");
		}

		public string Url { get; private set; }
		public string ApplicationName { get; private set; }
		public string ApiPassword { get; private set; }
	}
}
