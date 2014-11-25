using System.Linq;
using System.Web.Mvc;
using AspNetDeploy.Model;

namespace AspNetDeploy.WebUI.Controllers
{
    public class PublicationsController : AuthorizedAccessController
    {
        public ActionResult Details(int id)
        {
            Publication publication = this.Entities.Publication
                .Include("Environment")
                .Include("Package.BundleVersion.Bundle")
                .Include("Package.BundleVersion.DeploymentSteps.Properties")
                .Include("MachinePublication.Log")
                .Include("MachinePublication.Machine")
                .First( p => p.Id == id);



            this.ViewBag.Publication = publication;

            return this.View();
        }
    }
}