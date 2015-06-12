using System.Threading.Tasks;

namespace Ica.StackIt.AspNet.Identity.Crowd
{
	/// <summary>
	/// Provides sign in and sign out.
	/// </summary>
	public interface ISignInManager
	{
		Task SignInAsync(IdentityUser user, bool isPersistent, bool rememberBrowser);

		Task SignOutAsync();
	}
}