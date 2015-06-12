using System.Collections.Generic;
using System.Linq;

using Amazon.EC2;
using Amazon.EC2.Model;

namespace Ica.StackIt.Application.AWS
{
	public class NetworkService : INetworkService
	{
		private readonly IAmazonEC2 _ec2Client;

		public NetworkService(IAmazonEC2 ec2Client)
		{
			_ec2Client = ec2Client;
		}

		public IEnumerable<Vpc> GetVpcs()
		{
			var request = new DescribeVpcsRequest();
			var response = _ec2Client.DescribeVpcs(request);
			return response.Vpcs;
		}

		public IEnumerable<Subnet> GetSubnets(string vpcId)
		{
			var request = new DescribeSubnetsRequest();
			var response = _ec2Client.DescribeSubnets(request);
			return response.Subnets.Where(x => x.VpcId == vpcId);
		}

		public string GetCidrBySubnetId(string subnetId)
		{
			var request = new DescribeSubnetsRequest {SubnetIds = new List<string> {subnetId}};
			var response = _ec2Client.DescribeSubnets(request);

			return response.Subnets.Single().CidrBlock;
		}

		public IEnumerable<string> GetAllocatedIpAddresses()
		{
			var request = new DescribeInstancesRequest();
			var response = _ec2Client.DescribeInstances(request);

			return response.Reservations
				.SelectMany(x => x.Instances.Select(y => y.PrivateIpAddress))
				.Where(x => x != null);
		}
	}
}