using System.Web.Mvc;
using AspNetDeploy.Model;

namespace AspNetDeploy.WebUI.Controllers
{
    [ValidateInput(false)]
    public class GenericController : Controller
    {
        protected AspNetDeployEntities Entities { get; private set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            this.Entities = new AspNetDeployEntities();
            base.OnActionExecuting(filterContext);
        }
    }
}