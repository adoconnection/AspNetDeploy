using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;

namespace AspNetDeploy.WebUI.Controllers
{
    [ValidateInput(false)]
    public class GenericController : Controller
    {
        protected User ActiveUser { get; set; }

        protected AspNetDeployEntities Entities { get; private set; }

        private readonly ILoggingService loggingService;

        public GenericController(ILoggingService loggingService)
        {
            this.loggingService = loggingService;
        }

        public ActionResult Default()
        {
            if (!this.Entities.SourceControl.Any())
            {
                return this.RedirectToAction("Add", "Sources");
            }

            return this.RedirectToAction("List", "Bundles");
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            this.Entities = new AspNetDeployEntities();

            if (this.IsSetupState() && !filterContext.RouteData.Values["controller"].Equals("Setup"))
            {
                filterContext.Result = this.RedirectToAction(ConfigurationManager.AppSettings["Settings.SetupState"], "Setup");
            }

            base.OnActionExecuting(filterContext);
        }

        protected void Log(Exception e)
        {
            this.loggingService.Log(e, this.ActiveUser != null ? this.ActiveUser.Id : (int?)null);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (this.IsSetupState())
            {
                return;
            }

            this.loggingService.Log(filterContext.Exception, this.ActiveUser != null ? this.ActiveUser.Id : (int?) null);

            base.OnException(filterContext);
        }

        protected bool HasPermission(UserRoleAction roleAction)
        {
            return RolePermissions.MappingDictionary[this.ActiveUser.Role].Contains(roleAction);
        }

        protected void CheckPermission(UserRoleAction roleAction)
        {
            if (!this.HasPermission(roleAction))
            {
                throw new HttpException((int)HttpStatusCode.Forbidden, "Not authorized");
            }
        }

        protected bool IsSetupState()
        {
            return !string.IsNullOrEmpty(ConfigurationManager.AppSettings["Settings.SetupState"]);
        }
    }
}