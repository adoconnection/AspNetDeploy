using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AspNetDeploy.Contracts.Exceptions;
using AspNetDeploy.Model;
using AspNetDeploy.WebUI.Models;
using MvcSiteMapProvider.Linq;

namespace AspNetDeploy.WebUI.Controllers
{
    public class BundleVersionDeploymentController : AuthorizedAccessController
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

        public ActionResult AddStep(int id, DeploymentStepType deploymentStepType = DeploymentStepType.Undefined)
        {
            BundleVersion bundleVersion = this.Entities.BundleVersion
                .Include("DeploymentSteps")
                .First(bv => bv.Id == id);

            List<MachineRole> machineRoles = this.Entities.MachineRole.ToList();

            this.ViewBag.BundleVersion = bundleVersion;
            this.ViewBag.MachineRoles = machineRoles;

            if (deploymentStepType == DeploymentStepType.Undefined)
            {
                return this.View("AddStep");
            }

            this.ViewBag.DeploymentStep = new DeploymentStep();

            if (deploymentStepType == DeploymentStepType.DeployWebSite)
            {
                this.ViewBag.ProjectsSelect = this.Entities.SourceControlVersion
                    .SelectMany(scv => scv.ProjectVersions)
                    .Where(pv => pv.ProjectType.HasFlag(ProjectType.Web) && !pv.Project.Properties.Any(p => p.Key == "NotForDeployment" && p.Value == "true"))
                    .Select(pv => new SelectListItem
                    {
                        Text = pv.SourceControlVersion.SourceControl.Name + " / " + pv.SourceControlVersion.Name + " / " + pv.Name,
                        Value = pv.Id.ToString()
                    })
                    .OrderBy(sli => sli.Text)
                    .ToList();

                WebSiteDeploymentStepModel model = new WebSiteDeploymentStepModel
                {
                    BundleVersionId = bundleVersion.Id
                };

                return this.View("EditWebsiteStep", model);
            }

            if (deploymentStepType == DeploymentStepType.Configuration)
            {
                ConfigDeploymentStepModel model = new ConfigDeploymentStepModel
                {
                    BundleVersionId = bundleVersion.Id
                };

                return this.View("EditConfigStep", model);
            }
            
            throw new AspNetDeployException("Invalid deployment step type");
        }

