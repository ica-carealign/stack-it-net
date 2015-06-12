using Microsoft.AspNet.Identity;

namespace Ica.StackIt.AspNet.Identity.Crowd
{
	public class CrowdUserManager : UserManager<IdentityUser>
	{
		public CrowdUserManager(IUserStore<IdentityUser> store)
			: base(store) {}
	}
}