using System;

using Ica.StackIt.Application.AWS;
using Ica.StackIt.Application.Command;
using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;

using Moq;

using NUnit.Framework;

using AwsInstance = Amazon.EC2.Model.Instance;

namespace Ica.StackIt.Application.ApplicationTests.Command
{
	internal class UpdateInstanceTests
	{
		public Mock<IAwsClientFactory> AwsClientFactoryMock { get; set; }
		public Mock<IRepository<AwsProfile>> AwsProfileRepositoryMock { get; set; }
		public Mock<IRepository<Instance>> InstanceRepositoryMock { get; set; }
		public Mock<IInstanceService> InstanceServiceMock { get; set; }
		public UpdateInstance Command { get; set; }

		[SetUp]
		public void Setup()
		{
			AwsProfileRepositoryMock = new Mock<IRepository<AwsProfile>>();
			AwsClientFactoryMock = new Mock<IAwsClientFactory>();
			InstanceRepositoryMock = new Mock<IRepository<Instance>>();
			InstanceServiceMock = new Mock<IInstanceService>();
			Command = new UpdateInstance(AwsProfileRepositoryMock.Object, AwsClientFactoryMock.Object, InstanceRepositoryMock.Object);
		}

		[Test]
		public void ExitEarlyIfProfileIsMissing()
		{
			// Arrange
			AwsProfileRepositoryMock.Setup(x => x.Find(It.IsAny<Guid>())).Returns((AwsProfile)null);

			// Act
			Command.Execute(Guid.NewGuid(), "HoojeyId");

			// Assert
			InstanceServiceMock.Verify(x => x.GetSecurityGroups(It.IsAny<AwsInstance>()), Times.Never);
			
		}

		[Test]
		public void ExitEarlyIfInstanceIdIsNotAtAmazon()
		{
			// Arrange
			var profile = new AwsProfile();
			AwsProfileRepositoryMock.Setup(x => x.Find(profile.Id)).Returns(profile);
			InstanceServiceMock.Setup(x => x.GetInstance(It.IsAny<string>())).Returns((AwsInstance) null);
			AwsClientFactoryMock.Setup(x => x.GetClient(profile).InstanceService).Returns(InstanceServiceMock.Object);

			// Act
			Command.Execute(profile.Id, "HoojeyId");

			// Assert
			InstanceServiceMock.Verify(x => x.GetSecurityGroups(It.IsAny<AwsInstance>()), Times.Never);
		}
	}
}