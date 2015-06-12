using Amazon.Runtime;

using Ica.StackIt.Core.Entities;

namespace Ica.StackIt.Application.AWS
{
	public interface ICredentialService
	{
		AWSCredentials GetCredentials(AwsProfile profileName);
	}
}