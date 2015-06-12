using System.Web.Mvc;

namespace Ica.StackIt.Interactive.WebPortal
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}
	}
}
