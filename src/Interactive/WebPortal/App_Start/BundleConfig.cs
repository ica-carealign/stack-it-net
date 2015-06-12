using System.Web.Optimization;

namespace Ica.StackIt.Interactive.WebPortal
{
	public class BundleConfig
	{
		// For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
				"~/Scripts/jquery-{version}.js"));

			bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
				"~/Scripts/jquery-ui-{version}.js",
				"~/Scripts/autocomplete-multi-select.js"));

			bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
				"~/Scripts/jquery.validate*"));

			bundles.Add(new ScriptBundle("~/bundles/signalr").Include(
				"~/Scripts/jquery.signalR-{version}.js"));

			// Use the development version of Modernizr to develop with and learn from. Then, when you're
			// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
			bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
				"~/Scripts/modernizr-*"));

			bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
				"~/Scripts/bootstrap.js",
				"~/Scripts/respond.js"));

			bundles.Add(new ScriptBundle("~/bundles/stackit").Include(
				"~/Scripts/html-utils.js",
				"~/Scripts/profile-chooser.js",
				"~/Scripts/stack-power.js",
				"~/Scripts/name-generator.js",
				"~/Scripts/eventBus.js"));

			bundles.Add(new StyleBundle("~/Content/css").Include(
				"~/Content/bootstrap.css",
				"~/Content/site.css",
				"~/Content/crud.css",
				"~/Content/autocomplete-multi-select.css"));

			bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
				"~/Content/themes/base/all.css"));
		}
	}
}