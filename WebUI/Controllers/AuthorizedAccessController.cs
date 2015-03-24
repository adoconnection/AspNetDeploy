using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;
using Environment = AspNetDeploy.Model.Environment;

namespace AspNetDeploy.WebUI.Controllers
{
    [Authorize]
    public class AuthorizedAccessController : GenericController
    {
        public AuthorizedAccessController(ILoggingService loggingService) : base(loggingService)
        {
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

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

        }

        
    }
}