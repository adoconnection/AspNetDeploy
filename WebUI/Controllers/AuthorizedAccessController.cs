using System.Web.Mvc;
using AspNetDeploy.Contracts;

namespace AspNetDeploy.WebUI.Controllers
{
    [Authorize]
    public class AuthorizedAccessController : GenericController
    {
        public AuthorizedAccessController(ILoggingService loggingService) : base(loggingService)
        {
        }
    }
}