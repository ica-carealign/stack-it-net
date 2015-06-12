using Ica.StackIt.Application.AWS;

namespace Ica.StackIt.Application
{
	public interface IAwsClient
	{
		IInstanceService InstanceService { get; }
		IStackService StackService { get; }
		IStorageService StorageService { get; }
		IDnsService DnsService { get; }
		IImageService ImageService { get; }
		ISecurityGroupService SecurityGroupService { get; }
		INetworkService NetworkService { get; }
		IIdentityService IdentityService { get; }
	}
}