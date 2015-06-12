using System.Collections.Generic;

using FluentAssertions;

using Ica.StackIt.AspNet.Identity.Crowd;

using Moq;

using NUnit.Framework;

namespace AspNet.Identity.CrowdTests
{
	internal class UserStoreTests
	{
		private const string _userName = "John";
		private Mock<IAuthenticatedUserClient> AuthenticatedUserClientMock { get; set; }
		private UserStore UserStore { get; set; }

		[SetUp]
		public void SetUp()
		{
			AuthenticatedUserClientMock = new Mock<IAuthenticatedUserClient>();
			UserStore = new UserStore(AuthenticatedUserClientMock.Object);
		}

		[Test]
		public async void FindUserById_Success()
		{
			// Arrange
			IdentityUser expectedUser = SetUpValidUser(_userName, new string[0]);

			// Act
			IdentityUser actual = await UserStore.FindByIdAsync(_userName);

			// Assert
			actual.Should().Be(expectedUser);
		}

		[Test]
		public async void FindUserByName_Success()
		{
			// Arrange
			IdentityUser expected = SetUpValidUser(_userName, new string[0]);

			// Act
			IdentityUser actual = await UserStore.FindByNameAsync(_userName);

			// Assert
			actual.Should().Be(expected);
		}

		[Test]
		public async void GetRoles_Success()
		{
			// Arrange
			var expected = new[] {"admin", "user"};
			IdentityUser user = SetUpValidUser(_userName, expected);

			// Act
			IList<string> actual = await UserStore.GetRolesAsync(user);

			// Assert
			actual.ShouldBeEquivalentTo(expected);
		}

		public async void IsRoleInUser_Yes_Success()
		{
			// Arrange
			const string roleName = "admin";
			IdentityUser user = SetUpValidUser(_userName, new[] {roleName});

			// Act
			bool result = await UserStore.IsInRoleAsync(user, roleName);

			// Assert
			result.Should().BeTrue();
		}

		public async void IsRoleInUser_No_Success()
		{
			// Arrange
			const string roleName = "hoojey";
			IdentityUser user = SetUpValidUser(_userName, new string[0]);

			// Act
			bool result = await UserStore.IsInRoleAsync(user, roleName);

			// Assert
			result.Should().BeFalse();
		}

		private IdentityUser SetUpValidUser(string userName, IEnumerable<string> roles)
		{
			var user = new IdentityUser();
			AuthenticatedUserClientMock.Setup(x => x.GetUserByName(userName)).Returns(user);
			AuthenticatedUserClientMock.Setup(x => x.GetRolesByUser(user)).Returns(roles);
			return user;
		}
	}
}