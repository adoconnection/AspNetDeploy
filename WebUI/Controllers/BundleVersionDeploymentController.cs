using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AspNetDeploy.Model;
using AspNetDeploy.WebUI.Models;
using MvcSiteMapProvider.Linq;

namespace AspNetDeploy.WebUI.Controllers
{
    public class BundleVersionDeploymentController : GenericController
    {
        public ActionResult MoveUp(int bundleVersionId, int deploymentStepId)
        {
            BundleVersion bundleVersion = this.Entities.BundleVersion
                .Include("DeploymentSteps")
                .First(bv => bv.Id == bundleVersionId);

            List<DeploymentStep> deploymentSteps = bundleVersion.DeploymentSteps.OrderBy( ds => ds.OrderIndex).ToList();
            DeploymentStep deploymentStep = deploymentSteps.First( ds => ds.Id == deploymentStepId);

            int index = deploymentSteps.IndexOf(deploymentStep);

            if (index > 0)
            {
                deploymentSteps[index - 1].OrderIndex ++;
                deploymentSteps[index].OrderIndex --;
            }

            this.Entities.SaveChanges();

            return this.RedirectToAction("VersionDeployment", "Bundles", new {id = bundleVersionId});
        }
        public ActionResult MoveDown(int bundleVersionId, int deploymentStepId)
        {
            BundleVersion bundleVersion = this.Entities.BundleVersion
                .Include("DeploymentSteps")
                .First(bv => bv.Id == bundleVersionId);

            List<DeploymentStep> deploymentSteps = bundleVersion.DeploymentSteps.OrderBy(ds => ds.OrderIndex).ToList();
            DeploymentStep deploymentStep = deploymentSteps.First(ds => ds.Id == deploymentStepId);

            int index = deploymentSteps.IndexOf(deploymentStep);

            if (index < deploymentSteps.Count - 1)
            {
                deploymentSteps[index + 1].OrderIndex--;
                deploymentSteps[index].OrderIndex++;
            }

            this.Entities.SaveChanges();

            return this.RedirectToAction("VersionDeployment", "Bundles", new {id = bundleVersionId});
        }

        public ActionResult AddStep(int id)
        {
            BundleVersion bundleVersion = this.Entities.BundleVersion
                .Include("DeploymentSteps")
                .First(bv => bv.Id == id);

            this.ViewBag.BundleVersion = bundleVersion;

            return this.View();
        }

        public ActionResult EditStep(int id, int deploymentStepId)
        {
            DeploymentStep deploymentStep = this.Entities.DeploymentStep
                .Include("Properties")
                .Include("MachineRoles")
                .Include("BundleVersion.Bundle")
                .First( ds => ds.Id == deploymentStepId && ds.BundleVersion.Id == id);

            this.ViewBag.DeploymentStep = deploymentStep;

            if (deploymentStep.Type == DeploymentStepType.DeployWebSite)
            {
                WebSiteDeploymentStepModel model = new WebSiteDeploymentStepModel
                {
                    
                };

                return this.View("EditWebsiteStep", model);
            }

            if (deploymentStep.Type == DeploymentStepType.Configuration)
            {
                ConfigDeploymentStepModel model = new ConfigDeploymentStepModel
                {
                    DeploymentStepId = deploymentStepId,
                    ConfigJson = deploymentStep.GetStringProperty("SetValues"),
                    StepTitle = deploymentStep.GetStringProperty("Step.Title"),
                    File = deploymentStep.GetStringProperty("File")
                };

                return this.View("EditConfigStep", model);
            }

            return this.Content("Unsupported step type");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateConfigStep(ConfigDeploymentStepModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View("EditConfigStep", model);
            }

            DeploymentStep deploymentStep = this.Entities.DeploymentStep
                .Include("Properties")
                .Include("MachineRoles")
                .Include("BundleVersion.Bundle")
                .First(ds => ds.Id == model.DeploymentStepId);

            deploymentStep.SetStringProperty("SetValues", model.ConfigJson);
            deploymentStep.SetStringProperty("Step.Title", model.StepTitle);
            deploymentStep.SetStringProperty("File", model.File);

            this.Entities.SaveChanges();

            return this.RedirectToAction("VersionDeployment", "Bundles", new {id = deploymentStep.BundleVersionId});
        }
    }
}