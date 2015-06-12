using System.Collections.Generic;
using System.Linq;

using Amazon.CloudFormation;
using Amazon.CloudFormation.Model;

using FluentAssertions;

using Ica.StackIt.Application.AWS;

using Moq;

using NUnit.Framework;

namespace Ica.StackIt.Application.ApplicationTests.AWS
{
	internal class StackServiceTests
	{
		private Mock<IStorageService> StorageServiceMock { get; set; }
		private Mock<IAmazonCloudFormation> CloudFormationMock { get; set; }
		private StackService StackService { get; set; }

		[SetUp]
		public void SetUp()
		{
			CloudFormationMock = new Mock<IAmazonCloudFormation>();
			StorageServiceMock = new Mock<IStorageService>();
			StackService = new StackService(CloudFormationMock.Object, StorageServiceMock.Object, string.Empty);
		}

		[Test]
		public void GetAllStacks()
		{
			// Arrange
			var response = new DescribeStacksResponse
			{
				Stacks = new List<Stack>
				{
					new Stack {StackId = "one"},
					new Stack {StackId = "two"}
				}
			};
			CloudFormationMock.Setup(x => x.DescribeStacks()).Returns(response);

			// Act
			IEnumerable<Stack> stacks = StackService.GetAllStacks();

			// Assert
			stacks.Select(x => x.StackId).ShouldBeEquivalentTo(response.Stacks.Select(y => y.StackId));
		}

		[Test]
		public void GetStackByName()
		{
			// Arrange
			const string stackName = "three";
			var response = new DescribeStacksResponse
			{
				Stacks = new List<Stack>
				{
					new Stack {StackName = stackName}
				}
			};

			CloudFormationMock.Setup(x => x.DescribeStacks(It.Is<DescribeStacksRequest>(y => y.StackName == stackName))).Returns(response);

			// Act
			var stack = StackService.GetStack("three");

			// Assert
			stack.StackName.Should().Be(stackName);
		}
	}
}