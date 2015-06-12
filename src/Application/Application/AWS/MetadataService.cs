using Amazon.EC2.Util;

namespace Ica.StackIt.Application.AWS
{
	public class MetadataService : IMetadataService
	{
		public bool TryGetInstanceId(out string instanceId)
		{
			instanceId = null;
			string id = EC2Metadata.InstanceId;
			if (id == null)
			{
				return false;
			}

			instanceId = id;
			return true;
		}
	}
}