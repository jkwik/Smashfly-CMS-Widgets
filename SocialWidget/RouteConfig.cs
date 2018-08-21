using DotNetNuke.Web.Mvc.Routing;

namespace SF.CMS.Widgets.SocialWidget
{
    public class RouteConfig : IMvcRouteMapper
    {
        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            mapRouteManager.MapRoute("SocialSharing", "SocialSharing", "{controller}/{action}", new[]
                {"SF.CMS.Widgets.SocialWidget.Controllers"});
        }
    }
}