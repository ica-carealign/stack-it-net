using System;

namespace Ica.StackIt.Core
{
	public interface IClock
	{
		DateTime Now { get; }
		DateTime UtcNow { get; }
	}
}