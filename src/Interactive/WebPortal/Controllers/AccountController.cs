using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

using Ica.StackIt.AspNet.Identity.Crowd;
using Ica.StackIt.Interactive.WebPortal.Models;

using Microsoft.Ajax.Utilities;

namespace Ica.StackIt.Interactive.WebPortal.Controllers
{
	public class AccountController : Controller
	{
		private const string _apiExceptionMessage = "There was a problem. Contact your administrator or try again later.";
		private const string _unauthenticatedErrorMessage = "Login unsuccessful. Check your user name and password.";

		private readonly IAuthenticatedUserClient _authenticatedUserClient;
		private readonly ISignInManager _signInManager;
		private readonly IUserProfileAccessManager _userProfileAccessManager;

		public AccountController(
			IAuthenticatedUserClient authenticatedUserClient,
			ISignInManager signInManager,
			IUserProfileAccessManager userProfileAccessManager)
		{
			_authenticatedUserClient = authenticatedUserClient;
			_signInManager = signInManager;
			_userProfileAccessManager = userProfileAccessManager;
		}

		// GET: /Account/Login
		[AllowAnonymous]
		public ActionResult Login(string returnUrl)
		{
			ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		// POST: /Account/Login
		[HttpPost]
		[AllowAnonymous]
		public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			IdentityUser authenticatedUser;
			try
			{
				authenticatedUser = _authenticatedUserClient.AuthenticateUser(model.UserName, model.Password);
			}
			catch (AuthenticatedUserClientException)
			{
				ModelState.AddModelError(string.Empty, _apiExceptionMessage);
				return View(model);
			}

			if (authenticatedUser == null)
			{
				ModelState.AddModelError(string.Empty, _unauthenticatedErrorMessage);
				return View(model);
			}
			await _signInManager.SignInAsync(authenticatedUser, model.RememberMe, rememberBrowser: true);

			var profile = _userProfileAccessManager.GetProfilesForUser(model.UserName).FirstOrDefault();
			if(profile == null)
			{
				// The user doesn't have permission to use *any* profiles
				ModelState.AddModelError("", "You do not have permission to use any AWS profiles. Contact an administrator.");
				return View(model);
			}
			Session[WebPortalConstants.ActiveProfileId] = profile.Id;

			if(!returnUrl.IsNullOrWhiteSpace() && Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			else
			{
				return RedirectToAction("Index", "Home");
			}
		}

		// GET: /Account/Logout
		[AllowAnonymous]
		public async Task<ActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}
	}
}