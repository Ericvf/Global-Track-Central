using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Collections.Specialized;

namespace Trials.GTC.Mobile.Extensions
{
        public static class Extensions
        {
            public static RouteValueDictionary ToRouteValues(this NameValueCollection queryString, object routeValues = null)
            {
                var newRoute = new RouteValueDictionary();

                if (queryString != null && queryString.HasKeys())
                {
                    foreach (string key in queryString.AllKeys)
                        newRoute.Add(key, queryString[key]);
                }

                if (routeValues != null)
                {
                    foreach (var p in routeValues.GetType().GetProperties())
                        newRoute[p.Name] = p.GetValue(routeValues, null).ToString();
                }

                return newRoute;
            }

            public static string FixYoutubeUrl(this string url)
            {
                if (url.Contains("youtube"))
                {
                    return "http://youtu.be/" + url.ExtractYoutubeId();
                }
                else
                {
                    return url;
                }
            }

            public static string ExtractYoutubeId(this string url)
            {
                if (url.Contains("youtube"))
                {
                    var splitedUrl = url.Split(new[] { "v=" }, StringSplitOptions.None);
                    var splitedUrl2 = splitedUrl[1].Split('&');
                    var ytId = splitedUrl2[0];

                    return ytId;
                }
                else
                {
                    return null;
                }
            }
        }
}