        public ActionResult EditStep(int id, int deploymentStepId)
        {
            DeploymentStep deploymentStep = this.Entities.DeploymentStep
                .Include("Properties")
                .Include("MachineRoles")
                .Include("BundleVersion.Bundle")
                .First( ds => ds.Id == deploymentStepId && ds.BundleVersion.Id == id);

            List<MachineRole> machineRoles = this.Entities.MachineRole.ToList();

            this.ViewBag.DeploymentStep = deploymentStep;
            this.ViewBag.BundleVersion = deploymentStep.BundleVersion;
            this.ViewBag.MachineRoles = machineRoles;

            if (deploymentStep.Type == DeploymentStepType.DeployWebSite)
            {
                WebSiteDeploymentStepModel model = new WebSiteDeploymentStepModel
                {
                    BundleVersionId = deploymentStep.BundleVersionId,
                    SiteName = deploymentStep.GetStringProperty("IIS.SiteName"),
                    ProjectId = deploymentStep.GetIntProperty("ProjectId"),
                    Destination = deploymentStep.GetStringProperty("IIS.DestinationPath"),
                    Roles = string.Join(", ", deploymentStep.MachineRoles.Select( mr => mr.Name)),
                    BindingsJson = deploymentStep.GetStringProperty("IIS.Bindings")
                };

                this.ViewBag.ProjectsSelect = this.Entities.SourceControlVersion
                    .SelectMany(scv => scv.ProjectVersions)
                    .Where(pv => pv.ProjectType.HasFlag(ProjectType.Web) && !pv.Project.Properties.Any(p => p.Key == "NotForDeployment" && p.Value == "true"))
                    .Select(pv => new SelectListItem
                    {
                        Text = pv.SourceControlVersion.SourceControl.Name + " / " + pv.SourceControlVersion.Name + " / " + pv.Name,
                        Value = pv.Id.ToString()
                    })
                    .OrderBy(sli => sli.Text)
                    .ToList();

                return this.View("EditWebsiteStep", model);
            }

            if (deploymentStep.Type == DeploymentStepType.Configuration)
            {
                ConfigDeploymentStepModel model = new ConfigDeploymentStepModel
                {
                    BundleVersionId = deploymentStep.BundleVersionId,
                    DeploymentStepId = deploymentStepId,
                    ConfigJson = deploymentStep.GetStringProperty("SetValues"),
                    StepTitle = deploymentStep.GetStringProperty("Step.Title"),
                    File = deploymentStep.GetStringProperty("File"),
                    Roles = string.Join(", ", deploymentStep.MachineRoles.Select(mr => mr.Name))
                };

                return this.View("EditConfigStep", model);
            }

            return this.Content("Unsupported step type");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveConfigStep(ConfigDeploymentStepModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View("EditConfigStep", model);
            }

            DeploymentStep deploymentStep;

            if (model.DeploymentStepId == 0)
            {
                deploymentStep = new DeploymentStep();
                deploymentStep.Type = DeploymentStepType.Configuration;
                deploymentStep.BundleVersionId = model.BundleVersionId;
                deploymentStep.OrderIndex = this.Entities.DeploymentStep.Count(ds => ds.BundleVersionId == model.BundleVersionId) + 1;
                this.Entities.DeploymentStep.Add(deploymentStep);
            }
            else
            {
                deploymentStep = this.Entities.DeploymentStep
                    .Include("Properties")
                    .First(ds => ds.Id == model.DeploymentStepId);
            }

            deploymentStep.SetStringProperty("SetValues", model.ConfigJson);
            deploymentStep.SetStringProperty("Step.Title", model.StepTitle);
            deploymentStep.SetStringProperty("File", model.File);

            List<MachineRole> machineRoles = this.Entities.MachineRole.ToList();

            deploymentStep.MachineRoles.Clear();

            if (!string.IsNullOrWhiteSpace(model.Roles))
            {
                foreach (string role in model.Roles.ToLowerInvariant().Split(',').Select(r => r.Trim()))
                {
                    MachineRole machineRole = machineRoles.FirstOrDefault(mr => mr.Name.ToLowerInvariant() == role);

                    if (machineRole != null)
                    {
                        deploymentStep.MachineRoles.Add(machineRole);
                    }
                }
            }

            this.Entities.SaveChanges();

            return this.RedirectToAction("VersionDeployment", "Bundles", new {id = deploymentStep.BundleVersionId});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveWebSiteStep(WebSiteDeploymentStepModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View("EditWebsiteStep", model);
            }

            DeploymentStep deploymentStep;

            if (model.DeploymentStepId == 0)
            {
                deploymentStep = new DeploymentStep();
                deploymentStep.Type = DeploymentStepType.DeployWebSite;
                deploymentStep.BundleVersionId = model.BundleVersionId;
                deploymentStep.OrderIndex = this.Entities.DeploymentStep.Count(ds => ds.BundleVersionId == model.BundleVersionId) + 1;
                this.Entities.DeploymentStep.Add(deploymentStep);
            }
            else
            {
                deploymentStep = this.Entities.DeploymentStep
                    .Include("Properties")
                    .First(ds => ds.Id == model.DeploymentStepId);
            }

            int activeProjectId = deploymentStep.GetIntProperty("ProjectId");

            deploymentStep.SetStringProperty("IIS.SiteName", model.SiteName);
            deploymentStep.SetStringProperty("IIS.DestinationPath", model.Destination);
            deploymentStep.SetStringProperty("IIS.Bindings", model.BindingsJson);
            deploymentStep.SetStringProperty("ProjectId", model.ProjectId.ToString());

            BundleVersion bundleVersion = this.Entities.BundleVersion
                .Include("ProjectVersions")
                .Include("DeploymentSteps.Properties")
                .First(bv => bv.Id == model.BundleVersionId);

            if (activeProjectId > 0 && model.DeploymentStepId > 0) // remove unused project
            {
                if (bundleVersion.DeploymentSteps.Where(ds => ds.Id != model.DeploymentStepId).All(ds => ds.GetIntProperty("ProjectId") != activeProjectId))
                {
                    ProjectVersion activeProjectVersion = this.Entities.ProjectVersion.First( pv => pv.Id == activeProjectId);
                    bundleVersion.ProjectVersions.Remove(activeProjectVersion);
                }
            }

            ProjectVersion projectVersion = this.Entities.ProjectVersion.First( pv => pv.Id == model.ProjectId);

            if (!bundleVersion.ProjectVersions.Contains(projectVersion))
            {
                bundleVersion.ProjectVersions.Add(projectVersion);
            }

            List<MachineRole> machineRoles = this.Entities.MachineRole.ToList();

            deploymentStep.MachineRoles.Clear();

            if (!string.IsNullOrWhiteSpace(model.Roles))
            {
                foreach (string role in model.Roles.ToLowerInvariant().Split(',').Select(r => r.Trim()))
                {
                    MachineRole machineRole = machineRoles.FirstOrDefault(mr => mr.Name.ToLowerInvariant() == role);

                    if (machineRole != null)
                    {
                        deploymentStep.MachineRoles.Add(machineRole);
                    }
                }
            }

            this.Entities.SaveChanges();

            return this.RedirectToAction("VersionDeployment", "Bundles", new {id = deploymentStep.BundleVersionId});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteStep(int id)
        {
            DeploymentStep deploymentStep = this.Entities.DeploymentStep.First( ds => ds.Id == id);

            this.Entities.DeploymentStep.Remove(deploymentStep);
            this.Entities.SaveChanges();

            return this.RedirectToAction("VersionDeployment", "Bundles", new {id = deploymentStep.BundleVersionId});
        }
    }
}