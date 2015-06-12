using System.Collections.Generic;
using System.Linq;

using Amazon.EC2;
using Amazon.EC2.Model;

using FluentAssertions;

using Ica.StackIt.Application.AWS;

using Moq;

using NUnit.Framework;

namespace Ica.StackIt.Application.ApplicationTests.AWS
{
	public class SecurityGroupServiceTests
	{
		private Mock<IAmazonEC2> Ec2Mock { get; set; }
		private SecurityGroupService SecurityGroupService { get; set; }

		[SetUp]
		public void SetUp()
		{
			Ec2Mock = new Mock<IAmazonEC2>();
			SecurityGroupService = new SecurityGroupService(Ec2Mock.Object);
		}

		[Test]
		public void GetSecurityGroups_GroupIdentifier_Ok()
		{
			var groupIdentifiers = new List<GroupIdentifier>
			{
				new GroupIdentifier {GroupId = "a"},
				new GroupIdentifier {GroupId = "b"},
			};
			Ec2Mock.Setup(x => x.DescribeSecurityGroups(It.IsAny<DescribeSecurityGroupsRequest>()))
			       .Returns((DescribeSecurityGroupsRequest request) => new DescribeSecurityGroupsResponse
			       {
				       SecurityGroups = request.GroupIds.Select(x => new SecurityGroup {GroupId = x}).Reverse().ToList()
			       });
			List<SecurityGroup> result = SecurityGroupService.GetSecurityGroups(groupIdentifiers);
			result.Select(x => x.GroupId).Should().Equal(new[] {"a", "b"});
		}

		[Test]
		public void GetSecurityGroups_GroupIds_Ok()
		{
			Ec2Mock.Setup(x => x.DescribeSecurityGroups(It.IsAny<DescribeSecurityGroupsRequest>()))
			       .Returns((DescribeSecurityGroupsRequest request) => new DescribeSecurityGroupsResponse
			       {
				       SecurityGroups = request.GroupIds.Select(x => new SecurityGroup {GroupId = x}).ToList()
			       });
			List<SecurityGroup> result = SecurityGroupService.GetSecurityGroups(new[] {"a", "b"});
			result.OrderBy(x => x.GroupId).Select(x => x.GroupId).Should().Equal(new[] {"a", "b"});
		}

		[Test]
		public void GetSecurityGroups_GroupIds_Ordering()
		{
			// Switch up the order. Results should always be the same as the order given.
			Ec2Mock.Setup(x => x.DescribeSecurityGroups(It.IsAny<DescribeSecurityGroupsRequest>()))
			       .Returns((DescribeSecurityGroupsRequest request) => new DescribeSecurityGroupsResponse
			       {
				       SecurityGroups = request.GroupIds.Select(x => new SecurityGroup {GroupId = x}).Reverse().ToList()
			       });
			List<SecurityGroup> result = SecurityGroupService.GetSecurityGroups(new[] {"a", "b", "c"});
			result.Select(x => x.GroupId).Should().Equal(new[] {"a", "b", "c"});
		}

		[Test]
		public void GetSecurityGroupMap_Ok()
		{
			var instances = new List<Instance>();
			for (int sgCount = 0; sgCount < 3; sgCount++)
			{
				var instance = new Instance {InstanceId = string.Format("i-{0}", sgCount)};
				for (int i = 0; i < sgCount; i++)
				{
					instance.SecurityGroups.Add(new GroupIdentifier {GroupId = string.Format("sg-{0}", i)});
				}
				instances.Add(instance);
			}

			Ec2Mock.Setup(x => x.DescribeSecurityGroups(It.IsAny<DescribeSecurityGroupsRequest>()))
			       .Returns((DescribeSecurityGroupsRequest request) => new DescribeSecurityGroupsResponse
			       {
				       SecurityGroups = request.GroupIds.Select(x => new SecurityGroup {GroupId = x}).ToList()
			       });

			Dictionary<string, List<SecurityGroup>> map = SecurityGroupService.GetSecurityGroupMap(instances);
			for (int sgCount = 0; sgCount < 3; sgCount++)
			{
				map[string.Format("i-{0}", sgCount)].Count.Should().Be(sgCount);
			}
		}
	}
}