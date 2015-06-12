using System;
using System.Web.Mvc;

namespace Ica.StackIt.Interactive.WebPortal.Extensions
{
	public static class ControllerExtensions
	{
		public static Guid GetActiveProfileId(this Controller self)
		{
			return (Guid) (self.Session[WebPortalConstants.ActiveProfileId]);
		}
	}
}