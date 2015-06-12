using Amazon.Runtime;

using Ica.StackIt.Core.Entities;

namespace Ica.StackIt.Application
{
	public interface IAwsClientFactory
	{
		IAwsClient GetClient(AwsProfile profile);

		IAwsClient GetClient(AWSCredentials credentials);
	}
}