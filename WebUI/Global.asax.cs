using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AspNetDeploy.WebUI.Bootstrapper;
using AspNetDeploy.WebUI.Controllers;

namespace AspNetDeploy.WebUI
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            ObjectFactoryConfigurator.Configure();
            ObjectFactoryConfigurator.SetControllerFactory(ControllerBuilder.Current);

            
        }
    }
}
