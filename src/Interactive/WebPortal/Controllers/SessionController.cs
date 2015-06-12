using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using Ica.StackIt.Core.Entities;
using Ica.StackIt.Interactive.WebPortal.Models;

using Microsoft.Owin;

namespace Ica.StackIt.Interactive.WebPortal.Controllers
{
	public class SessionController : Controller
	{
		private readonly IOwinContext _owinContext;
		private readonly IUserProfileAccessManager _userProfileAccessManager;

		public SessionController(
			IOwinContext owinContext,
			IUserProfileAccessManager userProfileAccessManager)
		{
			_owinContext = owinContext;
			_userProfileAccessManager = userProfileAccessManager;
		}

		public ActionResult ProfileChooser()
		{
			string currentUsername = _owinContext.Authentication.User.Identity.Name;
			List<AwsProfile> profiles = _userProfileAccessManager.GetProfilesForUser(currentUsername).ToList();

			var model = new SessionViewModel
			{
				Profiles = profiles,
				SelectedProfileId = (Guid) Session[WebPortalConstants.ActiveProfileId]
			};

			return PartialView(model);
		}

		[HttpPost]
		public ActionResult SetActiveProfile(SessionViewModel model)
		{
			if (model.SelectedProfileId != default(Guid))
			{
				Session[WebPortalConstants.ActiveProfileId] = model.SelectedProfileId;
			}

			return RedirectToAction("Index", "Home");
		}
	}
}