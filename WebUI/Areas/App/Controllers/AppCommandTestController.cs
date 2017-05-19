using System.Web.Mvc;
using AspNetDeploy.Contracts;

namespace AspNetDeploy.WebUI.Areas.App.Controllers
{
    public class AppCommandTestController : AppController
    {
        public AppCommandTestController(ILoggingService loggingService) : base(loggingService)
        {
        }

        public ActionResult Index()
        {
            return this.View();
        }
    }
}