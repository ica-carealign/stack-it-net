using System.Collections.Generic;

using Amazon.EC2.Model;

namespace Ica.StackIt.Application.AWS
{
	public interface INetworkService
	{
		IEnumerable<Vpc> GetVpcs();

		IEnumerable<Subnet> GetSubnets(string vpcId);

		string GetCidrBySubnetId(string subnetId);

		IEnumerable<string> GetAllocatedIpAddresses();
	}
}