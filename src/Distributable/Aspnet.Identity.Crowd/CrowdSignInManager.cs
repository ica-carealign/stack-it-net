using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace Ica.StackIt.AspNet.Identity.Crowd
{
	public class CrowdSignInManager : SignInManager<IdentityUser, string>
	{
		public CrowdSignInManager(UserManager<IdentityUser, string> userManager, IAuthenticationManager authenticationManager)
			: base(userManager, authenticationManager) {}

		public override Task<ClaimsIdentity> CreateUserIdentityAsync(IdentityUser user)
		{
			return user.GenerateUserIdentityAsync((CrowdUserManager) UserManager);
		}
	}
}