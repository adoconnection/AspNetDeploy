using System.Web.Mvc;
using System.Web.Routing;

namespace AspNetDeploy.WebUI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("favicon.ico");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Generic", action = "Default", id = UrlParameter.Optional }
            );
        }
    }
}
