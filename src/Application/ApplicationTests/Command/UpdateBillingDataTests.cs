using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using FluentAssertions;

using Ica.StackIt.Application.ApplicationTests.Properties;
using Ica.StackIt.Application.AWS;
using Ica.StackIt.Application.Billing;
using Ica.StackIt.Application.Command;
using Ica.StackIt.Application.Parser;
using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;

using Moq;

using NUnit.Framework;

namespace Ica.StackIt.Application.ApplicationTests.Command
{
	public class UpdateBillingDataTests
	{
		[SetUp]
		public void SetUp()
		{
			AwsProfileRepositoryMock = new Mock<IRepository<AwsProfile>>();
			BillingManagerMock = new Mock<IBillingManager>();

			AwsClientFactoryMock = new Mock<IAwsClientFactory>();
			AwsClientMock = new Mock<IAwsClient>();

			ClockMock = new Mock<IClock>();
		}

		private Mock<IAwsClient> AwsClientMock { get; set; }

		private Mock<IRepository<AwsProfile>> AwsProfileRepositoryMock { get; set; }

		private IRepository<AwsProfile> AwsProfileRepository
		{
			get { return AwsProfileRepositoryMock.Object; }
		}

		private Mock<IAwsClientFactory> AwsClientFactoryMock { get; set; }

		private IAwsClientFactory AwsClientFactory
		{
			get { return AwsClientFactoryMock.Object; }
		}

		private Mock<IBillingManager> BillingManagerMock { get; set; }

		private IBillingManager BillingManager
		{
			get { return BillingManagerMock.Object; }
		}

		private Mock<IClock> ClockMock { get; set; }

		private IClock Clock
		{
			get { return ClockMock.Object; }
		}

		[Test]
		public void SkipsMissingProfile()
		{
			// Arrange
			Guid profileId = Guid.NewGuid();
			AwsProfileRepositoryMock.Setup(x => x.Find(profileId)).Returns((AwsProfile) null);
			var command = new UpdateBillingData(AwsClientFactory, AwsProfileRepository, BillingManager, Clock, new S3PathParser());

			// Act
			command.LoadDeltas(profileId);

			// Assert
			AwsClientFactoryMock.Verify(x => x.GetClient(It.IsAny<AwsProfile>()), Times.Never);
		}

		[Test]
		public void SkipsMissingS3Bucket()
		{
			// Arrange
			Guid profileId = Guid.NewGuid();
			var profile = new AwsProfile {Id = profileId, DetailedBillingS3Bucket = null};
			AwsProfileRepositoryMock.Setup(x => x.Find(profileId)).Returns(profile);
			var command = new UpdateBillingData(AwsClientFactory, AwsProfileRepository, BillingManager, Clock, new S3PathParser());

			// Act
			command.LoadDeltas(profileId);

			// Assert
			AwsProfileRepositoryMock.Verify(x => x.Update(It.IsAny<AwsProfile>()), Times.Never);
		}

		[Test]
		public void SkipsProfilesWithoutBillingHistory()
		{
			// Arrange
			Guid profileId = Guid.NewGuid();
			var profile = new AwsProfile {Id = profileId, DetailedBillingS3Bucket = "my-bucket", IsBillingHistoryLoaded = false};
			AwsProfileRepositoryMock.Setup(x => x.Find(profileId)).Returns(profile);
			var command = new UpdateBillingData(AwsClientFactory, AwsProfileRepository, BillingManager, Clock, new S3PathParser());

			// Act
			command.LoadDeltas(profileId);

			// Assert
			AwsProfileRepositoryMock.Verify(x => x.Update(It.IsAny<AwsProfile>()), Times.Never);
		}

		[Test]
		public void QueriesCorrectS3Object()
		{
			// Arrange
			Guid profileId = Guid.NewGuid();
			var lastModified = new DateTime(2014, 6, 14, 13, 12, 11, DateTimeKind.Utc);
			var profile = new AwsProfile
			{
				Id = profileId,
				DetailedBillingS3Bucket = "my-bucket",
				Account = "12345",
				IsBillingHistoryLoaded = true,
				BillingMetadata = { {"2014-06", new BillingMetadata { LastModified = lastModified}}}
			};
			AwsProfileRepositoryMock.Setup(x => x.Find(profileId)).Returns(profile);
			AwsClientFactoryMock.Setup(x => x.GetClient(profile)).Returns(AwsClientMock.Object);

			var storage = new TestStorageService();
			AwsClientMock.Setup(x => x.StorageService).Returns(storage);

			var now = new DateTime(2014, 6, 14, 16, 15, 14, DateTimeKind.Utc);
			ClockMock.Setup(x => x.UtcNow).Returns(now);

			var command = new UpdateBillingData(AwsClientFactory, AwsProfileRepository, BillingManager, Clock, new S3PathParser());

			// Act
			command.LoadDeltas(profileId);

			// Assert
			string expectedUrl = UpdateBillingData.MonthlyCsvUrl.Create("my-bucket", "12345", "2014-06");
			storage.S3Url.Should().Be(expectedUrl);
			storage.LastModified.Should().Be(lastModified);
			storage.CallCount.Should().Be(1);
		}

