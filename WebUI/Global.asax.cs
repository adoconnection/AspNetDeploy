using System;
using System.Configuration;
using System.Globalization;
using System.Threading;
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

            if (bool.Parse(ConfigurationManager.AppSettings["Settings.EnableTaskRunner"] ?? "false"))
            {
                Factory.GetInstance<ITaskRunner>().Initialize();
            }

            if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["Settings.WorkingFolder"]))
            {
                Factory.GetInstance<IEnvironmentResourcesService>().InitializeWorkingFolder(ConfigurationManager.AppSettings["Settings.WorkingFolder"]);
            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            CultureInfo culture = new CultureInfo("en-US");

            Thread.CurrentThread.CurrentUICulture = culture; 
            Thread.CurrentThread.CurrentCulture = culture;
        }
    }
}
