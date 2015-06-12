using System;
using System.Collections.Generic;

using Hangfire;
using Hangfire.Common;
using Hangfire.States;

using Ica.StackIt.Application.AWS;
using Ica.StackIt.Application.Command;
using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;

using Moq;

using NUnit.Framework;

namespace Ica.StackIt.Application.ApplicationTests.Command
{
	internal class RemoveStaleInstanceTest
	{
		public Mock<IAwsClientFactory> AwsClientFactoryMock { get; set; }
		public Mock<IRepository<Instance>> StackRepositoryMock { get; set; }
		public Mock<IRepository<AwsProfile>> ProfileRepositoryMock { get; set; }
		public Mock<IInstanceService> InstanceServiceMock { get; set; }
		public Mock<IBackgroundJobClient> BackgroundJobClientMock { get; set; }
		public RemoveStaleInstances Command { get; set; }
		public AwsProfile Profile { get; set; }

		[SetUp]
		public void SetUp()
		{
			AwsClientFactoryMock = new Mock<IAwsClientFactory>();
			StackRepositoryMock = new Mock<IRepository<Instance>>();
			ProfileRepositoryMock = new Mock<IRepository<AwsProfile>>();
			InstanceServiceMock = new Mock<IInstanceService>();
			BackgroundJobClientMock = new Mock<IBackgroundJobClient>();

			Profile = new AwsProfile {Id = Guid.NewGuid()};
			ProfileRepositoryMock.Setup(x => x.Find(Profile.Id)).Returns(Profile);

			Command = new RemoveStaleInstances(
				AwsClientFactoryMock.Object,
				StackRepositoryMock.Object,
				ProfileRepositoryMock.Object,
				BackgroundJobClientMock.Object
			);
		}

		[Test]
		public void RemoveStaleInstances()
		{
			// Arrange
			var awsInstances = new List<Amazon.EC2.Model.Instance>
			{
				new Amazon.EC2.Model.Instance {InstanceId = "One"},
				new Amazon.EC2.Model.Instance {InstanceId = "Two"},
				new Amazon.EC2.Model.Instance {InstanceId = "Five"},
			};
			InstanceServiceMock.Setup(x => x.GetAllInstances()).Returns(awsInstances);

			AwsClientFactoryMock.Setup(x => x.GetClient(Profile).InstanceService).Returns(InstanceServiceMock.Object);

			var instances = new List<Instance>
			{
				new Instance {ResourceId = "One", OwnerProfileId = Profile.Id},
				new Instance {ResourceId = "Two", OwnerProfileId = Profile.Id},
				new Instance {ResourceId = "Three", OwnerProfileId = Profile.Id},
				new Instance {ResourceId = "Four", OwnerProfileId = Profile.Id}
			};
			StackRepositoryMock.Setup(x => x.FindAll()).Returns(instances);

			// Act
			Command.Execute(Profile.Id);

			// Assert
			StackRepositoryMock.Verify(x => x.Delete(It.Is<Instance>(s => s.ResourceId == "Three")));
			StackRepositoryMock.Verify(x => x.Delete(It.Is<Instance>(s => s.ResourceId == "Four")));

			// Ensure no stacks were deleted that should not have been
			StackRepositoryMock.Verify(x => x.Delete(It.IsAny<Instance>()), Times.Exactly(2));
		}

		[Test]
		public void RemoveStaleInstances_DontRemoveInstancesThatNeedRefreshing_Ok()
		{
			// Arrange
			var awsInstances = new List<Amazon.EC2.Model.Instance>
			{
				new Amazon.EC2.Model.Instance {InstanceId = "One"},
			};
			InstanceServiceMock.Setup(x => x.GetAllInstances()).Returns(awsInstances);

			AwsClientFactoryMock.Setup(x => x.GetClient(Profile).InstanceService).Returns(InstanceServiceMock.Object);

			var instances = new List<Instance>
			{
				new Instance {ResourceId = "One", OwnerProfileId = Profile.Id},
				new Instance {ResourceId = "Two", NeedsRefreshing = true, OwnerProfileId = Profile.Id},
				new Instance {ResourceId = "Three", OwnerProfileId = Profile.Id}
			};
			StackRepositoryMock.Setup(x => x.FindAll()).Returns(instances);

			// Act
			Command.Execute(Profile.Id);

			// Assert
			StackRepositoryMock.Verify(x => x.Delete(It.Is<Instance>(s => s.ResourceId == "Three")));

			// Ensure no stacks were deleted that should not have been
			StackRepositoryMock.Verify(x => x.Delete(It.IsAny<Instance>()), Times.Exactly(1));
		}

		[Test]
		public void RemoveStaleInstances_CleanUpInstances_Ok()
		{
			// Arrange
			var awsInstances = new List<Amazon.EC2.Model.Instance>
			{
				new Amazon.EC2.Model.Instance {InstanceId = "One"},
			};
			InstanceServiceMock.Setup(x => x.GetAllInstances()).Returns(awsInstances);

			AwsClientFactoryMock.Setup(x => x.GetClient(Profile).InstanceService).Returns(InstanceServiceMock.Object);

			var instances = new List<Instance>
			{
				new Instance {ResourceId = "One", OwnerProfileId = Profile.Id},
				new Instance {ResourceId = "Two", NeedsRefreshing = true, OwnerProfileId = Profile.Id},
				new Instance {ResourceId = "Three", OwnerProfileId = Profile.Id}
			};
			StackRepositoryMock.Setup(x => x.FindAll()).Returns(instances);

			// Act
			Command.Execute(Profile.Id);

			// Assert
			BackgroundJobClientMock.Verify(x => x.Create(It.IsAny<Job>(), It.IsAny<IState>()), Times.Once());
		}
	}
}