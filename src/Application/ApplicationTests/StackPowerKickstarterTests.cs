using System;

using Hangfire;
using Hangfire.Common;
using Hangfire.States;

using Ica.StackIt.Application.Command;
using Ica.StackIt.Core.Entities;

using Moq;

using NUnit.Framework;

namespace Ica.StackIt.Application.ApplicationTests
{
	public class StackPowerKickstarterTests
	{
		private Mock<IScheduleCalculator> ScheduleCalculatorMock { get; set; }
		private Mock<IBackgroundJobClient> BackgroundJobClientMock { get; set; }

		private StackPowerKickstarter Kickstarter { get; set; }

		[SetUp]
		public void SetUp()
		{
			ScheduleCalculatorMock = new Mock<IScheduleCalculator>();
			BackgroundJobClientMock = new Mock<IBackgroundJobClient>();
			Kickstarter = new StackPowerKickstarter(ScheduleCalculatorMock.Object, BackgroundJobClientMock.Object);
		}

		[Test]
		public void NextStartSooner_Ok()
		{
			// Arrange
			var stack = new Stack();
			ScheduleCalculatorMock.Setup(x => x.GetNextStart(stack)).Returns(new TimeSpan(0, 5, 0));
			ScheduleCalculatorMock.Setup(x => x.GetNextStop(stack)).Returns(new TimeSpan(0, 10, 0));

			// Act
			Kickstarter.KickstartSchedule(stack);

			// Assert
			BackgroundJobClientMock.Verify(x => x.Create(It.Is<Job>(j => j.Type == typeof(ScheduledStartStack)), It.IsAny<ScheduledState>()), Times.Once());
		}

		[Test]
		public void NextStopSooner_Ok()
		{
			// Arrange
			var stack = new Stack();
			ScheduleCalculatorMock.Setup(x => x.GetNextStart(stack)).Returns(new TimeSpan(0, 15, 0));
			ScheduleCalculatorMock.Setup(x => x.GetNextStop(stack)).Returns(new TimeSpan(0, 10, 0));

			// Act
			Kickstarter.KickstartSchedule(stack);

			// Assert
			BackgroundJobClientMock.Verify(x => x.Create(It.Is<Job>(j => j.Type == typeof(ScheduledStopStack)), It.IsAny<ScheduledState>()), Times.Once());
		}
	}
}