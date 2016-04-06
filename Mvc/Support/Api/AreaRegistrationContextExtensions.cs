using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.WebHost;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Routing;
using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.IO;


namespace TeknoMobi.Common.Mvc
{
    public static class AreaRegistrationContextExtensions
    {
        public static Route MapHttpRoute(this AreaRegistrationContext context, string name, string routeTemplate)
        {
            return context.MapHttpRoute(name, routeTemplate, null, null);
        }

        public static Route MapHttpRoute(this AreaRegistrationContext context, string name, string routeTemplate, object defaults)
        {
            return context.MapHttpRoute(name, routeTemplate, defaults, null);
        }

        public static Route MapHttpRoute(this AreaRegistrationContext context, string name, string routeTemplate, object defaults, object constraints)
        {
            var route = context.Routes.MapHttpRoute(name, routeTemplate, defaults, constraints);
            if (route.DataTokens == null)
            {
                route.DataTokens = new RouteValueDictionary();
            }
            route.DataTokens.Add("area", context.AreaName);
            return route;
        }
    }
}