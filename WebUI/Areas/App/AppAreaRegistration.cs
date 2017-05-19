using System.Web.Mvc;

namespace AspNetDeploy.WebUI.Areas.App
{
    public class AppAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "App";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "App_commandTest",
                "App/CommandsTest",
                new { action = "Index", controller = "AppCommandTest" }
            );

            context.MapRoute(
                "App_default",
                "App/{*url}",
                new { action = "Page", controller = "AppPage" }
            );
        }
    }
}