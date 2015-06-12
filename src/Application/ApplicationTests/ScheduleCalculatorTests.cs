using System;

using FluentAssertions;

using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;

using Moq;

using NUnit.Framework;

namespace Ica.StackIt.Application.ApplicationTests
{
	[Ignore("Time zone issues; for now, we assume StackIt will run on a machine configured to central timezone. Fix me, eventually.")]
	public class ScheduleCalculatorTests
	{
		private Mock<IRepository<Schedule>> ScheduleRepositoryMock { get; set; }
		private Mock<IClock> ClockMock { get; set; }
		private ScheduleCalculator Calculator { get; set; }

		[SetUp]
		public void SetUp()
		{
			ScheduleRepositoryMock = new Mock<IRepository<Schedule>>();
			ClockMock = new Mock<IClock>();
			Calculator = new ScheduleCalculator(ScheduleRepositoryMock.Object, ClockMock.Object);
		}

		[Test]
		public void GetNextStart_Ok()
		{
			// Arrange
			var stack = new Stack {ScheduleId = Guid.NewGuid()};
			var schedule = new Schedule {Id = stack.ScheduleId, StartCron = "* 8 * * 1-5"};
			ScheduleRepositoryMock.Setup(x => x.Find(schedule.Id)).Returns(schedule);
			ClockMock.Setup(x => x.Now).Returns(new DateTime(2015, 10, 1, 13, 0, 0));
			var expected = new TimeSpan(19, 0, 0);

			// Act
			var result = Calculator.GetNextStart(stack);

			// Assert
			result.Should().Be(expected);
		}

		[Test]
		public void GetNextStop_Ok()
		{
			// Arrange
			var stack = new Stack { ScheduleId = Guid.NewGuid() };
			var schedule = new Schedule { Id = stack.ScheduleId, StopCron = "* 18 * * 1-5" };
			ScheduleRepositoryMock.Setup(x => x.Find(schedule.Id)).Returns(schedule);
			ClockMock.Setup(x => x.Now).Returns(new DateTime(2015, 10, 1, 17, 0, 0));
			var expected = new TimeSpan(1, 0, 0);

			// Act
			var result = Calculator.GetNextStop(stack);

			// Assert
			result.Should().Be(expected);
		}

		[Test]
		public void GetNextStop_ServerInNonCentralTimeZone_Ok()
		{
			// Arrange
			var stack = new Stack { ScheduleId = Guid.NewGuid() };
			var schedule = new Schedule { Id = stack.ScheduleId, StopCron = "* 18 * * 1-5" };
			ScheduleRepositoryMock.Setup(x => x.Find(schedule.Id)).Returns(schedule);
			var oneOClockEastern = new DateTime(2015, 10, 1, 13, 0, 0);
			var easternTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
			oneOClockEastern = TimeZoneInfo.ConvertTime(oneOClockEastern, easternTimeZone);

			ClockMock.Setup(x => x.Now).Returns(oneOClockEastern); // == 2 PM central
			var expected = new TimeSpan(4, 0, 0);

			// Act
			var result = Calculator.GetNextStop(stack);

			// Assert
			result.Should().Be(expected);
		}
	}
}