using System.Collections.Generic;

namespace Ica.StackIt.AspNet.Identity.Crowd
{
	/// <summary>
	/// Provides for high level interaction with a role based user store
	/// </summary>
	public interface IAuthenticatedUserClient
	{
		/// <summary>
		/// Authenticate a user by username and password
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		IdentityUser AuthenticateUser(string username, string password);

		/// <summary>
		/// Get a user by username
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		IdentityUser GetUserByName(string name);

		/// <summary>
		/// Get the user's AD groups
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		IEnumerable<string> GetRolesByUser(IdentityUser user);

		/// <summary>
		/// Get a role (AD group) by name
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		IdentityRole GetRoleByName(string name);

		/// <summary>
		/// Get a list of all roles
		/// </summary>
		/// <returns></returns>
		IEnumerable<string> GetAllRoles();
	}
}