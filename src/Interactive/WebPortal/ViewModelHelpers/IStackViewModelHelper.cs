using Ica.StackIt.Core.Entities;
using Ica.StackIt.Interactive.WebPortal.Models;

namespace Ica.StackIt.Interactive.WebPortal.ViewModelHelpers
{
	public interface IStackViewModelHelper
	{
		StackPowerViewModel CreateStackPowerViewModel(Stack stack);

		StackInstancesViewModel CreateStackInstancesViewModel(Stack stack);
	}
}