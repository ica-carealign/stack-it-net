using System.Linq;

using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.Runtime;

using FluentAssertions;

using Ica.StackIt.Application;
using Ica.StackIt.Application.AWS;

using NUnit.Framework;

namespace Ica.StackIt.Testing.IntegrationTests
{
	[Ignore("Not intended as an automated test. Use for manual testing.")]
	internal class SecurityGroupServiceTests
	{
		IAmazonEC2 Ec2Client { get; set; }
		public SecurityGroupService Service { get; set; }

		[SetUp]
		public void SetUp()
		{
			AWSCredentials credentials = AmbientCredentials.GetCredentials();
			Ec2Client = AWSClientFactory.CreateAmazonEC2Client(credentials);
			Service = new SecurityGroupService(Ec2Client);
		}

		[Test]
		public void GetSecurityGroupByName()
		{
			// Arrange
			string vpcId = Ec2Client.DescribeVpcs(new DescribeVpcsRequest())
									.Vpcs.First(vpc => vpc.Tags.First(tag => tag.Key == "Name").Value == "Infrastructure Dev")
									.VpcId;
			string securityGroupName = Conventions.DefaultSecurityGroupName;

			// Act
			var securityGroup = Service.GetSecurityGroup(securityGroupName, vpcId);

			// Assert
			securityGroup.Should().NotBeNull();
		}
	}
}