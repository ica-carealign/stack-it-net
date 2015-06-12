using System;
using System.Collections.Generic;
using System.Linq;

using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Amazon.CloudFormation;

namespace Ica.StackIt.Interactive.WebPortal.Extensions
{
	public static class HtmlHelperExtensions
	{
		private static readonly Dictionary<string, string> _stackStatusMap = new Dictionary<string, string>
		{
			{StackStatus.CREATE_COMPLETE, "ok-sign text-success"},
			{StackStatus.CREATE_FAILED, "warning-sign text-danger"},
			{StackStatus.CREATE_IN_PROGRESS, "warning-sign text-warning"},
			{StackStatus.DELETE_IN_PROGRESS, "warning-sign text-warning"}
		};

		// http://stackoverflow.com/questions/20410623/how-to-add-active-class-to-html-actionlink-in-asp-net-mvc
		public static string IsSelected(this HtmlHelper html, string controller = null, string action = null)
		{
			const string cssClass = "active";
			string currentAction = (string) html.ViewContext.RouteData.Values["action"];
			string currentController = (string) html.ViewContext.RouteData.Values["controller"];

			if(String.IsNullOrEmpty(controller))
			{
				controller = currentController;
			}

			if(String.IsNullOrEmpty(action))
			{
				action = currentAction;
			}

			return controller == currentController && action == currentAction ?
				cssClass : String.Empty;
		}

		/// <summary>
		///	Returns the name of the current controller and action, lowercased and hyphenated.
		/// Can be placed as classes in HTML tags so pages can be styled in css based on the current action.
		/// </summary>
		public static string ClassesForCurrentContext(this HtmlHelper html)
		{
			var contexts = new[]
			{
				Hyphenate(html.ViewContext.RouteData.Values["controller"] as string),
				Hyphenate(html.ViewContext.RouteData.Values["action"] as string)
			};
			return string.Join(" ", contexts.Where(s => !string.IsNullOrEmpty(s)));
		}

		public static IHtmlString RenderStatusIcon(this HtmlHelper html, string status, Guid stackId)
		{
			var urlHelper = new UrlHelper(html.ViewContext.RequestContext);
			const string spanTemplate = @"<span class=""glyphicon glyphicon-{0} center-block text-center"" title=""{1}""></span>";

			var action = urlHelper.Action("StackEvents", "Home", new RouteValueDictionary{{"id", stackId}});
			var linkTemplate = string.Format(@"<a href=""{0}"">{{0}}</a>", action);

			if (status == null)
			{
				return html.Raw(string.Format(spanTemplate, _stackStatusMap[StackStatus.CREATE_IN_PROGRESS], "CREATE_IN_PROGRESS"));
			}

			var span = string.Format(spanTemplate, _stackStatusMap.ContainsKey(status)
				? _stackStatusMap[status]
				: _stackStatusMap[StackStatus.CREATE_IN_PROGRESS], status);

			var link = string.Format(linkTemplate, span);

			return html.Raw(stackId == default(Guid) ? span : link);
		}

		private static readonly Regex _hyphenationPattern = new Regex(@"(?<=\p{Ll})(?=\p{Lu})");
		private static string Hyphenate(string orig)
		{
			return _hyphenationPattern.Replace(orig ?? string.Empty, "-").ToLowerInvariant();
		}
	}
}