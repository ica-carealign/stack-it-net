namespace Ica.StackIt.Application.AWS
{
	public interface IMetadataService
	{
		bool TryGetInstanceId(out string instanceId);
	}
}