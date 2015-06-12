using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

using FluentAssertions;

using Ica.StackIt.AspNet.Identity.Crowd;
using Ica.StackIt.Interactive.WebPortal;
using Ica.StackIt.Interactive.WebPortal.Controllers;
using Ica.StackIt.Interactive.WebPortal.Models;

using Moq;

using NUnit.Framework;

namespace Ica.StackIt.Interactive.WebPortalTests
{
	public class AccountControllerTests
	{
		private Mock<IAuthenticatedUserClient> UserClientMock { get; set; }
		private Mock<ISignInManager> SignInManagerMock { get; set; }
		private Mock<IUserProfileAccessManager> UserProfileAccessManagerMock { get; set; }
		private AccountController AccountController { get; set; }

		[SetUp]
		public void SetUp()
		{
			UserClientMock = new Mock<IAuthenticatedUserClient>();
			SignInManagerMock = new Mock<ISignInManager>();
			UserProfileAccessManagerMock = new Mock<IUserProfileAccessManager>();
			AccountController = new AccountController(
				UserClientMock.Object,
				SignInManagerMock.Object,
				UserProfileAccessManagerMock.Object
				);
		}

		[Test]
		public void Login_HappyPath()
		{
			// Arrange
			const string username = "Hello";
			const string password = "World";
			var loginModel = new LoginViewModel
			{
				UserName = username,
				Password = password,
				RememberMe = true
			};
			string returnUrl = string.Empty;
			var identityUser = new IdentityUser();
			UserClientMock.Setup(client => client.AuthenticateUser(username, password)).Returns(identityUser);

			// Act
			ActionResult result = AccountController.Login(loginModel, returnUrl).Result;

			// Assert
			SignInManagerMock.Verify(x => x.SignInAsync(identityUser, true, true));
			result.Should().NotBeNull();
		}

		[Test]
		public void Login_CanNotAuthenticateUser()
		{
			// Arrange
			UserClientMock.Setup(x => x.AuthenticateUser(It.IsAny<string>(), It.IsAny<string>())).Returns((IdentityUser) null);

			// Act
			var result = AccountController.Login(new LoginViewModel(), string.Empty).Result;

			// Assert
			AccountController.ModelState[string.Empty].Errors.First().ErrorMessage
				.Should().Be("Login unsuccessful. Check your user name and password.");
			AssertNeverSignedIn();
			result.Should().NotBeNull();
		}

		[Test]
		public void Login_UserClientThrowsException()
		{
			// Arrange
			UserClientMock.Setup(x => x.AuthenticateUser(It.IsAny<string>(), It.IsAny<string>())).Throws<AuthenticatedUserClientException>();

			// Act
			Task<ActionResult> result = AccountController.Login(new LoginViewModel(), string.Empty);

			// Assert
			AccountController.ModelState[string.Empty].Errors.First().ErrorMessage
				.Should().Be("There was a problem. Contact your administrator or try again later.");
			AssertNeverSignedIn();
			result.Should().NotBeNull();
		}

		[Test]
		public async void Logout_HappyPath()
		{
			// Act
			await AccountController.Logout();

			// Assert
			SignInManagerMock.Verify(x => x.SignOutAsync(), Times.Once);
		}

		private void AssertNeverSignedIn()
		{
			SignInManagerMock.Verify(x => x.SignInAsync(It.IsAny<IdentityUser>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
		}
	}
}