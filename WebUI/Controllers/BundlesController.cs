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
                .Include("BundleVersions.ProjectVersions")
                .ToList();

            this.ViewBag.Environments = this.Entities.Environment.ToList();

            this.ViewBag.Bundles = bundles.Select( b => new BundleInfo
            {
                Bundle = b,
                BundleVersionsInfo = b.BundleVersions.Select( bv => new BundleVersionInfo()
                {
                    BundleVersion = bv,
                    State = this.taskRunner.GetBundleState(b.Id),
                    ProjectsVersionsInfo = bv.ProjectVersions.Select( pv => new ProjectVersionInfo
                    {
                        ProjectVersion = pv,
                        ProjectState = this.taskRunner.GetProjectState(pv.Id)
                    }).ToList()
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