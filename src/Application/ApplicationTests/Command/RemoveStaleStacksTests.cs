using System;
using System.Collections.Generic;

using Ica.StackIt.Application.AWS;
using Ica.StackIt.Application.Command;
using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;

using Moq;

using NUnit.Framework;

namespace Ica.StackIt.Application.ApplicationTests.Command
{
	internal class RemoveStaleStacksTests
	{
		public Mock<IAwsClientFactory> AwsClientFactoryMock { get; set; }
		public Mock<IRepository<Stack>> StackRepositoryMock { get; set; }
		public Mock<IRepository<AwsProfile>> ProfileRepositoryMock { get; set; }
		public Mock<IStackService> StackServiceMock { get; set; }
		public RemoveStaleStacks Command { get; set; }
		AwsProfile Profile { get; set; }

		[SetUp]
		public void SetUp()
		{
			AwsClientFactoryMock = new Mock<IAwsClientFactory>();
			StackRepositoryMock = new Mock<IRepository<Stack>>();
			ProfileRepositoryMock = new Mock<IRepository<AwsProfile>>();
			StackServiceMock = new Mock<IStackService>();

			Profile = new AwsProfile {Id = Guid.NewGuid()};
			ProfileRepositoryMock.Setup(x => x.Find(Profile.Id)).Returns(Profile);

			Command = new RemoveStaleStacks(AwsClientFactoryMock.Object, StackRepositoryMock.Object, ProfileRepositoryMock.Object);
		}

		[Test]
		public void RemoveStaleStacks()
		{
			// Arrange
			var awsStacks = new List<Amazon.CloudFormation.Model.Stack>
			{
				new Amazon.CloudFormation.Model.Stack {StackId = "One"},
				new Amazon.CloudFormation.Model.Stack {StackId = "Two"},
				new Amazon.CloudFormation.Model.Stack {StackId = "Five"},
			};
			StackServiceMock.Setup(x => x.GetAllStacks()).Returns(awsStacks);

			AwsClientFactoryMock.Setup(x => x.GetClient(Profile).StackService).Returns(StackServiceMock.Object);

			var stacks = new List<Stack>
			{
				new Stack {ResourceId = "One", OwnerProfileId = Profile.Id},
				new Stack {ResourceId = "Two", OwnerProfileId = Profile.Id},
				new Stack {ResourceId = "Three", OwnerProfileId = Profile.Id},
				new Stack {ResourceId = "Four", OwnerProfileId = Profile.Id}
			};
			StackRepositoryMock.Setup(x => x.FindAll()).Returns(stacks);

			// Act
			Command.Execute(Profile.Id);

			// Assert
			StackRepositoryMock.Verify(x => x.Delete(It.Is<Stack>(s => s.ResourceId == "Three")));
			StackRepositoryMock.Verify(x => x.Delete(It.Is<Stack>(s => s.ResourceId == "Four")));

			// Ensure no stacks were deleted that should not have been
			StackRepositoryMock.Verify(x => x.Delete(It.IsAny<Stack>()), Times.Exactly(2));
		}

		[Test]
		public void RemoveStaleStacks_DontRemoveStacksThatNeedRefreshing()
		{
			// Arrange
			var awsStacks = new List<Amazon.CloudFormation.Model.Stack>
			{
				new Amazon.CloudFormation.Model.Stack {StackId = "One"}
			};
			StackServiceMock.Setup(x => x.GetAllStacks()).Returns(awsStacks);

			AwsClientFactoryMock.Setup(x => x.GetClient(Profile).StackService).Returns(StackServiceMock.Object);

			var stacks = new List<Stack>
			{
				new Stack {ResourceId = "One", OwnerProfileId = Profile.Id},
				new Stack {ResourceId = "Two", NeedsRefreshing = false, OwnerProfileId = Profile.Id},
				new Stack {Id = Guid.NewGuid(), NeedsRefreshing = true, OwnerProfileId = Profile.Id}
			};
			StackRepositoryMock.Setup(x => x.FindAll()).Returns(stacks);

			// Act
			Command.Execute(Profile.Id);

			// Assert
			StackRepositoryMock.Verify(x => x.Delete(It.Is<Stack>(s => s.ResourceId == "Two")));

			StackRepositoryMock.Verify(x => x.Delete(It.IsAny<Stack>()), Times.Exactly(1));
		}
	}
}