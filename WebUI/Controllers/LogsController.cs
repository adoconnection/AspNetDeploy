using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;

namespace AspNetDeploy.WebUI.Controllers
{
    public class LogsController : AuthorizedAccessController
    {
        public LogsController(ILoggingService loggingService) : base(loggingService)
        {
        }

        public ActionResult List()
        {
            this.CheckPermission(UserRoleAction.ViewLogs);

            List<AspNetDeployExceptionEntry> aspNetDeployExceptionEntries = this.Entities.AspNetDeployExceptionEntry
                .Include("ExceptionEntry.InnerExceptionEntry")
                .Include("ExceptionEntry.ExceptionData")
                .Include("User")
                .OrderByDescending(ex => ex.TimeStamp)
                .Take(100)
                .ToList();

            this.ViewBag.AspNetDeployExceptionEntries = aspNetDeployExceptionEntries;

            return this.View();
        }

        
        public ActionResult Details(int id)
        {
            this.CheckPermission(UserRoleAction.ViewLogs);

            AspNetDeployExceptionEntry aspNetDeployExceptionEntry = this.Entities.AspNetDeployExceptionEntry
                .Include("ExceptionEntry.InnerExceptionEntry")
                .Include("ExceptionEntry.ExceptionData")
                .Include("User")
                .OrderByDescending(ex => ex.TimeStamp)
                .Take(100)
                .First( ex => ex.Id == id);

            this.ViewBag.aspNetDeployExceptionEntry = aspNetDeployExceptionEntry;

            return this.View();
        }
    }
}