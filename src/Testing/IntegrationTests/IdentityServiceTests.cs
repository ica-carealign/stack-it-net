using Amazon;
using Amazon.IdentityManagement;

using FluentAssertions;

using Ica.StackIt.Application.AWS;
using Ica.StackIt.Application.Parser;

using NUnit.Framework;

namespace Ica.StackIt.Testing.IntegrationTests
{
	[Ignore("Not intended as an automated test. Use for manual testing.")]
	class IdentityServiceTests
	{
		public IAmazonIdentityManagementService IamClient { get; set; }
		public IdentityService IdentityService { get; set; }

		[SetUp]
		public void SetUp()
		{
			var credentials = AmbientCredentials.GetCredentials();
			IamClient = AWSClientFactory.CreateAmazonIdentityManagementServiceClient(credentials);

			IdentityService = new IdentityService(IamClient, new ArnParser());
		}

		[Test]
		public void GetUser_Ok()
		{
			// Arrange
			
			// Act
			var user = IdentityService.GetCurrentUser();

			// Assert
			user.Should().NotBeNull();
		}

		[Test]
		public void GetAccount_Ok()
		{
			// Arrange

			// Act
			var account = IdentityService.GetCurrentAccount();

			// Assert
			account.Should().NotBeNullOrEmpty();
		}
	}
}
