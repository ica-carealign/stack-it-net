using FluentAssertions;

using Ica.StackIt.AspNet.Identity.Crowd;

using Moq;

using NUnit.Framework;

namespace AspNet.Identity.CrowdTests
{
	internal class RoleStoreTests
	{
		private const string _roleName = "admin";
		private Mock<IAuthenticatedUserClient> AuthenticatedUserClientMock { get; set; }
		private RoleStore RoleStore { get; set; }

		[SetUp]
		public void SetUp()
		{
			AuthenticatedUserClientMock = new Mock<IAuthenticatedUserClient>();
			RoleStore = new RoleStore(AuthenticatedUserClientMock.Object);
		}

		[Test]
		public async void FindRoleById_Success()
		{
			// Arrange
			IdentityRole role = SetUpValidRole(_roleName);

			// Act
			IdentityRole actual = await RoleStore.FindByIdAsync(_roleName);

			// Assert
			actual.Should().Be(role);
		}

		[Test]
		public async void FindRoleByName_Success()
		{
			// Arrange
			IdentityRole role = SetUpValidRole(_roleName);

			// Act
			IdentityRole actual = await RoleStore.FindByNameAsync(_roleName);

			// Assert
			actual.Should().Be(role);
		}

		private IdentityRole SetUpValidRole(string roleName)
		{
			var role = new IdentityRole();
			AuthenticatedUserClientMock.Setup(x => x.GetRoleByName(roleName)).Returns(role);
			return role;
		}
	}
}