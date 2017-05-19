using System.Web.Mvc;
using AspNetDeploy.Contracts;
using AspNetDeploy.WebUI.Controllers;

namespace AspNetDeploy.WebUI.Areas.App.Controllers
{
    [Authorize]
    public class AppController : GenericController
    {
        public AppController(ILoggingService loggingService) : base(loggingService)
        {
        }
    }
}