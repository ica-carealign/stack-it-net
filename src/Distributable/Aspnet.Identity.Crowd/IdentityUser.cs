using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;

namespace Ica.StackIt.AspNet.Identity.Crowd
{
	public class IdentityUser : IUser<string>
	{
		public string Id
		{
			get { return UserName; }
		}

		public string UserName { get; set; }

		public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<IdentityUser> manager)
		{
			// Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
			var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
			return userIdentity;
		}
	}
}