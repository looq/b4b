using System.Web.Mvc;
using System.Web.Routing;

namespace Breast_For_Baby
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapMvcAttributeRoutes();

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "BreastForBaby", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
