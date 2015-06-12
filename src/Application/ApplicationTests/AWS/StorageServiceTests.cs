using System.IO;
using System.Linq;
using System.Text;

using Amazon.S3;
using Amazon.S3.Model;

using FluentAssertions;

using Ica.StackIt.Application.AWS;
using Ica.StackIt.Application.Parser;

using Moq;

using NUnit.Framework;

namespace Ica.StackIt.Application.ApplicationTests.AWS
{
	internal class StorageServiceTests
	{
		private Mock<IAmazonS3> S3ClientMock { get; set; }
		private Mock<IS3PathParser> PathParserMock { get; set; }
		private StorageService StorageService { get; set; }

		[SetUp]
		public void SetUp()
		{
			S3ClientMock = new Mock<IAmazonS3>();
			PathParserMock = new Mock<IS3PathParser>();
			StorageService = new StorageService(S3ClientMock.Object, PathParserMock.Object);
		}

		[Test]
		public void UploadFile_Ok()
		{
			// Arrange
			const string bucket = "files";
			const string key = "tax_return.pdf";

			// Act
			string result = StorageService.UploadFile(bucket, key, new byte[] {1, 2, 3});

			// Assert
			S3ClientMock.Verify(x => x.PutObject(It.Is<PutObjectRequest>(req =>
				req.BucketName == bucket &&
				req.Key == key &&
				req.ServerSideEncryptionMethod == ServerSideEncryptionMethod.AES256
				)));
			result.Should().Be("http://s3.amazonaws.com/files/tax_return.pdf");
		}

		[Test]
		public void GetFile_Ok()
		{
			// Arrange
			const string path = "s3://files/tax_return.pdf";
			PathParserMock.Setup(x => x.Parse(path)).Returns(new S3PathParts("files", "tax_return.pdf"));

			S3ClientMock.Setup(x => x.GetObject("files", "tax_return.pdf")).Returns(new GetObjectResponse
			{
				ResponseStream = new MemoryStream(Encoding.UTF8.GetBytes("oneMillionDollars"))
			});

			// Act
			string fileContents = StorageService.GetFile(path);

			// Assert
			fileContents.Should().Be("oneMillionDollars");
		}

		[Test]
		public void CreateExpirationRule_Ok()
		{
			// Arrange

			// Act
			StorageService.CreateExpirationRule("files", "throwAway", 1, "Files I don't care about");

			// Assert
			S3ClientMock.Verify(x => x.PutLifecycleConfiguration("files", It.Is<LifecycleConfiguration>(conf =>
				conf.Rules.Single().Id == "Files I don't care about")));
		}
	}
}