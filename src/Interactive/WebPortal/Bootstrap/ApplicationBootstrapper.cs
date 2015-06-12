using System.Linq;

using Amazon.EC2.Model;

using Ica.StackIt.Application;
using Ica.StackIt.Application.AWS;
using Ica.StackIt.Application.Configuration;

using Newtonsoft.Json;

namespace Ica.StackIt.Interactive.WebPortal.Bootstrap
{
	public class ApplicationBootstrapper
	{
		private readonly IInstanceService _instanceService;
		private readonly IStorageService _storageService;
		private readonly IMetadataService _metadataService;

		public ApplicationBootstrapper(
			IInstanceService instanceService,
			IStorageService storageService,
			IMetadataService metadataService)
		{
			_instanceService = instanceService;
			_storageService = storageService;
			_metadataService = metadataService;
		}

		public BootstrapData BootstrapApplication()
		{
			string instanceId;
			bool onEc2 = _metadataService.TryGetInstanceId(out instanceId);
			if (!onEc2)
			{
				return null;
			}

			Instance instance = _instanceService.GetInstance(instanceId);
			string path = instance.Tags.Single(x => x.Key == Conventions.BootstrapPathTag).Value;
			string contents = _storageService.GetFile(path);
			var bootstrapData = JsonConvert.DeserializeObject<BootstrapData>(contents);
			return bootstrapData;
		}
	}
}