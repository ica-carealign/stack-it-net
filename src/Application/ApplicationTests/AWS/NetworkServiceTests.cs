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
	internal class NetworkServiceTests
	{
		private Mock<IAmazonEC2> Ec2ClientMock { get; set; }
		private NetworkService NetworkService { get; set; }

		[SetUp]
		public void SetUp()
		{
			Ec2ClientMock = new Mock<IAmazonEC2>();
			NetworkService = new NetworkService(Ec2ClientMock.Object);
		}

		[Test]
		public void GetVpcs_Ok()
		{
			// Arrange
			var vpcs = new List<Vpc> {new Vpc()};
			var response = new DescribeVpcsResponse { Vpcs = vpcs };
			Ec2ClientMock.Setup(x => x.DescribeVpcs(It.IsAny<DescribeVpcsRequest>())).Returns(response);

			// Act
			var actualVpcs = NetworkService.GetVpcs();

			// Assert
			actualVpcs.Should().BeSameAs(vpcs);
		}

		[Test]
		public void GetSubnetsByVpcId_Ok()
		{
			// Arrange
			const string vpcId = "VpcOne";
			var subnetInVpcOne = new Subnet {VpcId = vpcId};
			var subnetInVpcTwo = new Subnet {VpcId = "VpcTwo"};
			var subnets = new List<Subnet>
			{
				subnetInVpcOne,
				subnetInVpcTwo
			};
			var response = new DescribeSubnetsResponse {Subnets = subnets};
			Ec2ClientMock.Setup(x => x.DescribeSubnets(It.IsAny<DescribeSubnetsRequest>())).Returns(response);

			// Act
			var actualSubnets = NetworkService.GetSubnets(vpcId);

			// Assert
			actualSubnets.Single().Should().BeSameAs(subnetInVpcOne);
		}

		[Test]
		public void GetCidrBySubnet_Ok()
		{
			// Arrange
			const string subnetId = "SubnetOne";
			const string cidr = "192.0.0.0/24";
			var response = new DescribeSubnetsResponse {Subnets = new List<Subnet> {new Subnet {CidrBlock = "192.0.0.0/24"}}};
			Ec2ClientMock.Setup(x => x.DescribeSubnets(It.Is<DescribeSubnetsRequest>(req => req.SubnetIds.Contains("SubnetOne")))).Returns(response);

			// Act
			var results = NetworkService.GetCidrBySubnetId(subnetId);

			// Assert
			results.Should().Be(cidr);
		}

		[Test]
		public void GetAllocatedIpAddresses_Ok()
		{
			// Arrange
			var response = new DescribeInstancesResponse
			{
				Reservations = new List<Reservation>
				{
					new Reservation
					{
						Instances = new List<Instance>
						{
							new Instance {PrivateIpAddress = "192.168.10.10"},
							new Instance {PrivateIpAddress = "192.168.10.11"}
						}
					},
					new Reservation
					{
						Instances = new List<Instance>
						{
							new Instance {PrivateIpAddress = "192.168.10.12"},
							new Instance {PrivateIpAddress = "192.168.10.13"}
						}
					}
				}
			};
			Ec2ClientMock.Setup(x => x.DescribeInstances(It.IsAny<DescribeInstancesRequest>())).Returns(response);
			var expected = new List<string> {"192.168.10.10", "192.168.10.11", "192.168.10.12", "192.168.10.13"};

			// Act
			var actual = NetworkService.GetAllocatedIpAddresses();

			// Assert
			actual.Should().BeEquivalentTo(expected);
		}
	}
}