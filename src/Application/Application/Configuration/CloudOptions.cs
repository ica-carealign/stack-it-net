using System.Configuration;

namespace Ica.StackIt.Application.Configuration
{
	public class CloudOptions : ICloudOptions
	{
		public string MasterKeyAlias
		{
			get { return ConfigurationManager.AppSettings["MasterKeyAlias"]; }
		}

		public string ConfigurationTemplateBucket
		{
			get { return ConfigurationManager.AppSettings["ConfigurationTemplateBucket"]; }
		}
	}
}