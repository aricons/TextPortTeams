using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TextPort
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
            name: "messages",
            url: "messages",
            defaults: new { controller = "messages", action = "index" }
            );

            routes.MapRoute(
            name: "trial",
            url: "trial",
            defaults: new { controller = "trial", action = "index" }
            );

            routes.MapRoute(
            name: "bulk",
            url: "bulk",
            defaults: new { controller = "bulk", action = "index" }
            );

            routes.MapRoute(
            name: "numbers",
            url: "numbers",
            defaults: new { controller = "numbers", action = "index" }
            );

            routes.MapRoute(
            name: "api",
            url: "api",
            defaults: new { controller = "smsapi", action = "index" }
            );

            routes.MapRoute(
            name: "apidocs",
            url: "api/documentation",
            defaults: new { controller = "smsapi", action = "setup" }
            );

            routes.MapRoute(
            name: "apidocsdetail",
            url: "api/documentation/{action}",
            defaults: new { controller = "smsapi", action = "Index" });

            routes.MapRoute(
            name: "group",
            url: "group",
            defaults: new { controller = "group", action = "index" }
            );

            routes.MapRoute(
            name: "groups",
            url: "groups",
            defaults: new { controller = "groups", action = "index" }
            );

            routes.MapRoute(
            name: "emailtosms",
            url: "emailtosms",
            defaults: new { controller = "emailtosms", action = "index" }
            );

            routes.MapRoute(
            name: "apisettings",
            url: "apisettings",
            defaults: new { controller = "apisettings", action = "index" }
            );

            routes.MapRoute(
            name: "blog",
            url: "blog",
            defaults: new { controller = "blog", action = "index" }
            );

            routes.MapRoute(
            name: "blogarticle",
            url: "blog/{action}",
            defaults: new { controller = "blog", action = "Index" });

            routes.MapRoute(
            name: "DefaultActionOnlyOnHomeController",
            url: "{action}",
            defaults: new { controller = "home", action = "index" }
            );

            routes.MapRoute(
            name: "Default",
            url: "{controller}/{action}/{id}/{p1}",
            defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional, p1 = UrlParameter.Optional }
            );
        }
    }
}
