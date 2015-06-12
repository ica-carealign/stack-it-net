using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;

namespace Ica.StackIt.AspNet.Identity.Crowd
{
	public class UserStore : CrowdStore, IUserStore<IdentityUser>, IUserRoleStore<IdentityUser>
	{
		public UserStore(IAuthenticatedUserClient authenticatedUserClient) : base(authenticatedUserClient) {}

		public Task CreateAsync(IdentityUser user)
		{
			throw new NotImplementedException();
		}

		public Task UpdateAsync(IdentityUser user)
		{
			throw new NotImplementedException();
		}

		public Task DeleteAsync(IdentityUser user)
		{
			throw new NotImplementedException();
		}

		public Task<IdentityUser> FindByIdAsync(string userId)
		{
			// Since we can not get the ID of the user via the Crowd SDK,
			// we'll treat the user name as a "natural" ID
			return FindByNameAsync(userId);
		}

		public Task<IdentityUser> FindByNameAsync(string userName)
		{
			return TaskFactory.StartNew(() => AuthenticatedUserClient.GetUserByName(userName));
		}

		public Task AddToRoleAsync(IdentityUser user, string roleName)
		{
			throw new NotImplementedException();
		}

		public Task RemoveFromRoleAsync(IdentityUser user, string roleName)
		{
			throw new NotImplementedException();
		}

		public Task<IList<string>> GetRolesAsync(IdentityUser user)
		{
			return TaskFactory.StartNew(() => AuthenticatedUserClient.GetRolesByUser(user).ToList() as IList<string>);
		}

		public Task<bool> IsInRoleAsync(IdentityUser user, string roleName)
		{
			return TaskFactory.StartNew(() => AuthenticatedUserClient.GetRolesByUser(user).Contains(roleName));
		}

		public void Dispose()
		{
			// No-op, nothing to dispose
		}
	}
}