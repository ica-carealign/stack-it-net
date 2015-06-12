using Amazon.Runtime;

using AwsContrib.EnvelopeCrypto;

using Ica.StackIt.Core.Entities;

namespace Ica.StackIt.Application.AWS
{
	public class CredentialService : ICredentialService
	{
		private readonly ICryptoProvider _cryptoProvider;

		public CredentialService(ICryptoProvider cryptoProvider)
		{
			_cryptoProvider = cryptoProvider;
		}

		public AWSCredentials GetCredentials(AwsProfile profile)
		{
			var secretAccessKey = _cryptoProvider.Decrypt(profile.EncryptedKey, profile.EncryptedSecretAccessKey);
			return new BasicAWSCredentials(profile.AccessKeyId, secretAccessKey);
		}
	}
}