using System;
using System.Configuration;

namespace Ica.StackIt.Application.Configuration
{
	public class PuppetConfiguration : IPuppetConfiguration
	{
		public string BootstrapperUrl
		{
			get { return ConfigurationManager.AppSettings["BootstrapperUrl"]; }
		}

		public string PuppetInstallerUrl
		{
			get { return ConfigurationManager.AppSettings["PuppetInstallerUrl"]; }
		}

		public string PuppetHost
		{
			get { return ConfigurationManager.AppSettings["PuppetHost"]; }
		}

		public string PuppetCleanupHost
		{
			get { return ConfigurationManager.AppSettings["PuppetCleanupHost"]; }
		}

		public int NodeServicePort
		{
			get { return Convert.ToInt32(ConfigurationManager.AppSettings["NodeServicePort"]); }
		}
	}
}