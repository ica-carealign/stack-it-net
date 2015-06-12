using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using Ica.StackIt.Application.AWS;
using Ica.StackIt.Application.Command;
using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;

using Moq;

using NUnit.Framework;

namespace Ica.StackIt.Application.ApplicationTests.Command
{
	internal class RefreshIpRangesTests
	{
		private Mock<IRepository<IPRange>> IPRangeRepositoryMock { get; set; }
		private Mock<IRepository<AwsProfile>> ProfileRepositoryMock { get; set; }
		private Mock<IAwsClientFactory> ClientFactoryMock { get; set; }
		private Mock<INetworkService> NetworkServiceMock { get; set; }
		private Mock<IAwsClient> AwsClientMock { get; set; }
		private RefreshIpRanges Command { get; set; }
		private Guid _profileId;
		private List<string> _ipsInRange;

		[SetUp]
		public void SetUp()
		{
			IPRangeRepositoryMock = new Mock<IRepository<IPRange>>();
			ProfileRepositoryMock = new Mock<IRepository<AwsProfile>>();
			ClientFactoryMock = new Mock<IAwsClientFactory>();
			Command = new RefreshIpRanges(ProfileRepositoryMock.Object, IPRangeRepositoryMock.Object, ClientFactoryMock.Object);

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

			_ipsInRange = Enumerable.Range(8, 4)
			                        .Select(x => string.Format("255.255.255.{0}", x)).ToList();
			var allocatedIps = _ipsInRange.Concat(new List<string> { "192.168.1.1" });
			NetworkServiceMock.Setup(x => x.GetAllocatedIpAddresses()).Returns(allocatedIps);
		}

		[Test]
		public void Execute_Ok()
		{
			// Arrange
			var ipRange = new IPRange
			{
				AwsProfileId = _profileId,
				Cidr = "255.255.255.10/30",
				Addresses = _ipsInRange.ToDictionary(x => x, y => new SubnetIpAddress {IsInUse = false, Address = IPAddress.Parse(y)})
			};
			IPRangeRepositoryMock.Setup(x => x.FindAll()).Returns(new List<IPRange> {ipRange});

			// Act
			Command.Execute(_profileId);

			// Assert
			IPRangeRepositoryMock.Verify(x => x.Update(ipRange), Times.Once);
		}

		[Test]
		public void OnlyUpdateIfDocumentChanges_TrueInDatabase()
		{
			// Arrange
			var ipRange = new IPRange
			{
				AwsProfileId = _profileId,
				Cidr = "255.255.255.10/30",
				Addresses = new Dictionary<string, SubnetIpAddress>
				{
					{"255.255.255.8", new SubnetIpAddress {Address = IPAddress.Parse("255.255.255.8"), IsInUse = true}}
				}
			};
			IPRangeRepositoryMock.Setup(x => x.FindAll()).Returns(new List<IPRange> { ipRange });

			// Act
			Command.Execute(_profileId);

			// Assert
			IPRangeRepositoryMock.Verify(x => x.Update(ipRange), Times.Never);
		}

		[Test]
		public void OnlyUpdateIfDocumentChanges_FalseInDatabase()
		{
			// Arrange
			var notAllocated = _ipsInRange.Single(x => x == "255.255.255.8");
			_ipsInRange.Remove(notAllocated);
			var ipRange = new IPRange
			{
				AwsProfileId = _profileId,
				Cidr = "255.255.255.10/30",
				Addresses = new Dictionary<string, SubnetIpAddress>
				{
					{"255.255.255.8", new SubnetIpAddress {Address = IPAddress.Parse("255.255.255.8"), IsInUse = false}}
				}
			};
			IPRangeRepositoryMock.Setup(x => x.FindAll()).Returns(new List<IPRange> { ipRange });

			// Act
			Command.Execute(_profileId);

			// Assert
			IPRangeRepositoryMock.Verify(x => x.Update(ipRange), Times.Never);
		}
	}
}