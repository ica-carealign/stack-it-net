using Amazon.Runtime;

using Ica.StackIt.Application.AWS;
using Ica.StackIt.Application.Configuration;
using Ica.StackIt.Core.Entities;

namespace Ica.StackIt.Application
{
	public class AwsClientFactory : IAwsClientFactory
	{
		private readonly ICredentialService _credentialService;
		private readonly IStackItConfiguration _configuration;

		public AwsClientFactory(ICredentialService credentialService, IStackItConfiguration configuration)
		{
			_credentialService = credentialService;
			_configuration = configuration;
		}

		public IAwsClient GetClient(AwsProfile profile)
		{
			AWSCredentials awsCredentials = _credentialService.GetCredentials(profile);
			return new AwsClient(awsCredentials, _configuration);
		}

		/// <summary>
		///     Get an AWS client (ICA type) by supplying AWS credentials (Amazon type)
		/// </summary>
		/// <param name="credentials"></param>
		/// <returns></returns>
		/// <remarks>
		///     For most use cases, the GetClient(AwsProfile profile) method should be used. Use this overload
		///     a complete AwsProfile is otherwise unavailable i.e. upon profile creation.
		/// </remarks>
		public IAwsClient GetClient(AWSCredentials credentials)
		{
			return new AwsClient(credentials, _configuration);
		}
	}
}