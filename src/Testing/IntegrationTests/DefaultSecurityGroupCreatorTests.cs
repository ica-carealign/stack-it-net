using System.Collections.Generic;
using System.Linq;

using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.Runtime;

using FluentAssertions;

using Ica.StackIt.Application;
using Ica.StackIt.Application.AWS;
using Ica.StackIt.Interactive.WebPortal;

using NUnit.Framework;

namespace Ica.StackIt.Testing.IntegrationTests
{
	[Ignore("Not intended as an automated test. Use for manual testing.")]
	public class DefaultSecurityGroupCreatorTests
	{
		private const string _testSecurityGroupName = "Test_SeeMe_DeleteMe";

		private IAmazonEC2 Ec2Client { get; set; }
		private SecurityGroupService SecurityGroupService { get; set; }
		private DefaultSecurityGroupCreator Creator { get; set; }

		[SetUp]
		public void SetUp()
		{
			AWSCredentials credentials = AmbientCredentials.GetCredentials();
			Ec2Client = AWSClientFactory.CreateAmazonEC2Client(credentials);
			SecurityGroupService = new SecurityGroupService(Ec2Client);

			Creator = new DefaultSecurityGroupCreator(SecurityGroupService) {SecurityGroupName = _testSecurityGroupName};
		}

		[Test]
		public void CreateDefaultSecurityGroup_Ok()
		{
			// Arrange
			string vpcId = Ec2Client.DescribeVpcs(new DescribeVpcsRequest())
			                        .Vpcs.First(vpc => vpc.Tags.First(tag => tag.Key == "Name").Value == "Infrastructure Dev")
			                        .VpcId;

			// .. Verify that the security group doesn't already exist since this test relies on it not existing
			var describeSgs = new DescribeSecurityGroupsRequest {GroupNames = new List<string> {_testSecurityGroupName}};
			try
			{
				Ec2Client.DescribeSecurityGroups(describeSgs);
			}
			catch (AmazonEC2Exception e)
			{
				if (e.Message == "The security group 'Test_SeeMe_DeleteMe' does not exist")
				{
					// This is what we want to happen so let's keep going
				}
				else
				{
					throw;
				}
			}

			// Act
			Assert.AreEqual(Creator.SecurityGroupName, _testSecurityGroupName);
			string sgId = Creator.TryCreateDefaultSecurityGroup(vpcId);

			// Assert
			sgId.Should().NotBeNull();
		}
	}
}