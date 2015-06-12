using System.Configuration;

namespace Ica.StackIt.Application.Configuration
{
	public class QueueingConfiguration : IQueueingConfiguration
	{
		public string UnorderedCommandQueue
		{
			get { return ConfigurationManager.AppSettings["UnorderedCommandQueue"]; }
		}

		public string OrderedCommandQueue
		{
			get { return ConfigurationManager.AppSettings["OrderedCommandQueue"]; }
		}
	}
}