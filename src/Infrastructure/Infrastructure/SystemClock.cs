using System;

using Ica.StackIt.Core;

namespace Ica.StackIt.Infrastructure
{
	public class SystemClock : IClock
	{
		public DateTime Now
		{
			get { return DateTime.Now; }
		}

		public DateTime UtcNow
		{
			get { return DateTime.UtcNow; }
		}
	}
}