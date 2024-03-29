﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace www.Helpers
{
    public static class HtmlHelpers
    {
        public static MvcHtmlString ActionImage(this HtmlHelper htmlHelper, string imageUrl, string linkText, string actionName)
        {
            return htmlHelper.ActionImage(imageUrl, linkText, actionName, null, new RouteValueDictionary(), new RouteValueDictionary());
        }

        public static MvcHtmlString ActionImage(this HtmlHelper htmlHelper, string imageUrl, string linkText, string actionName, object routeValues)
        {
            return htmlHelper.ActionImage(imageUrl, linkText, actionName, null, new RouteValueDictionary(routeValues), new RouteValueDictionary());
        }

        public static MvcHtmlString ActionImage(this HtmlHelper htmlHelper, string imageUrl, string linkText, string actionName, string controllerName)
        {
            return htmlHelper.ActionImage(imageUrl, linkText, actionName, controllerName, new RouteValueDictionary(), new RouteValueDictionary());
        }

        public static MvcHtmlString ActionImage(this HtmlHelper htmlHelper, string imageUrl, string linkText, string actionName, RouteValueDictionary routeValues)
        {
            return htmlHelper.ActionImage(imageUrl, linkText, actionName, null, routeValues, new RouteValueDictionary());
        }

        public static MvcHtmlString ActionImage(this HtmlHelper htmlHelper, string imageUrl, string linkText, string actionName, object routeValues, object htmlanchorAttributes)
        {
            return htmlHelper.ActionImage(imageUrl, linkText, actionName, null, new RouteValueDictionary(routeValues), new RouteValueDictionary(htmlanchorAttributes), null);
        }

        public static MvcHtmlString ActionImage(this HtmlHelper htmlHelper, string imageUrl, string linkText, string actionName, RouteValueDictionary routeValues, IDictionary<string, object> htmlanchorAttributes, IDictionary<string, object> htmlImageAttributes)
        {
            return htmlHelper.ActionImage(imageUrl, linkText, actionName, null, routeValues, htmlanchorAttributes, htmlImageAttributes);
        }

        public static MvcHtmlString ActionImage(this HtmlHelper htmlHelper, string imageUrl, string linkText, string actionName, string controllerName, object routeValues, object htmlanchorAttributes)
        {
            return htmlHelper.ActionImage(imageUrl, linkText, actionName, controllerName, new RouteValueDictionary(routeValues), new RouteValueDictionary(htmlanchorAttributes), null);
        }

        public static MvcHtmlString ActionImage(this HtmlHelper htmlHelper, string imageUrl, string linkText, string actionName, string controllerName, string protocol, string hostName, string fragment, object routeValues, object htmlanchorAttributes, object htmlImageAttributes)
        {
            return htmlHelper.ActionImage(imageUrl, linkText, actionName, controllerName, protocol, hostName, fragment, new RouteValueDictionary(routeValues), new RouteValueDictionary(htmlanchorAttributes), new RouteValueDictionary(htmlImageAttributes));
        }

        public static MvcHtmlString ActionImage<TController>(this HtmlHelper htmlHelper, Expression<Action<TController>> action, string imageUrl, string linkText) where TController : Controller
        {
            return htmlHelper.ActionImage(action, imageUrl, linkText, null, null, null);
        }

        public static MvcHtmlString ActionImage<TController>(this HtmlHelper htmlHelper, Expression<Action<TController>> action, string imageUrl, string linkText, object routeValues) where TController : Controller
        {
            return htmlHelper.ActionImage(action, imageUrl, linkText, routeValues, null, null);
        }

        public static MvcHtmlString ActionImage<TController>(this HtmlHelper htmlHelper, Expression<Action<TController>> action, string imageUrl, string linkText, object routeValues, object htmlanchorAttributes, object htmlImageAttributes) where TController : Controller
        {
            return htmlHelper.ActionImage(action, imageUrl, linkText, routeValues, new RouteValueDictionary(htmlanchorAttributes), new RouteValueDictionary(htmlImageAttributes));
        }

        //public static string ActionImage<TController>(this HtmlHelper htmlHelper, Expression<Action<TController>> action, string imageUrl, string linkText, object routeValues, IDictionary<string, object> htmlanchorAttributes, IDictionary<string, object> htmlImageAttributes) where TController : Controller
        //{

        //    var _routeValuesFromExpression = ExpressionHelper.GetRouteValuesFromExpression(action);
        //    var _mergedRouteValues = MergeRouteValueDictionaries(_routeValuesFromExpression, new RouteValueDictionary(routeValues));

        //    // get the action name
        //    //
        //    var _actionName = ((MethodCallExpression)action.Body).Method.Name;

        //    // get the bare url for the Action using the current
        //    // request context
        //    //     
        //    var _url = new UrlHelper(htmlHelper.ViewContext.RequestContext).Action(_actionName, _mergedRouteValues);

        //    return GetImageLink(_url, linkText, imageUrl, htmlanchorAttributes, htmlImageAttributes);

        //}

        public static MvcHtmlString ActionImage(this HtmlHelper htmlHelper, string imageUrl, string linkText, string actionName, string controllerName, RouteValueDictionary routeValues, IDictionary<string, object> htmlanchorAttributes, IDictionary<string, object> htmlImageAttributes)
        {
            // get the bare url for the Action using the current
            // request context
            //
            var url = new UrlHelper(htmlHelper.ViewContext.RequestContext).Action(actionName, controllerName, routeValues);

            return MvcHtmlString.Create(GetImageLink(url, linkText, imageUrl, htmlanchorAttributes, htmlImageAttributes));

        }

        public static MvcHtmlString ActionImage(this HtmlHelper htmlHelper, string imageUrl, string linkText, string actionName, string controllerName, string protocol, string hostName, string fragment, RouteValueDictionary routeValues, IDictionary<string, object> htmlanchorAttributes, IDictionary<string, object> htmlImageAttributes)
        {

            // get the bare url for the Action using the current
            // request context
            //
            var url = new UrlHelper(htmlHelper.ViewContext.RequestContext).Action(actionName, controllerName, routeValues, protocol, hostName);

            return MvcHtmlString.Create(GetImageLink(url, linkText, imageUrl, htmlanchorAttributes, htmlImageAttributes));

        }

        /// <summary>
        /// Build up the anchor and image tag.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="linkText">The link text.</param>
        /// <param name="imageUrl">The image URL.</param>
        /// <param name="htmlanchorAttributes">The HTML anchor attributes.</param>
        /// <param name="htmlImageAttributes">The HTML image attributes.</param>
        /// <returns></returns>
        internal static string GetImageLink(string url, string linkText, string imageUrl, IDictionary<string, object> htmlanchorAttributes, IDictionary<string, object> htmlImageAttributes)
        {
            // build up the image link.
            // <a href=\"ActionUrl\"><img src=\"ImageUrl\" alt=\"Your Link Text\" /></a>
            //

            var linkText1 = !string.IsNullOrEmpty(linkText) ? HttpUtility.HtmlEncode(linkText) : string.Empty;

            // build the img tag
            //
            TagBuilder image = new TagBuilder("img");
            image.MergeAttributes(htmlImageAttributes);
            image.MergeAttribute("src", imageUrl);
            image.MergeAttribute("alt", linkText1);
            image.MergeAttribute("border", "0");

            // build the anchor tag
            //
            TagBuilder link = new TagBuilder("a");
            link.MergeAttributes(htmlanchorAttributes);
            link.MergeAttribute("href", url);

            // place the img tag inside the anchor tag.
            //
            link.InnerHtml = image.ToString(TagRenderMode.Normal);

            // render the image link.
            //
            return link.ToString(TagRenderMode.Normal);

        }


        /// <summary>
        /// Merges the 2 source route value dictionaries.
        /// </summary>
        /// <param name="routeValueDictionary1">RouteValueDictionary 1.</param>
        /// <param name="routeValueDictionary2">RouteValueDictionary 2.</param>
        /// <returns></returns>
        internal static RouteValueDictionary MergeRouteValueDictionaries(RouteValueDictionary routeValueDictionary1, RouteValueDictionary routeValueDictionary2)
        {
            var mergedRouteValues = new RouteValueDictionary();

            if ((routeValueDictionary1 != null) & (routeValueDictionary2 != null))
            {
                foreach (KeyValuePair<string, object> routeElement in routeValueDictionary1)
                {
                    mergedRouteValues[routeElement.Key] = routeElement.Value;
                }

                foreach (KeyValuePair<string, object> routeElement in routeValueDictionary2)
                {
                    mergedRouteValues[routeElement.Key] = routeElement.Value;
                }

                return mergedRouteValues;
            }

            return null;
        }
    }
}
