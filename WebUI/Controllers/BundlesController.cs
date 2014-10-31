using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AspNetDeploy.Model;

namespace AspNetDeploy.WebUI.Controllers
{
    public class BundlesController : GenericController
    {
        public ActionResult Index()
        {
            List<Bundle> bundles = this.Entities.Bundle
                .Include("Projects")
                .ToList();

            this.ViewBag.Bundles = bundles;

            return this.View();
        }

        public ActionResult Details(int id)
        {
            Bundle bundle = this.Entities.Bundle
                .Include("Projects.SourceControl")
                .Include("DeploymentSteps.Properties")
                .Include("DeploymentSteps.MachineRoles")
                .First( b => b.Id == id);

            this.ViewBag.Bundle = bundle;

            return this.View();
        }
    }
}