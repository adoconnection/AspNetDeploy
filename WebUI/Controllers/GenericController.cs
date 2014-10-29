using System;
using System.Web.Mvc;
using WebUI.Models;

namespace WebUI.Controllers
{
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