using Ica.StackIt.AspNet.Identity.Crowd;
using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;
using Ica.StackIt.Interactive.WebPortal;

using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentAssertions;

namespace Ica.StackIt.Interactive.WebPortalTests
{
	class UserProfileAccessManagerTests
	{
		Mock<IAuthenticatedUserClient> _authenticatedUserClientMock;
		Mock<IRepository<AwsProfile>> _awsProfileRepositoryMock;

		UserProfileAccessManager _userProfileAccessManger;

		[SetUp]
		public void SetUp()
		{
			_authenticatedUserClientMock = new Mock<IAuthenticatedUserClient>();
			_awsProfileRepositoryMock = new Mock<IRepository<AwsProfile>>();

			_userProfileAccessManger = new UserProfileAccessManager(
				_authenticatedUserClientMock.Object,
				_awsProfileRepositoryMock.Object
			);
		}

		[Test]
		public void SingleUserSingleProfile_OK()
		{
			var user = new IdentityUser();
			user.UserName = "test";

			//Arrange
			_authenticatedUserClientMock.Setup(x => x.GetRolesByUser(user)).Returns(new List<string> { "Admin" });
			_awsProfileRepositoryMock.Setup(x => x.FindAll()).Returns(new List<AwsProfile> {
				new AwsProfile{
					Groups = new List<string> { "Admin" }
				}
			});

			_authenticatedUserClientMock.Setup(x => x.GetUserByName(user.UserName)).Returns(user);

			//Act
			var actual = _userProfileAccessManger.GetProfilesForUser(user.UserName);

			//Assert
			var expected = new List<AwsProfile> { new AwsProfile { Groups = new List<string> { "Admin" } } };

			actual.ShouldBeEquivalentTo(expected);
		}

		[Test]
		public void SingleUserMultiProfile_OK()
		{
			var user = new IdentityUser();
			user.UserName = "test";

			//Arrange
			_authenticatedUserClientMock.Setup(x => x.GetRolesByUser(user)).Returns(new List<string> { "Admin" });
			_awsProfileRepositoryMock.Setup(x => x.FindAll()).Returns(new List<AwsProfile> {
				new AwsProfile{
					Name = "Group1",
					Groups = new List<string> { "Admin" }
				},
				new AwsProfile{
					Name = "Group2",
					Groups = new List<string> { "Admin" }
				},
			});

			_authenticatedUserClientMock.Setup(x => x.GetUserByName(user.UserName)).Returns(user);

			//Act
			var actual = _userProfileAccessManger.GetProfilesForUser(user.UserName);

			//Assert
			var expected = new List<AwsProfile>
			{
				new AwsProfile
				{
					Name = "Group1",
					Groups = new List<string> { "Admin" }
				},
				new AwsProfile
				{
					Name = "Group2",
					Groups = new List<string> { "Admin" }
				}
			};

			actual.ShouldBeEquivalentTo(expected);
		}

		[Test]
		public void SingleUserMultipleGroupsMultipleProfiles_OK()
		{
			var user = new IdentityUser();
			user.UserName = "test";

			//Arrange
			_authenticatedUserClientMock.Setup(x => x.GetRolesByUser(user)).Returns(
				new List<string>
				{
					"Admin",
					"Developers"
				}
			);

			_awsProfileRepositoryMock.Setup(x => x.FindAll()).Returns(new List<AwsProfile> {
				new AwsProfile{
					Name = "Group1",
					Groups = new List<string> { "Admin", "Developers" }
				},
				new AwsProfile{
					Name = "Group2",
					Groups = new List<string> { "Admin" }
				},
				new AwsProfile{
					Name = "Group3",
					Groups = new List<string> { "Developers" }
				}
			});

			_authenticatedUserClientMock.Setup(x => x.GetUserByName(user.UserName)).Returns(user);

			//Act
			var actual = _userProfileAccessManger.GetProfilesForUser(user.UserName);

			//Assert
			var expected = new List<AwsProfile>
			{
				new AwsProfile{
					Name = "Group1",
					Groups = new List<string> { "Admin", "Developers" }
				},
				new AwsProfile{
					Name = "Group2",
					Groups = new List<string> { "Admin" }
				},
				new AwsProfile{
					Name = "Group3",
					Groups = new List<string> { "Developers" }
				}
			};

			actual.ShouldBeEquivalentTo(expected);
		}
	}
}
