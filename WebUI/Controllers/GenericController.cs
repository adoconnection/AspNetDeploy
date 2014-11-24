using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AspNetDeploy.Model;

namespace AspNetDeploy.WebUI.Controllers
{
    [ValidateInput(false)]
    public class GenericController : Controller
    {
        protected User ActiveUser { get; private set; }
        protected AspNetDeployEntities Entities { get; private set; }


        public ActionResult Default()
        {
            return this.RedirectToAction("Index", "Sources");
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