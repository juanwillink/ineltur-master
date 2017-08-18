using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Collections.Generic;

namespace ArgentinahtlMVC.Models.Services
{
    public static class ImageResultHelper
    {
        public static MvcHtmlString Image<T>(this HtmlHelper helper, Expression<Action<T>> action, string width, string height)
            where T : Controller
        {
            return ImageResultHelper.Image<T>(helper, string.Empty, action, width, height);
        }

        public static MvcHtmlString Image<T>(this HtmlHelper helper, string id, Expression<Action<T>> action, string width, string height)
            where T : Controller
        {
            return ImageResultHelper.Image<T>(helper, id, action, width, height, string.Empty);
        }

        public static MvcHtmlString Image<T>(this HtmlHelper helper, string id, Expression<Action<T>> action, string width, string height, string alt)
            where T : Controller
        {
            //string url = helper.BuildUrlFromExpression<T>(action);
            //return string.Format("<img src=\"{0}\" width=\"{1}\" height=\"{2}\" alt=\"{3}\" />", url, width, height, alt);

            var expression = action.Body as MethodCallExpression;
            string actionMethodName = string.Empty;
            if (expression != null)
            {
                actionMethodName = expression.Method.Name;
            }
            string url = new UrlHelper(helper.ViewContext.RequestContext, helper.RouteCollection).Action(actionMethodName, typeof(T).Name.Remove(typeof(T).Name.IndexOf("Controller"))).ToString();
            var html = MvcHtmlString.Create(string.Format("<img src=\"{0}\" width=\"{1}\" height=\"{2}\" alt=\"{3}\" />", url, width, height, alt));

            if (!string.IsNullOrEmpty(id.Trim()))
                html = MvcHtmlString.Create(string.Format("<img id=\"{0}\" src=\"{1}\" width=\"{2}\" height=\"{3}\" alt=\"{4}\" />", id, url, width, height, alt));

            return html;
        }

        public static MvcHtmlString Image(this HtmlHelper helper, string imagePath, bool inDomain = true)
        {
            return Image(helper, imagePath, string.Empty, inDomain);
        }

        public static MvcHtmlString Image(this HtmlHelper helper, string imagePath, string alt, bool inDomain = true)
        {
            var url = new UrlHelper(helper.ViewContext.RequestContext);

            // build the <img> tag
            var imgBuilder = new TagBuilder("img");
            imgBuilder.MergeAttribute("src", (inDomain ? url.Content(imagePath) : imagePath));
            imgBuilder.MergeAttribute("alt", alt);
            string imgHtml = imgBuilder.ToString(TagRenderMode.SelfClosing);

            return MvcHtmlString.Create(imgHtml);
        }

        public static MvcHtmlString ActionImage(this HtmlHelper helper, string action, string controllerName, string imagePath, string alt, bool imageInDomain = true)
        {
            var url = new UrlHelper(helper.ViewContext.RequestContext);

            string imgHtml = Image(helper, imagePath, imageInDomain).ToHtmlString();

            // build the <a> tag
            var anchorBuilder = new TagBuilder("a");
            anchorBuilder.MergeAttribute("href", url.Action(action, controllerName));
            anchorBuilder.InnerHtml = imgHtml; // include the <img> tag inside
            string anchorHtml = anchorBuilder.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(anchorHtml);
        }
    }

    public static class ButtonResultHelper
    {
        public static MvcHtmlString Button(this HtmlHelper helper, string text)
        {
            return Button(helper, text, null);
        }

        public static MvcHtmlString Button(this HtmlHelper helper, string text, bool typeSubmit)
        {
            return Button(helper, text, null, typeSubmit);
        }

        public static MvcHtmlString Button(this HtmlHelper helper, string text, IDictionary<string, object> htmlAttributes)
        {
            return Button(helper, text, htmlAttributes, false);
        }

        public static MvcHtmlString Button(this HtmlHelper helper, string text, IDictionary<string, object> htmlAttributes, bool typeSubmit)
        {
            var builder = new TagBuilder("input");

            builder.Attributes.Add("type", (typeSubmit ? "submit" : "button"));
            builder.Attributes.Add("value", text);

            if (htmlAttributes != null)
                builder.MergeAttributes(htmlAttributes);

            return MvcHtmlString.Create(builder.ToString());
        }
    }
}