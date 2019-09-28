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

            //  routes.MapRoute(
            //      name: "Contact",
            //      url: "contact",
            //      defaults: new { controller = "Home", action = "Contact" }
            //  );

            //  routes.MapRoute(
            //      name: "Support",
            //      url: "support",
            //      defaults: new { controller = "Home", action = "Support" }
            //  );

            //  routes.MapRoute(
            //      name: "Terms",
            //      url: "terms",
            //      defaults: new { controller = "Home", action = "Terms" }
            //  );

            //  routes.MapRoute(
            //      name: "Privacy",
            //      url: "privacy",
            //      defaults: new { controller = "Home", action = "Privacy" }
            //  );

            //  routes.MapRoute(
            //    name: "DedicatedVirtualNumbers",
            //    url: "DedicatedVirtualNumbers",
            //    defaults: new { controller = "Home", action = "DedicatedVirtualNumbers" }
            //);

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
                name: "group",
                url: "group",
                defaults: new { controller = "group", action = "index" }
            );

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