		[Test]
		public void LoadsCsv()
		{
			// Arrange
			Guid profileId = Guid.NewGuid();
			var profile = new AwsProfile
			{
				Id = profileId,
				DetailedBillingS3Bucket = "my-bucket",
				IsBillingHistoryLoaded = true,
				Account = "12345",
			};
			AwsProfileRepositoryMock.Setup(x => x.Find(profileId)).Returns(profile);
			AwsClientFactoryMock.Setup(x => x.GetClient(profile)).Returns(AwsClientMock.Object);

			var storage = new TestStorageService();
			AwsClientMock.Setup(x => x.StorageService).Returns(storage);
			storage.Contents = new MemoryStream(Resources.LineItemsZip);

			var now = new DateTime(2014, 6, 14, 16, 15, 14, DateTimeKind.Utc);
			ClockMock.Setup(x => x.UtcNow).Returns(now);

			var command = new UpdateBillingData(AwsClientFactory, AwsProfileRepository, BillingManager, Clock, new S3PathParser());

			string loadedPeriod = null;
			LineItem lineItem = null;
			BillingManagerMock.Setup(x => x.LoadLineItems(It.IsAny<IEnumerable<LineItem>>(), It.IsAny<string>()))
			                 .Callback((IEnumerable<LineItem> lineItems, string period) =>
			                 {
				                 lineItem = lineItems.FirstOrDefault();
				                 loadedPeriod = period;
			                 });

			// Act
			command.LoadDeltas(profileId);

			// Assert
			loadedPeriod.Should().Be("2014-06");
			lineItem.RecordId.Should().Be("31861622192480759163092020", "should match first line item from line-items.csv embedded resource");
		}

		[Test]
		public void SetsLastModified()
		{
			// Arrange
			Guid profileId = Guid.NewGuid();
			var profile = new AwsProfile
			{
				Id = profileId,
				DetailedBillingS3Bucket = "my-bucket",
				IsBillingHistoryLoaded = true,
				Account = "12345",
			};
			AwsProfileRepositoryMock.Setup(x => x.Find(profileId)).Returns(profile);
			AwsClientFactoryMock.Setup(x => x.GetClient(profile)).Returns(AwsClientMock.Object);

			var now = new DateTime(2014, 6, 14, 16, 15, 14, DateTimeKind.Utc);
			ClockMock.Setup(x => x.UtcNow).Returns(now);

			var storage = new TestStorageService();
			AwsClientMock.Setup(x => x.StorageService).Returns(storage);
			storage.NewLastModified = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);
			storage.Contents = new MemoryStream(Resources.LineItemsZip);

			var command = new UpdateBillingData(AwsClientFactory, AwsProfileRepository, BillingManager, Clock, new S3PathParser());

			// Act
			command.LoadDeltas(profileId);

			// Assert
			AwsProfileRepositoryMock.Verify(
				x => x.Update(It.Is((AwsProfile p) =>
					p.Id == profileId &&
					p.BillingMetadata["2014-06"].LastModified == storage.NewLastModified)
				));
		}

		private class TestStorageService : IStorageService
		{
			public string S3Url { get; set; }
			public DateTime LastModified { get; set; }
			public DateTime NewLastModified { get; set; }
			public int CallCount { get; private set; }
			public MemoryStream Contents { get; set; }

			public TestStorageService()
			{
				CallCount = 0;
			}

			public Stream GetFileIfChangedSince(string s3Url, DateTime lastModified, out DateTime newLastModified)
			{
				CallCount++;
				S3Url = s3Url;
				LastModified = lastModified;
				newLastModified = NewLastModified;
				return Contents;
			}

			public string UploadFile(string bucket, string key, byte[] contents)
			{
				throw new NotImplementedException();
			}

			public string GetFile(string path)
			{
				throw new NotImplementedException();
			}

			public IList<string> ListFiles(string path)
			{
				throw new NotImplementedException();
			}

			public void CreateExpirationRule(string bucket, string prefix, int expirationDays, string description)
			{
				throw new NotImplementedException();
			}
		}
	}
}