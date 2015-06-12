using System;

using FluentAssertions;

using Ica.StackIt.Infrastructure;

using NUnit.Framework;

namespace Ica.StackIt.InfrastructureTests
{
	public class SystemClockTests
	{
		[Test]
		public void Now_CorrectTime()
		{
			// Arrange
			var clock = new SystemClock();
			DateTime currentTime = DateTime.Now;

			// Act
			DateTime clockTime = clock.Now;

			// Assert
			TimeSpan timeDiff = clockTime - currentTime;
			double seconds = Math.Abs(timeDiff.TotalSeconds);
			seconds.Should().BeLessOrEqualTo(1);
		}

		[Test]
		public void Now_CorrectKind()
		{
			// Arrange
			var clock = new SystemClock();

			// Act
			DateTimeKind kind = clock.Now.Kind;

			// Assert
			kind.Should().Be(DateTimeKind.Local);
		}

		[Test]
		public void UtcNow_CorrectTime()
		{
			// Arrange
			var clock = new SystemClock();
			DateTime currentTime = DateTime.UtcNow;

			// Act
			DateTime clockTime = clock.UtcNow;

			// Assert
			TimeSpan timeDiff = clockTime - currentTime;
			double seconds = Math.Abs(timeDiff.TotalSeconds);
			seconds.Should().BeLessOrEqualTo(1);
		}

		[Test]
		public void UtcNow_CorrectKind()
		{
			// Arrange
			var clock = new SystemClock();

			// Act
			DateTimeKind kind = clock.UtcNow.Kind;

			// Assert
			kind.Should().Be(DateTimeKind.Utc);
		}
	}
}