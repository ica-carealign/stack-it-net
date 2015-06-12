using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Services.Protocols;

namespace Ica.StackIt.AspNet.Identity.Crowd
{
	public class CrowdAuthenticatedUserClient : IAuthenticatedUserClient
	{
		private readonly string _apiPassword;
		private readonly string _applicationName;
		private readonly ISecurityServer _securityServer;

		public CrowdAuthenticatedUserClient(ISecurityServer securityServer, string applicationName, string apiPassword)
		{
			_securityServer = securityServer;
			_applicationName = applicationName;
			_apiPassword = apiPassword;
		}

		public IdentityUser AuthenticateUser(string username, string password)
		{
			AuthenticatedToken apiToken = GetAuthenticatedToken();
			var principalPasswordCredentials = new PasswordCredential {credential = password};
			var principalContext = new UserAuthenticationContext {application = _applicationName, name = username, credential = principalPasswordCredentials};

			try
			{
				_securityServer.authenticatePrincipal(apiToken, principalContext);
			}
			catch (SoapException)
			{
				// SoapException means the user could not be authenticated.
				return null;
			}

			return new IdentityUser {UserName = username};
		}

		public IdentityUser GetUserByName(string name)
		{
			AuthenticatedToken apiToken = GetAuthenticatedToken();
			SOAPPrincipal soapPrincipal = _securityServer.findPrincipalByName(apiToken, name);
			return soapPrincipal == null ? null : new IdentityUser {UserName = name};
		}

		public IEnumerable<string> GetRolesByUser(IdentityUser user)
		{
			AuthenticatedToken apiToken = GetAuthenticatedToken();

			SOAPGroup[] groups = _securityServer.searchGroups(apiToken, new SearchRestriction[0]);

			// Until I find a better way
			return groups.Where(x => x.members.Contains(user.UserName)).Select(x => x.name).ToList();
		}

		public IdentityRole GetRoleByName(string name)
		{
			var apiToken = GetAuthenticatedToken();
			var group = _securityServer.findGroupByName(apiToken, name);
			return group == null ? null : new IdentityRole {Name = name};
		}

        public IEnumerable<string> GetAllRoles()
        {
            AuthenticatedToken apiToken = GetAuthenticatedToken();

            SOAPGroup[] groups = _securityServer.searchGroups(apiToken, new SearchRestriction[0]);

            return groups.Select(x => x.name);
        }

		private AuthenticatedToken GetAuthenticatedToken()
		{
			var passwordCredentials = new PasswordCredential {credential = _apiPassword};
			var appContext = new ApplicationAuthenticationContext
			{
				name = _applicationName,
				credential = passwordCredentials
			};

			try
			{
				return _securityServer.authenticateApplication(appContext);
			}
			catch (WebException e)
			{
				throw new AuthenticatedUserClientException("Unable to connect to the Crowd API.", e);
			}
			catch (SoapException e)
			{
				throw new AuthenticatedUserClientException("Unable to authenticate with the Crowd API.", e);
			}
		}
	}
}