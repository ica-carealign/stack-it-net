using System.Collections.Generic;

using Amazon.EC2.Model;

namespace Ica.StackIt.Application.AWS
{
	public interface ISecurityGroupService
	{
		List<SecurityGroup> GetSecurityGroups(IEnumerable<GroupIdentifier> groupIds);

		List<SecurityGroup> GetSecurityGroups(IEnumerable<string> groupIds);

		Dictionary<string, List<SecurityGroup>> GetSecurityGroupMap(IEnumerable<Instance> instances);

		SecurityGroup GetSecurityGroup(string groupName, string vpcId);

		string CreateSecurityGroup(string groupName, string description, string vpcId);

		void SetSecurityGroupRules(string groupId, List<IpPermission> ipPermissions);
	}
}