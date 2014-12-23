using System;
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
        protected User ActiveUser { get; private set; }
        protected AspNetDeployEntities Entities { get; private set; }

        private readonly ILoggingService loggingService;

        

        public GenericController(ILoggingService loggingService)
        {
            this.loggingService = loggingService;
        }

        public ActionResult Default()
        {
            return this.RedirectToAction("List", "Sources");
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            this.Entities = new AspNetDeployEntities();

            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                Guid guid = new Guid(filterContext.HttpContext.User.Identity.Name);
                User user = this.Entities.User.FirstOrDefault(u => u.Guid == guid);

                if (user == null || user.IsDisabled)
                {
                    filterContext.Result = this.RedirectToAction("Logout", "Account");
                    return;
                }

                this.ActiveUser = user;
                this.ViewBag.AllowedActions = RolePermissions.MappingDictionary[this.ActiveUser.Role];
                this.ViewBag.ActiveUser = user;
            }

            base.OnActionExecuting(filterContext);
        }

        protected void Log(Exception e)
        {
            this.loggingService.Log(e, this.ActiveUser.Id);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
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
    }
}