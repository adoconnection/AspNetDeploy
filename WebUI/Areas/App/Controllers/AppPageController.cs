using System.Web.Mvc;
using AspNetDeploy.Contracts;

namespace AspNetDeploy.WebUI.Areas.App.Controllers
{
    public class AppPageController : AppController
    {
        public AppPageController(ILoggingService loggingService) : base(loggingService)
        {
        }

        public ActionResult Page(string url)
        {
            return this.View();
        }
    }
}