using System.Collections.Generic;
using System.Linq;

using Amazon.EC2;
using Amazon.EC2.Model;

namespace Ica.StackIt.Application.AWS
{
	public class ImageService : IImageService
	{
		private const string _filterName = "tag-key";
		private readonly IAmazonEC2 _ec2Client;

		public ImageService(IAmazonEC2 ec2Client)
		{
			_ec2Client = ec2Client;
		}

		public List<Image> GetImagesByTagName(string tagName)
		{
			var baseImageTagFilter = new Filter(_filterName, new List<string> {tagName});

			var request = new DescribeImagesRequest
			{
				Filters = new List<Filter> {baseImageTagFilter}
			};

			DescribeImagesResponse response = _ec2Client.DescribeImages(request);
			return response.Images;
		}

		public List<Image> GetImages(IEnumerable<string> imageIds)
		{
			var request = new DescribeImagesRequest
			{
				ImageIds = imageIds.AsList()
			};
			DescribeImagesResponse response = _ec2Client.DescribeImages(request);
			return response.Images;
		}

		public Dictionary<string, Image> GetImageMap(IEnumerable<Instance> instances)
		{
			List<Instance> instanceList = instances.AsList();
			List<Image> images = GetImages(instanceList.Select(x => x.ImageId));
			Dictionary<string, Image> imageByImageId = images.ToDictionary(image => image.ImageId);

			var result = new Dictionary<string, Image>();
			foreach (Instance instance in instanceList)
			{
				Image image;
				if (imageByImageId.TryGetValue(instance.ImageId, out image))
				{
					result[instance.InstanceId] = image;
				}
				else
				{
					result[instance.InstanceId] = null;
				}
			}
			return result;
		}
	}
}