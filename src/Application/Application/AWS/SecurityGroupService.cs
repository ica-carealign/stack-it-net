using System.Collections.Generic;
using System.Linq;

using Amazon.EC2;
using Amazon.EC2.Model;

namespace Ica.StackIt.Application.AWS
{
	public class SecurityGroupService : ISecurityGroupService
	{
		private readonly IAmazonEC2 _ec2Client;

		public SecurityGroupService(IAmazonEC2 ec2Client)
		{
			_ec2Client = ec2Client;
		}

		public List<SecurityGroup> GetSecurityGroups(IEnumerable<GroupIdentifier> groupIdentifiers)
		{
			return GetSecurityGroups(groupIdentifiers.Select(x => x.GroupId));
		}

		public List<SecurityGroup> GetSecurityGroups(IEnumerable<string> groupIds)
		{
			List<string> groupIdsList = groupIds.AsList();
			var request = new DescribeSecurityGroupsRequest {GroupIds = groupIdsList.Distinct().ToList()};
			DescribeSecurityGroupsResponse response = _ec2Client.DescribeSecurityGroups(request);
			Dictionary<string, SecurityGroup> groupsBySgId = response.SecurityGroups.ToDictionary(sg => sg.GroupId);
			return groupIdsList.Where(groupsBySgId.ContainsKey).Select(x => groupsBySgId[x]).ToList();
		}

		public Dictionary<string, List<SecurityGroup>> GetSecurityGroupMap(IEnumerable<Instance> instances)
		{
			List<Instance> instanceList = instances.AsList();
			IEnumerable<string> groupIds = instanceList.SelectMany(x => x.SecurityGroups).Select(x => x.GroupId).Distinct();
			Dictionary<string, SecurityGroup> groupsBySgId = GetSecurityGroups(groupIds).ToDictionary(sg => sg.GroupId);
			return instanceList.ToDictionary(
				instance => instance.InstanceId,
				instance => instance.SecurityGroups.Select(sgid => groupsBySgId[sgid.GroupId]).ToList()
				);
		}

		public SecurityGroup GetSecurityGroup(string groupName, string vpcId)
		{
			/* Do not try to initialize GroupNames in the describe request and expect AWS to do server side filtering.
			 *
			 * If you do, and the security group doesn't exist, the SDK throws an AmazonEC2Exception with the message
			 * "the security group {groupName} does not exist." or something similar.
			 *
			 * If you do, and the security group *does* exist, the SDK throws an AmazonEC2Exception with the message
			 * "GroupName is not allowed in this operation. Use the GroupId instead." or something similar.
			 *
			 * It stinks, but that's why the filtering is done on the client side. If you try to fix this and fail,
			 * increment this counter: 1
			*/
			var request = new DescribeSecurityGroupsRequest();
			DescribeSecurityGroupsResponse response = _ec2Client.DescribeSecurityGroups(request);
			return response.SecurityGroups.SingleOrDefault(x => x.VpcId == vpcId && x.GroupName == groupName);
		}

		public string CreateSecurityGroup(string groupName, string description, string vpcId)
		{
			var createSecurityGroupRequest = new CreateSecurityGroupRequest
			{
				Description = description,
				GroupName = groupName,
				VpcId = vpcId
			};
			var response = _ec2Client.CreateSecurityGroup(createSecurityGroupRequest);
			return response.GroupId;
		}

		public void SetSecurityGroupRules(string groupId, List<IpPermission> ipPermissions)
		{
			var request = new AuthorizeSecurityGroupIngressRequest
			{
				GroupId = groupId,
				IpPermissions = ipPermissions
			};

			_ec2Client.AuthorizeSecurityGroupIngress(request);
		}
	}
}