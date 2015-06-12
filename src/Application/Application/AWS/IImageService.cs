using System.Collections.Generic;

using Amazon.EC2.Model;

namespace Ica.StackIt.Application.AWS
{
	public interface IImageService
	{
		List<Image> GetImages(IEnumerable<string> imageIds);

		List<Image> GetImagesByTagName(string tagName);

		Dictionary<string, Image> GetImageMap(IEnumerable<Instance> instances);
	}
}