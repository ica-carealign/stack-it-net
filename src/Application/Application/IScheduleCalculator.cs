using System;

using Ica.StackIt.Core.Entities;

namespace Ica.StackIt.Application
{
	public interface IScheduleCalculator
	{
		TimeSpan GetNextStart(Stack stack);

		TimeSpan GetNextStop(Stack stack);
	}
}