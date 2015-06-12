using System.Collections.Generic;

using Amazon.CloudFormation.Model;

namespace Ica.StackIt.Application.AWS
{
	public interface IStackService
	{
		void CreateStack(string stackName, string configuration);

		void DeleteStack(string stackName);

		IEnumerable<Stack> GetAllStacks();

		Stack GetStack(string stackName);

		IEnumerable<StackEvent> GetStackEvents(Stack stack);

		IEnumerable<StackResource> GetResources(Stack stack);
	}
}