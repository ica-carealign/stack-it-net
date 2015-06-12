using System.Collections.Generic;

using Ica.StackIt.Application.Enums;

namespace Ica.StackIt.Application.Configuration
{
	public class StackItConfiguration : IStackItConfiguration
	{
		public StackItConfiguration(
			AuthenticationProvider authenticationProvider,
			ICrowdConfiguration crowdConfiguration,
			IQueueingConfiguration queueingConfiguration,
			ICloudOptions cloudOptions,
			IList<IInstanceTypeConfiguration> instanceTypeConfigurations,
			IPuppetConfiguration puppetConfiguration
			)
		{
			AuthenticationProvider = authenticationProvider;
			CrowdConfiguration = crowdConfiguration;
			QueueingConfiguration = queueingConfiguration;
			CloudOptions = cloudOptions;
			InstanceTypes = instanceTypeConfigurations;
			PuppetConfiguration = puppetConfiguration;
		}

		public AuthenticationProvider AuthenticationProvider { get; private set; }
		public ICrowdConfiguration CrowdConfiguration { get; private set; }
		public IQueueingConfiguration QueueingConfiguration { get; private set; }
		public IPuppetConfiguration PuppetConfiguration { get; private set; }
		public ICloudOptions CloudOptions { get; private set; }

		public IList<IInstanceTypeConfiguration> InstanceTypes { get; private set; }
	}
}