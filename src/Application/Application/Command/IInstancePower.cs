using System;
using System.Collections.Generic;

namespace Ica.StackIt.Application.Command
{
	public interface IInstancePower
	{
		void Execute(Guid profileId, List<string> instanceIds);
	}
}
