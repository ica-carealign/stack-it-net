using System;
using System.Collections.Generic;

using Ica.StackIt.Application.AWS;
using Ica.StackIt.Application.Command;
using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;

using Moq;

using NUnit.Framework;

namespace Ica.StackIt.Application.ApplicationTests.Command
{
	internal class CreateIpRangeTests
	{
		private Mock<IRepository<IPRange>> IPRangeRepositoryMock { get; set; }
		private Mock<IRepository<AwsProfile>> ProfileRepositoryMock { get; set; }
		private Mock<IAwsClientFactory> ClientFactoryMock { get; set; }
		private Mock<INetworkService> NetworkServiceMock { get; set; }
		private Mock<IAwsClient> AwsClientMock { get; set; }
		private CreateIpRange Command { get; set; }
		private const string _subnetId = "SubnetOne";
		private Guid _profileId;

		[SetUp]
		public void SetUp()
		{
			IPRangeRepositoryMock = new Mock<IRepository<IPRange>>();
			ProfileRepositoryMock = new Mock<IRepository<AwsProfile>>();
			ClientFactoryMock = new Mock<IAwsClientFactory>();
			Command = new CreateIpRange(IPRangeRepositoryMock.Object, ProfileRepositoryMock.Object, ClientFactoryMock.Object);

			_profileId = Guid.NewGuid();
			var profile = new AwsProfile
			{
				Id = _profileId
			};
			ProfileRepositoryMock.Setup(x => x.Find(_profileId)).Returns(profile);

			AwsClientMock = new Mock<IAwsClient>();
			NetworkServiceMock = new Mock<INetworkService>();

			AwsClientMock.Setup(x => x.NetworkService).Returns(NetworkServiceMock.Object);
			ClientFactoryMock.Setup(x => x.GetClient(profile)).Returns(AwsClientMock.Object);
		}

		[Test]
		public void Execute_EndEarlyIfIPRangeAlreadyExists()
		{
			// Arrange
			const string cidr = "10.50.50.10/24";
			NetworkServiceMock.Setup(x => x.GetCidrBySubnetId(_subnetId)).Returns(cidr);
			IPRangeRepositoryMock.Setup(x => x.FindAll()).Returns(new List<IPRange> {new IPRange {Cidr = cidr}});

			// Act
			Command.Execute(_profileId, _subnetId);

			// Assert
			IPRangeRepositoryMock.Verify(x => x.Add(It.IsAny<IPRange>()), Times.Never);
		}

		[Test]
		public void Execute_Ok()
		{
			// Arrange
			const string cidr = "255.255.255.10/24";
			NetworkServiceMock.Setup(x => x.GetCidrBySubnetId(_subnetId)).Returns(cidr);
			IPRangeRepositoryMock.Setup(x => x.FindAll()).Returns(new List<IPRange>());

			// Act
			Command.Execute(_profileId, _subnetId);

			// Assert
			IPRangeRepositoryMock.Verify(x => x.Add(It.Is<IPRange>(range =>
				range.Addresses.ContainsKey("255.255.255.6") &&
				range.Addresses.ContainsKey("255.255.255.254") &&
				!range.Addresses.ContainsKey("255.255.255.255") &&
				range.Addresses.Count == 250)), Times.Once);
		}
	}
}