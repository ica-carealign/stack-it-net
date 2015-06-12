using Ica.StackIt.Core.Entities;

namespace Ica.StackIt.Application.Command
{
	public interface ICleanUpPuppet
	{
		void CleanUp(Instance instance);
	}
}