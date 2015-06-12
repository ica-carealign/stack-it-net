using System;

using Hangfire;
using Hangfire.Common;
using Hangfire.States;

using Ica.StackIt.Application.Command;
using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;

using Moq;

using NUnit.Framework;

namespace Ica.StackIt.Application.ApplicationTests.Command
{
	internal class RefreshEverythingTests
	{
		private Mock<IRepository<AwsProfile>> ProfileRepositoryMock { get; set; }
		private Mock<IBackgroundJobClient> BackgroundJobClientMock { get; set; }
		private RefreshEverything RefreshEverything { get; set; }

		[SetUp]
		public void SetUp()
		{
			ProfileRepositoryMock = new Mock<IRepository<AwsProfile>>();
			BackgroundJobClientMock = new Mock<IBackgroundJobClient>();
			RefreshEverything = new RefreshEverything(
				ProfileRepositoryMock.Object,
				BackgroundJobClientMock.Object
				);
		}

		[Test]
		public void Execute()
		{
			// Arrange
			var profiles = new[]
			{
				new AwsProfile {Id = Guid.NewGuid()},
				new AwsProfile {Id = Guid.NewGuid()}
			};
			ProfileRepositoryMock.Setup(x => x.FindAll()).Returns(profiles);

			// Act
			RefreshEverything.Execute();

			// Assert

			// This is good enough for now, but it isn't great because Execute calls an extension method on IBackgroundJobClient.
			// One can not verify that extension methods were called.
			BackgroundJobClientMock.Verify(x => x.Create(
				It.Is<Job>(j => j.Type == typeof (UpdateAllInstances)),
				It.IsAny<IState>()),
				Times.Exactly(profiles.Length));

			BackgroundJobClientMock.Verify(x => x.Create(
				It.Is<Job>(j => j.Type == typeof (UpdateAllStacks)),
				It.IsAny<IState>()),
				Times.Exactly(profiles.Length));
		}
	}
}