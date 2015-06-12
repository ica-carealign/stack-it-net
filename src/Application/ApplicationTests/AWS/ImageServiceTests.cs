using System.Collections.Generic;
using System.Linq;

using Amazon.EC2;
using Amazon.EC2.Model;

using FluentAssertions;

using Ica.StackIt.Application.AWS;

using Moq;

using NUnit.Framework;

namespace Ica.StackIt.Application.ApplicationTests.AWS
{
	public class ImageServiceTests
	{
		private Mock<IAmazonEC2> Ec2Mock { get; set; }
		private ImageService ImageService { get; set; }

		[SetUp]
		public void SetUp()
		{
			Ec2Mock = new Mock<IAmazonEC2>();
			ImageService = new ImageService(Ec2Mock.Object);
		}

		[Test]
		public void GetAllImagesByTagName_Ok()
		{
			const string filterName = "tag-key";
			var images = new List<Image>();
			var response = new DescribeImagesResponse {Images = images};
			Ec2Mock.Setup(x => x.DescribeImages(It.Is<DescribeImagesRequest>(req => req.Filters.Single().Name == filterName
			                                                                        && req.Filters.Single().Values.Contains(Conventions.BaseImageTag))))
			       .Returns(response);

			List<Image> result = ImageService.GetImagesByTagName(Conventions.BaseImageTag);

			ReferenceEquals(images, result).Should().BeTrue();
		}

		[Test]
		public void GetImages_Ok()
		{
			Ec2Mock.Setup(x => x.DescribeImages(It.Is((DescribeImagesRequest req) => string.Join("", req.ImageIds) == "ab")))
			       .Returns((DescribeImagesRequest req) => new DescribeImagesResponse
			       {
				       Images = req.ImageIds.Select(x => new Image {ImageId = x, Name = x}).ToList()
			       });

			List<Image> result = ImageService.GetImages(new[] {"a", "b"});
			result.Count.Should().Be(2);
			result.Select(x => x.Name).Should().Equal(new[] {"a", "b"});
		}

		[Test]
		public void GetImageMap_Ok()
		{
			IEnumerable<Instance> instances = new[] {1, 2}.Select(id =>
				new Instance
				{
					InstanceId = string.Format("i-{0}", id),
					ImageId = string.Format("ami-{0}", id)
				});

			Ec2Mock.Setup(x => x.DescribeImages(It.IsAny<DescribeImagesRequest>()))
			       .Returns((DescribeImagesRequest req) => new DescribeImagesResponse
			       {
				       Images = req.ImageIds.Select(x => new Image {ImageId = x, Name = x + "-name"}).ToList()
			       });

			Dictionary<string, Image> map = ImageService.GetImageMap(instances);
			map["i-1"].Name.Should().Be("ami-1-name");
			map["i-2"].Name.Should().Be("ami-2-name");
		}
	}
}