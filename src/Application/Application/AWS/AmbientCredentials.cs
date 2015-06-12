using System;
using System.Collections.Generic;
using System.Threading;

using Amazon.Runtime;

namespace Ica.StackIt.Application.AWS
{
	// Mimic the behavior of Amazon's internal FallbackCredentialsFactory.
	public static class AmbientCredentials
	{
		private static readonly Lazy<AWSCredentials> _lazyCredentials = new Lazy<AWSCredentials>(FindCredentials, LazyThreadSafetyMode.ExecutionAndPublication);

		private static readonly List<Func<AWSCredentials>> _credentialFactories = new List<Func<AWSCredentials>>
		{
			() => new EnvironmentAWSCredentials(),
			() => new StoredProfileAWSCredentials(),
			() => new EnvironmentVariablesAWSCredentials(),
			() => new InstanceProfileAWSCredentials()
		};

		/// <summary>
		///     Returns an AWSCredentials object using the best available ambient settings, such as environment variables or stored
		///     profiles. This is the same behavior that you get by creating a new AWS client object without passing in
		///     credentials.
		/// </summary>
		public static AWSCredentials GetCredentials()
		{
			return _lazyCredentials.Value;
		}

		private static AWSCredentials FindCredentials()
		{
			AWSCredentials credentials = null;
			var errors = new List<Exception>();
			foreach (Func<AWSCredentials> factory in _credentialFactories)
			{
				try
				{
					credentials = factory();
				}
				catch (Exception e)
				{
					errors.Add(e);
				}
				if (credentials != null)
				{
					break;
				}
			}
			if (credentials == null)
			{
				throw new AmazonServiceException(new AggregateException(errors));
			}
			return credentials;
		}
	}
}