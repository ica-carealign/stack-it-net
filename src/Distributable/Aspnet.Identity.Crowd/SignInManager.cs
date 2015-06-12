using System.Threading.Tasks;

namespace Ica.StackIt.AspNet.Identity.Crowd
{
	public class SignInManager : ISignInManager
	{
		private readonly CrowdSignInManager _crowdSignInManager;

		public SignInManager(CrowdSignInManager crowdSignInManager)
		{
			_crowdSignInManager = crowdSignInManager;
		}

		public Task SignInAsync(IdentityUser user, bool isPersistent, bool rememberBrowser)
		{
			return _crowdSignInManager.SignInAsync(user, isPersistent, rememberBrowser);
		}

		public Task SignOutAsync()
		{
			return new TaskFactory().StartNew(() => _crowdSignInManager.AuthenticationManager.SignOut());
		}
	}
}