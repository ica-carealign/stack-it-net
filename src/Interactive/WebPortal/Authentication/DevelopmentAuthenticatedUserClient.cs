using System.Collections.Generic;

using Ica.StackIt.AspNet.Identity.Crowd;

namespace Ica.StackIt.Interactive.WebPortal.Authentication
{
	public class DevelopmentAuthenticatedUserClient : IAuthenticatedUserClient
	{
		public IdentityUser AuthenticateUser(string username, string password)
		{
			return password == username + "-password" ? GetUserByName(username) : null;
		}

		public IdentityUser GetUserByName(string name)
		{
			return new IdentityUser {UserName = name};
		}

		public IEnumerable<string> GetRolesByUser(IdentityUser user)
		{
			yield return "Admin";
			yield return "Developer";
		}

		public IdentityRole GetRoleByName(string name)
		{
			return new IdentityRole {Name = name};
		}

		public IEnumerable<string> GetAllRoles()
		{
			yield return "Admin";
			yield return "Manager";
			yield return "Developer";
			yield return "DevOps";
		}
	}
}