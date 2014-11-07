using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;
using AspNetDeploy.WebUI.Models;

namespace AspNetDeploy.WebUI.Controllers
{
    public class BundlesController : GenericController
    {
        private readonly ITaskRunner taskRunner;

        public BundlesController(ITaskRunner taskRunner)
        {
            this.taskRunner = taskRunner;
        }

        public ActionResult Index()
        {
            List<Bundle> bundles = this.Entities.Bundle
                .Include("Projects")
                .ToList();

            this.ViewBag.Bundles = bundles.Select( b => new BundleInfo
            {
                Bundle = b,
                State = this.taskRunner.GetBundleState(b.Id),
                ProjectsInfo = b.Projects.Select( p => new ProjectInfo
                {
                    Project = p,
                    ProjectState = this.taskRunner.GetProjectState(p.Id)
                }).ToList()
            }).ToList();

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