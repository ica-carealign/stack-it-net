using System.Linq;

using Amazon;
using Amazon.Runtime;
using Amazon.S3;

using FluentAssertions;

using Ica.StackIt.Application.AWS;
using Ica.StackIt.Application.Parser;

using NUnit.Framework;

namespace Ica.StackIt.Testing.IntegrationTests
{
	[Ignore("Not intended as an automated test. Use for manual testing.")]
	internal class StorageServiceTests
	{
		private const string _bucket = "nelson-test-creds-bucket";
		private const string _testTemplates = "test-templates/";
		private const string _testLifecycleRule = "Test lifecycle rule";

		private IAmazonS3 S3Client { get; set; }
		private StorageService StorageService { get; set; }

		[SetUp]
		public void SetUp()
		{
			AWSCredentials credentials = AmbientCredentials.GetCredentials();
			S3Client = AWSClientFactory.CreateAmazonS3Client(credentials);
			StorageService = new StorageService(S3Client, new S3PathParser());
		}

		[TearDown]
		public void TearDown()
		{
			S3Client.DeleteLifecycleConfiguration(_bucket);
		}

		[Test]
		public void CanCreateLifecycleRule()
		{
			// Arrange

			// Act
			StorageService.CreateExpirationRule(_bucket, _testTemplates, 1, _testLifecycleRule);

			// Assert
			var response = S3Client.GetLifecycleConfiguration(_bucket);
			response.Configuration.Rules.Select(x => x.Id).Single().Should().NotBeNull();
		}
	}
}