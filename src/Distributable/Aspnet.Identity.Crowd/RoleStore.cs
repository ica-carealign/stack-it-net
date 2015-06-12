using System;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;

namespace Ica.StackIt.AspNet.Identity.Crowd
{
	public class RoleStore : CrowdStore, IRoleStore<IdentityRole>
	{
		public RoleStore(IAuthenticatedUserClient securityServer): base(securityServer) {}

		public Task CreateAsync(IdentityRole role)
		{
			throw new NotImplementedException();
		}

		public Task UpdateAsync(IdentityRole role)
		{
			throw new NotImplementedException();
		}

		public Task DeleteAsync(IdentityRole role)
		{
			throw new NotImplementedException();
		}

		public Task<IdentityRole> FindByIdAsync(string roleId)
		{
			return FindByNameAsync(roleId);
		}

		public Task<IdentityRole> FindByNameAsync(string roleName)
		{
			return TaskFactory.StartNew(() => AuthenticatedUserClient.GetRoleByName(roleName));
		}

		public void Dispose()
		{
			// No-op, nothing to dispose
		}
	}
}