using System.Collections.Generic;

using Amazon.EC2.Model;

namespace Ica.StackIt.Application.AWS
{
	public interface IInstanceService
	{
		IEnumerable<Instance> GetAllInstances();

		Instance GetInstance(string instanceId);

		IEnumerable<SecurityGroup> GetSecurityGroups(Instance instance);

		Image GetImage(Instance instance);

		void StopInstances(List<string> instanceIds);
		void StartInstances(List<string> instanceIds);
	}
}