using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;
using AspNetDeploy.WebUI.Models;

namespace AspNetDeploy.WebUI.Controllers
{
    public class BundlesController : AuthorizedAccessController
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
                .OrderBy( b => b.OrderIndex)
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

        public ActionResult VersionPackages(int id)
        {
            BundleVersion bundleVersion = this.Entities.BundleVersion
                .Include("Bundle")
                .Include("ProjectVersions.Project.Properties")
                .Include("ProjectVersions.SourceControlVersion.SourceControl.Properties")
                .Include("DeploymentSteps.Properties")
                .Include("DeploymentSteps.MachineRoles")
                .Include("Packages.Publications.Environment.Machines")
                .Include("Packages.ApprovedOnEnvironments")
                .First( b => b.Id == id);

            List<Environment> environments = this.Entities.Environment.ToList();

            this.ViewBag.Environments = environments;
            this.ViewBag.BundleVersion = bundleVersion;

            return this.View();
        }
        public ActionResult VersionProjects(int id)
        {
            BundleVersion bundleVersion = this.Entities.BundleVersion
                .Include("Bundle")
                .Include("ProjectVersions.Project.Properties")
                .Include("ProjectVersions.SourceControlVersion.SourceControl.Properties")
                .Include("DeploymentSteps.Properties")
                .Include("DeploymentSteps.MachineRoles")
                .Include("Packages.Publications.Environment.Machines")
                .First( b => b.Id == id);

            this.ViewBag.BundleVersion = bundleVersion;

            return this.View();
        }

        public ActionResult VersionDeployment(int id)
        {
            BundleVersion bundleVersion = this.Entities.BundleVersion
                .Include("Bundle")
                .Include("ProjectVersions.Project.Properties")
                .Include("ProjectVersions.SourceControlVersion.SourceControl.Properties")
                .Include("DeploymentSteps.Properties")
                .Include("DeploymentSteps.MachineRoles")
                .Include("Packages.Publications.Environment.Machines")
                .First( b => b.Id == id);

            this.ViewBag.BundleVersion = bundleVersion;

            return this.View();
        }

        [HttpPost]
        public ActionResult GetBundleStates()
        {
            List<BundleVersion> bundleVersions = this.Entities.BundleVersion
                .Include("ProjectVersions")
                .ToList();

            var states = bundleVersions.Select(
                bv => new
                {
                    id = bv.Id,
                    state = this.taskRunner.GetBundleState(bv.Id).ToString(),
                    projects = bv.ProjectVersions.Select(pv => new
                    {
                        id = pv.Id,
                        state = this.taskRunner.GetProjectState(pv.Id).ToString()
                    }).ToList()
                })
                .ToList();

            return this.Json(states, JsonRequestBehavior.AllowGet);
        }
    }
}