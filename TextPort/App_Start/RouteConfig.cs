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
            name: "bulk",
            url: "bulk",
            defaults: new { controller = "bulk", action = "index" }
            );

            routes.MapRoute(
            name: "bulk-upload",
            url: "bulk-upload",
            defaults: new { controller = "bulkupload", action = "index" }
            );

            routes.MapRoute(
            name: "bulk-upload-guidelines",
            url: "bulk-upload/upload-guidelines",
            defaults: new { controller = "bulkupload", action = "upload-guidelines" }
            );

            routes.MapRoute(
            name: "contacts",
            url: "contacts",
            defaults: new { controller = "contacts", action = "index" }
            );

            routes.MapRoute(
            name: "numbers",
            url: "numbers",
            defaults: new { controller = "numbers", action = "index" }
            );

            routes.MapRoute(
            name: "purchases",
            url: "purchases",
            defaults: new { controller = "purchases", action = "index" }
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
            name: "users",
            url: "users",
            defaults: new { controller = "users", action = "index" }
            );

            routes.MapRoute(
            name: "branches",
            url: "branches",
            defaults: new { controller = "branches", action = "index" }
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
