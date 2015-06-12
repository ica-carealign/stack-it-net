using System.Collections.Generic;

using Ica.StackIt.Application.Enums;

namespace Ica.StackIt.Application.Configuration
{
	public interface IStackItConfiguration
	{
		ICloudOptions CloudOptions { get; }
		AuthenticationProvider AuthenticationProvider { get; }
		ICrowdConfiguration CrowdConfiguration { get; }
		IQueueingConfiguration QueueingConfiguration { get; }
		IPuppetConfiguration PuppetConfiguration { get; }

		/// <summary>
		/// A whitelist of instance types available for users to choose when creating new stacks
		/// </summary>
		IList<IInstanceTypeConfiguration> InstanceTypes { get; }
	}
}