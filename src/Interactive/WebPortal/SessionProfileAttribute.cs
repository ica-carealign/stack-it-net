using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Ica.StackIt.AspNet.Identity.Crowd;

namespace Ica.StackIt.Interactive.WebPortal
{
	/// <summary>
	///     Specifies that access to a controller or action method is restricted to users who have an active profile in their
	///     session.
	/// </summary>
	public class SessionProfileAttribute : ActionFilterAttribute
	{
		public ISignInManager SignInManager { get; set; }
		public IUserProfileAccessManager UserProfileAccessManager { get; set; }

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var context = HttpContext.Current;
			var user = context.User;

			if (context.User != null && context.Session[WebPortalConstants.ActiveProfileId] == null)
			{
				var potentialProfileId = UserProfileAccessManager.GetProfilesForUser(user.Identity.Name)
				                                                                              .Select(x => x.Id)
				                                                                              .FirstOrDefault();

				if (potentialProfileId != default(Guid))
				{
					context.Session[WebPortalConstants.ActiveProfileId] = potentialProfileId;
				}
				else
				{
					filterContext.Controller.TempData["Error"] = "You do not have permission to use any AWS profiles. Contact an administrator.";
				}
			}

			if (context.Session[WebPortalConstants.ActiveProfileId] == null)
			{
				// Stale session, log the user out and redirect to the log in page
				SignInManager.SignOutAsync().Wait();

				filterContext.Result = new RedirectResult("~/Account/Login");
				return;
			}

			base.OnActionExecuting(filterContext);
		}
	}
}