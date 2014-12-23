using System.Linq;
using System.Web.Mvc;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;

namespace AspNetDeploy.WebUI.Controllers
{
    public class ProjectVersionsController : AuthorizedAccessController
    {
        public ProjectVersionsController(ILoggingService loggingService) : base(loggingService)
        {
        }

        public ActionResult Details(int id)
        {
            ProjectVersion projectVersion = this.Entities.ProjectVersion
                .Include("Project")
                .Include("BundleVersions")
                .Include("SourceControlVersion.Properties")
                .Include("SourceControlVersion.SourceControl.Properties")
                .First(pv => pv.Id == id);

            this.ViewBag.ProjectVersion = projectVersion;

            return this.View();
        }

        public ActionResult MarkProjectNotForDeployment(int id)
        {
            return this.ChangeProjectNotForDeploymentProperty(id, true);
        }

        public ActionResult MarkProjectForDeployment(int id)
        {
            return this.ChangeProjectNotForDeploymentProperty(id, false);
        }

        private ActionResult ChangeProjectNotForDeploymentProperty(int id, bool value)
        {
            ProjectVersion projectVersion = this.Entities.ProjectVersion
                .Include("Project")
                .First(pv => pv.Id == id);

            projectVersion.Project.SetBoolProperty("NotForDeployment", value);

            this.Entities.SaveChanges();

            return this.RedirectToAction("Details", new {id});
        }
    }
}