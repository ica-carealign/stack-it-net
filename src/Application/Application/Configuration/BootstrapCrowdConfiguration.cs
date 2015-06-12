namespace Ica.StackIt.Application.Configuration
{
	public class BootstrapCrowdConfiguration : ICrowdConfiguration
	{
		public BootstrapCrowdConfiguration(string url, string applicationName, string apiPassword)
		{
			Url = url;
			ApplicationName = applicationName;
			ApiPassword = apiPassword;
		}

		public string Url { get; private set; }
		public string ApplicationName { get; private set; }
		public string ApiPassword { get; private set; }
	}
}