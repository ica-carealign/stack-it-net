using System;
using System.Collections.Generic;
using System.Linq;

using Amazon.EC2.Model;

using Ica.StackIt.Application.AWS;
using Ica.StackIt.Application.Properties;

using Newtonsoft.Json;

namespace Ica.StackIt.Application
{
	public class DefaultSecurityGroupCreator
	{
		private readonly ISecurityGroupService _securityGroupService;

		public string SecurityGroupName { get; set; }

		public DefaultSecurityGroupCreator(ISecurityGroupService securityGroupService)
		{
			_securityGroupService = securityGroupService;
			SecurityGroupName = Conventions.DefaultSecurityGroupName;
		}

		/// <summary>
		///     Create the default security group if it doesn't exist.
		/// </summary>
		/// <returns>
		///     The group ID of the default security group.
		/// </returns>
		public string TryCreateDefaultSecurityGroup(string vpcId)
		{
			SecurityGroup existingSg = _securityGroupService.GetSecurityGroup(SecurityGroupName, vpcId);
			if (existingSg != null)
			{
				// The default security group already exists so don't do any work
				return existingSg.GroupId;
			}

			var json = JsonConvert.DeserializeObject<SecurityGroupConfiguration>(Resources.DefaultSecurityGroupAddresses);
			List<IpPermission> ipPermissions = json.Ingress.Select(x => new IpPermission
			{
				FromPort = Convert.ToInt32(x.Port),
				ToPort = Convert.ToInt32(x.Port),
				IpProtocol = x.Protocol.ToLower(),
				IpRanges = new List<string> {x.Source}
			}).ToList();

			string groupId = _securityGroupService.CreateSecurityGroup(SecurityGroupName, SecurityGroupName, vpcId);
			_securityGroupService.SetSecurityGroupRules(groupId, ipPermissions);
			return groupId;
		}

		internal class SecurityGroupConfiguration
		{
			public List<PortConfiguration> Ingress { get; set; }
		}

		internal class PortConfiguration
		{
			public string Type { get; set; }
			public string Protocol { get; set; }
			public string Port { get; set; }
			public string Source { get; set; }
		}
	}
}