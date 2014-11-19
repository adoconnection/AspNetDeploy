using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AspNetDeploy.Contracts;
using AspNetDeploy.WebUI.Bootstrapper;
using AspNetDeploy.WebUI.Mvc;
using ObjectFactory;

namespace AspNetDeploy.WebUI
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            ObjectFactoryConfigurator.Configure();

            ControllerBuilder.Current.SetControllerFactory(typeof(ControllerFactory)); 
            
            //Factory.GetInstance<ITaskRunner>().Initialize();
        }
    }
}
