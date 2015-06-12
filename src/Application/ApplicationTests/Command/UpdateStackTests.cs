using System;

using Ica.StackIt.Application.AWS;
using Ica.StackIt.Application.Command;
using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;

using Moq;

using NUnit.Framework;

using AwsStack = Amazon.CloudFormation.Model.Stack;

namespace Ica.StackIt.Application.ApplicationTests.Command
{
	internal class UpdateStackTests
	{
		private Mock<IAwsClientFactory> AwsClientFactoryMock { get; set; }
		private Mock<IRepository<Stack>> StackRepositoryMock { get; set; }
		private Mock<IRepository<Instance>> InstanceRepositoryMock { get; set; }
		private Mock<IRepository<AwsProfile>> ProfileRepositoryMock { get; set; }
		private Mock<IStackService> StackServiceMock { get; set; }

		private UpdateStack Command { get; set; }
		private AwsProfile Profile { get; set; }

		[SetUp]
		public void SetUp()
		{
			AwsClientFactoryMock = new Mock<IAwsClientFactory>();
			StackRepositoryMock = new Mock<IRepository<Stack>>();
			InstanceRepositoryMock = new Mock<IRepository<Instance>>();
			ProfileRepositoryMock = new Mock<IRepository<AwsProfile>>();
			StackServiceMock = new Mock<IStackService>();

			Profile = new AwsProfile {Id = Guid.NewGuid()};
			ProfileRepositoryMock.Setup(x => x.Find(Profile.Id)).Returns(Profile);

			var stackLoader = new StackLoader(StackRepositoryMock.Object, InstanceRepositoryMock.Object);
			Command = new UpdateStack(AwsClientFactoryMock.Object, ProfileRepositoryMock.Object, stackLoader);
		}

		[Test]
		public void ExitEarlyIfProfileMissing()
		{
			// Arrange
			ProfileRepositoryMock.Setup(x => x.Find(Profile.Id)).Returns((AwsProfile)null);

			// Act
			Command.Execute(Profile.Id, "HoojeyStack");

			// Assert
			StackRepositoryMock.Verify(x => x.FindAll(), Times.Never);
		}

		[Test]
		public void ExitEarlyIfStackIsNotAtAmazon()
		{
			// Arrange
			StackServiceMock.Setup(x => x.GetStack(It.IsAny<string>())).Returns((AwsStack) null);
			AwsClientFactoryMock.Setup(x => x.GetClient(Profile).StackService).Returns(StackServiceMock.Object);

			// Act
			Command.Execute(Profile.Id, "HoojeyStack");

			// Assert
			StackRepositoryMock.Verify(x => x.FindAll(), Times.Never);
		}
	}
}