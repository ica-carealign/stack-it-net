using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Services.Protocols;

using FluentAssertions;

using Ica.StackIt.AspNet.Identity.Crowd;

using NUnit.Framework;

namespace Ica.StackIt.Testing.IntegrationTests
{
	/* These are integration tests built against the internal AD instance.
	 * As integration tests, not only is the CrowdAuthenticatedUserClient tested, but so is the SecurityServer proxy class.
	 *
	 * To run these tests
	 * 1. Remove the [Ignore] attribute on the class
	 * 2. Change the _password field from an empty string to the API endpoint password
	 * 3. Don't forget to change it back before committing
	 */

	[Ignore]
	internal class CrowdAuthenticatedUserClientTests
	{
		private const string _userName = "stackitsa";
		private const string _password = "";
		private const string _roleName = "jira-developers";
		private IAuthenticatedUserClient _crowdAuthenticatedUserClient;

		[SetUp]
		public void SetUp()
		{
			_crowdAuthenticatedUserClient = new CrowdAuthenticatedUserClient(new SecurityServer(), "stackit", _password);
		}

		[Test]
		public void CanNotConnectToCrowdApi_AuthenticateUser()
		{
			SetUpConnectionToAnUnboundPort();

			Action act = () => _crowdAuthenticatedUserClient.AuthenticateUser(_userName, _password);

			ShouldBeUnableToConnect(act);
		}

		[Test]
		public void CanNotConnectToCrowdApi_GetUserByName()
		{
			SetUpConnectionToAnUnboundPort();

			Action act = () => _crowdAuthenticatedUserClient.GetUserByName(_userName);

			ShouldBeUnableToConnect(act);
		}

		[Test]
		public void CanNotConnectToCrowdApi_GetRolesByUser()
		{
			SetUpConnectionToAnUnboundPort();

			Action act = () => _crowdAuthenticatedUserClient.GetRolesByUser(new IdentityUser {UserName = _userName});

			ShouldBeUnableToConnect(act);
		}

		[Test]
		public void CanNotConnectToCrowdApi_GetRoleByName()
		{
			SetUpConnectionToAnUnboundPort();

			Action act = () => _crowdAuthenticatedUserClient.GetRoleByName(_roleName);

			ShouldBeUnableToConnect(act);
		}

		[Test]
		public void CanNotAuthenticateWithCrowdApi_AuthenticateUser()
		{
			SetUpUnauthenticatedConnection();

			Action act = () => _crowdAuthenticatedUserClient.AuthenticateUser(_userName, _password);

			ShouldBeUnableToAuthenticate(act);
		}

		[Test]
		public void CanNotAuthenticateWithCrowdApi_GetUserByName()
		{
			SetUpUnauthenticatedConnection();

			Action act = () => _crowdAuthenticatedUserClient.GetUserByName(_userName);

			ShouldBeUnableToAuthenticate(act);
		}

		[Test]
		public void CanNotAuthenticateWithCrowdApi_GetRolesByUser()
		{
			SetUpUnauthenticatedConnection();

			Action act = () => _crowdAuthenticatedUserClient.GetRolesByUser(new IdentityUser {UserName = _userName});

			ShouldBeUnableToAuthenticate(act);
		}

		[Test]
		public void CanNotAuthenticateWithCrowdApi_GetRoleByName()
		{
			SetUpUnauthenticatedConnection();

			Action act = () => _crowdAuthenticatedUserClient.GetRoleByName(_roleName);

			ShouldBeUnableToAuthenticate(act);
		}

		[Test]
		public void AuthenticateUser_Success()
		{
			var expected = new IdentityUser {UserName = _userName};
			IdentityUser result = _crowdAuthenticatedUserClient.AuthenticateUser(_userName, _password);
			result.ShouldBeEquivalentTo(expected);
		}

		[Test]
		public void GetUserByName_Success()
		{
			var expected = new IdentityUser {UserName = _userName};
			IdentityUser result = _crowdAuthenticatedUserClient.GetUserByName(_userName);
			result.ShouldBeEquivalentTo(expected);
		}

		[Test]
		public void GetRolesByUser_Success()
		{
			var expected = new List<string> {_roleName};
			var user = new IdentityUser {UserName = _userName};
			IEnumerable<string> result = _crowdAuthenticatedUserClient.GetRolesByUser(user);
			result.ShouldBeEquivalentTo(expected);
		}

		[Test]
		public void GetRoleByName_Success()
		{
			var expected = new IdentityRole {Name = _roleName};
			IdentityRole result = _crowdAuthenticatedUserClient.GetRoleByName(_roleName);
			result.ShouldBeEquivalentTo(expected);
		}

		private void SetUpUnauthenticatedConnection()
		{
			_crowdAuthenticatedUserClient = new CrowdAuthenticatedUserClient(new SecurityServer(), _userName, "wrongpassword");
		}

		private void SetUpConnectionToAnUnboundPort()
		{
			_crowdAuthenticatedUserClient = new CrowdAuthenticatedUserClient(
				new SecurityServer {Url = "http://localhost:43552"}, // an unbound port on a known host
				_userName,
				_password
				);
		}

		private static void ShouldBeUnableToConnect(Action act)
		{
			act.ShouldThrow<AuthenticatedUserClientException>().WithMessage("Unable to connect to the Crowd API.")
			   .And.InnerException.Should().BeOfType<WebException>();
		}

		private static void ShouldBeUnableToAuthenticate(Action act)
		{
			act.ShouldThrow<AuthenticatedUserClientException>().WithMessage("Unable to authenticate with the Crowd API.")
			   .And.InnerException.Should().BeOfType<SoapException>();
		}
	}
}
