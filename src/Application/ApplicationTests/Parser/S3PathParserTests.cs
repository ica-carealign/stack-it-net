using FluentAssertions;

using Ica.StackIt.Application.Parser;

using NUnit.Framework;

namespace Ica.StackIt.Application.ApplicationTests.Parser
{
	internal class S3PathParserTests
	{
		private S3PathParser Parser { get; set; }

		[SetUp]
		public void SetUp()
		{
			Parser = new S3PathParser();
		}

		[Test]
		public void CanParseHttpPath_NoFolder()
		{
			// Arrange
			const string path = "https://s3.amazonaws.com/test-bucket/credentials.json";

			// Act
			var results = Parser.Parse(path);

			// Assert
			results.Bucket.Should().Be("test-bucket");
			results.Key.Should().Be("credentials.json");
		}

		[Test]
		public void CanParseHttpPath_NestedResource()
		{
			// Arrange
			const string path = "https://s3.amazonaws.com/test-bucket/test-user/secure/credentials.json";

			// Act
			var results = Parser.Parse(path);

			// Assert
			results.Bucket.Should().Be("test-bucket");
			results.Key.Should().Be("test-user/secure/credentials.json");
		}

		[Test]
		public void CanParseS3Uri_NoFolders()
		{
			// Arrange
			const string uriString = "s3://test-bucket/credentials.json";

			// Act
			var results = Parser.Parse(uriString);

			// Assert
			results.Bucket.Should().Be("test-bucket");
			results.Key.Should().Be("credentials.json");
		}

		[Test]
		public void CanParseS3Uri_NestedResource()
		{
			// Arrange
			const string uriString = "s3://test-bucket/test-user/secure/credentials.json";

			// Act
			var results = Parser.Parse(uriString);

			// Assert
			results.Bucket.Should().Be("test-bucket");
			results.Key.Should().Be("test-user/secure/credentials.json");
		}
	}
}