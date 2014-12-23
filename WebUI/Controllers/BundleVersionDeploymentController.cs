using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using AspNetDeploy.Contracts;
using AspNetDeploy.Contracts.Exceptions;
using AspNetDeploy.Model;
using AspNetDeploy.WebUI.Models;
using AspNetDeploy.WebUI.Models.DeploymentSteps;
using MvcSiteMapProvider.Linq;

namespace AspNetDeploy.WebUI.Controllers
{
    public class BundleVersionDeploymentController : AuthorizedAccessController
    {
        public BundleVersionDeploymentController(ILoggingService loggingService) : base(loggingService)
        {
        }

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

            if (deploymentStepType == DeploymentStepType.DeployDacpac)
            {
                this.ViewBag.ProjectsSelect = this.Entities.SourceControlVersion
                    .SelectMany(scv => scv.ProjectVersions)
                    .Where(pv => pv.ProjectType.HasFlag(ProjectType.Database) && !pv.Project.Properties.Any(p => p.Key == "NotForDeployment" && p.Value == "true"))
                    .Select(pv => new SelectListItem
                    {
                        Text = pv.SourceControlVersion.SourceControl.Name + " / " + pv.SourceControlVersion.Name + " / " + pv.Name,
                        Value = pv.Id.ToString()
                    })
                    .OrderBy(sli => sli.Text)
                    .ToList();

                DacpacDeploymentStepModel model = new DacpacDeploymentStepModel
                {
                    BundleVersionId = bundleVersion.Id
                };

                return this.View("EditDacpacStep", model);
            }

            if (deploymentStepType == DeploymentStepType.CopyFiles)
            {
                this.ViewBag.ProjectsSelect = this.Entities.SourceControlVersion
                    .SelectMany(scv => scv.ProjectVersions)
                    .Where(pv => pv.ProjectType.HasFlag(ProjectType.ZipArchive) && !pv.Project.Properties.Any(p => p.Key == "NotForDeployment" && p.Value == "true"))
                    .Select(pv => new SelectListItem
                    {
                        Text = pv.SourceControlVersion.SourceControl.Name + " / " + pv.SourceControlVersion.Name + " / " + pv.Name,
                        Value = pv.Id.ToString()
                    })
                    .OrderBy(sli => sli.Text)
                    .ToList();

                ZipArchiveDeploymentStepModel model = new ZipArchiveDeploymentStepModel
                {
                    BundleVersionId = bundleVersion.Id
                };

                return this.View("EditZipArchiveStep", model);
            }

            if (deploymentStepType == DeploymentStepType.UpdateHostsFile)
            {
                HostsDeploymentStepModel model = new HostsDeploymentStepModel
                {
                    BundleVersionId = bundleVersion.Id
                };

                return this.View("EditHostsStep", model);
            }

            if (deploymentStepType == DeploymentStepType.RunSQLScript)
            {
                SqlScriptDeploymentStepModel model = new SqlScriptDeploymentStepModel
                {
                    BundleVersionId = bundleVersion.Id
                };

                return this.View("EditSqlScriptStep", model);
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
                    OrderIndex = deploymentStep.OrderIndex,
                    DeploymentStepId = deploymentStepId,
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
            if (deploymentStep.Type == DeploymentStepType.CopyFiles)
            {
                ZipArchiveDeploymentStepModel model = new ZipArchiveDeploymentStepModel
                {
                    OrderIndex = deploymentStep.OrderIndex,
                    DeploymentStepId = deploymentStepId,
                    BundleVersionId = deploymentStep.BundleVersionId,
                    StepTitle = deploymentStep.GetStringProperty("Step.Title"),
                    ProjectId = deploymentStep.GetIntProperty("ProjectId"),
                    Destination = deploymentStep.GetStringProperty("DestinationPath"),
                    Roles = string.Join(", ", deploymentStep.MachineRoles.Select( mr => mr.Name)),
                    CustomConfigurationJson = deploymentStep.GetStringProperty("CustomConfiguration")
                };

                this.ViewBag.ProjectsSelect = this.Entities.SourceControlVersion
                    .SelectMany(scv => scv.ProjectVersions)
                    .Where(pv => pv.ProjectType.HasFlag(ProjectType.ZipArchive) && !pv.Project.Properties.Any(p => p.Key == "NotForDeployment" && p.Value == "true"))
                    .Select(pv => new SelectListItem
                    {
                        Text = pv.SourceControlVersion.SourceControl.Name + " / " + pv.SourceControlVersion.Name + " / " + pv.Name,
                        Value = pv.Id.ToString()
                    })
                    .OrderBy(sli => sli.Text)
                    .ToList();

                return this.View("EditZipArchiveStep", model);
            }

            if (deploymentStep.Type == DeploymentStepType.Configuration)
            {
                ConfigDeploymentStepModel model = new ConfigDeploymentStepModel
                {
                    OrderIndex = deploymentStep.OrderIndex,
                    BundleVersionId = deploymentStep.BundleVersionId,
                    DeploymentStepId = deploymentStepId,
                    ConfigJson = deploymentStep.GetStringProperty("SetValues"),
                    StepTitle = deploymentStep.GetStringProperty("Step.Title"),
                    File = deploymentStep.GetStringProperty("File"),
                    Roles = string.Join(", ", deploymentStep.MachineRoles.Select(mr => mr.Name))
                };

                return this.View("EditConfigStep", model);
            }

            if (deploymentStep.Type == DeploymentStepType.RunSQLScript)
            {
                SqlScriptDeploymentStepModel model = new SqlScriptDeploymentStepModel
                {
                    OrderIndex = deploymentStep.OrderIndex,
                    BundleVersionId = deploymentStep.BundleVersionId,
                    DeploymentStepId = deploymentStepId,
                    Roles = string.Join(", ", deploymentStep.MachineRoles.Select(mr => mr.Name)),
                    ConnectionString = deploymentStep.GetStringProperty("ConnectionString"),
                    StepTitle = deploymentStep.GetStringProperty("Step.Title"),
                    Command = deploymentStep.GetStringProperty("Command")
                };

                return this.View("EditSqlScriptStep", model);
            }

            if (deploymentStep.Type == DeploymentStepType.UpdateHostsFile)
            {
                HostsDeploymentStepModel model = new HostsDeploymentStepModel
                {
                    OrderIndex = deploymentStep.OrderIndex,
                    BundleVersionId = deploymentStep.BundleVersionId,
                    DeploymentStepId = deploymentStepId,
                    Roles = string.Join(", ", deploymentStep.MachineRoles.Select(mr => mr.Name)),
                    StepTitle = deploymentStep.GetStringProperty("Step.Title"),
                    ConfigJson = deploymentStep.GetStringProperty("ConfigurationJson")
                };

                return this.View("EditHostsStep", model);
            }

            if (deploymentStep.Type == DeploymentStepType.DeployDacpac)
            {
                DacpacDeploymentStepModel model = new DacpacDeploymentStepModel
                {
                    OrderIndex = deploymentStep.OrderIndex,
                    BundleVersionId = deploymentStep.BundleVersionId,
                    DeploymentStepId = deploymentStepId,
                    Roles = string.Join(", ", deploymentStep.MachineRoles.Select(mr => mr.Name)),
                    StepTitle = deploymentStep.GetStringProperty("Step.Title"),
                    ProjectId = deploymentStep.GetIntProperty("ProjectId"),
                    ConnectionString = deploymentStep.GetStringProperty("ConnectionString"),
                    TargetDatabase = deploymentStep.GetStringProperty("TargetDatabase"),
                    CustomConfiguration = deploymentStep.GetStringProperty("CustomConfiguration")
                };

                this.ViewBag.ProjectsSelect = this.Entities.SourceControlVersion
                    .SelectMany(scv => scv.ProjectVersions)
                    .Where(pv => pv.ProjectType.HasFlag(ProjectType.Database) && !pv.Project.Properties.Any(p => p.Key == "NotForDeployment" && p.Value == "true"))
                    .Select(pv => new SelectListItem
                    {
                        Text = pv.SourceControlVersion.SourceControl.Name + " / " + pv.SourceControlVersion.Name + " / " + pv.Name,
                        Value = pv.Id.ToString()
                    })
                    .OrderBy(sli => sli.Text)
                    .ToList();

                return this.View("EditDacpacStep", model);
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

            DeploymentStep deploymentStep = this.GetDeploymentStep(model, DeploymentStepType.Configuration);

            deploymentStep.SetStringProperty("SetValues", model.ConfigJson);
            deploymentStep.SetStringProperty("Step.Title", model.StepTitle);
            deploymentStep.SetStringProperty("File", model.File);

            this.SaveRoles(model, deploymentStep);

            this.Entities.SaveChanges();

            return this.RedirectToAction("VersionDeployment", "Bundles", new {id = deploymentStep.BundleVersionId});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveHostsStep(HostsDeploymentStepModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View("EditHostsStep", model);
            }

            DeploymentStep deploymentStep = this.GetDeploymentStep(model, DeploymentStepType.UpdateHostsFile);

            deploymentStep.SetStringProperty("ConfigurationJson", model.ConfigJson);
            deploymentStep.SetStringProperty("Step.Title", model.StepTitle);

            this.SaveRoles(model, deploymentStep);

            this.Entities.SaveChanges();

            return this.RedirectToAction("VersionDeployment", "Bundles", new {id = deploymentStep.BundleVersionId});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveSQLStep(SqlScriptDeploymentStepModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View("EditSqlScriptStep", model);
            }

            DeploymentStep deploymentStep = this.GetDeploymentStep(model, DeploymentStepType.RunSQLScript);

            deploymentStep.SetStringProperty("Step.Title", model.StepTitle);
            deploymentStep.SetStringProperty("Command", model.Command);
            deploymentStep.SetStringProperty("ConnectionString", model.ConnectionString);

            this.SaveRoles(model, deploymentStep);

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

            DeploymentStep deploymentStep = this.GetDeploymentStep(model, DeploymentStepType.DeployWebSite);

            deploymentStep.SetStringProperty("IIS.SiteName", model.SiteName);
            deploymentStep.SetStringProperty("IIS.DestinationPath", model.Destination);
            deploymentStep.SetStringProperty("IIS.Bindings", model.BindingsJson);
            deploymentStep.SetStringProperty("ProjectId", model.ProjectId.ToString());

            this.UpdateProjectReference(model, deploymentStep);

            this.SaveRoles(model, deploymentStep);

            this.Entities.SaveChanges();

            return this.RedirectToAction("VersionDeployment", "Bundles", new {id = deploymentStep.BundleVersionId});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveDacpacStep(DacpacDeploymentStepModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View("EditDacpacStep", model);
            }

            DeploymentStep deploymentStep = this.GetDeploymentStep(model, DeploymentStepType.DeployDacpac);

            deploymentStep.SetStringProperty("Step.Title", model.StepTitle);
            deploymentStep.SetStringProperty("ConnectionString", model.ConnectionString);
            deploymentStep.SetStringProperty("TargetDatabase", model.TargetDatabase);
            deploymentStep.SetStringProperty("CustomConfiguration", model.CustomConfiguration);
            deploymentStep.SetStringProperty("ProjectId", model.ProjectId.ToString());

            this.UpdateProjectReference(model, deploymentStep);
            this.SaveRoles(model, deploymentStep);
            this.Entities.SaveChanges();

            return this.RedirectToAction("VersionDeployment", "Bundles", new {id = deploymentStep.BundleVersionId});
        }
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveZipArchiveStep(ZipArchiveDeploymentStepModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View("EditZipArchiveStep", model);
            }

            DeploymentStep deploymentStep = this.GetDeploymentStep(model, DeploymentStepType.CopyFiles);

            deploymentStep.SetStringProperty("Step.Title", model.StepTitle);
            deploymentStep.SetStringProperty("DestinationPath", model.Destination);
            deploymentStep.SetStringProperty("CustomConfiguration", model.CustomConfigurationJson);
            deploymentStep.SetStringProperty("ProjectId", model.ProjectId.ToString(CultureInfo.InvariantCulture));

            this.UpdateProjectReference(model, deploymentStep);

            this.SaveRoles(model, deploymentStep);

            this.Entities.SaveChanges();

            return this.RedirectToAction("VersionDeployment", "Bundles", new {id = deploymentStep.BundleVersionId});
        }

        public ActionResult DeleteStep(int id, int deploymentStepId)
        {
            DeploymentStep deploymentStep = this.Entities.DeploymentStep
                .Include("Properties")
                .Include("MachineRoles")
                .Include("BundleVersion")
                .First(ds => ds.BundleVersionId == id && ds.Id == deploymentStepId);

            switch (deploymentStep.Type)
            {
                    case DeploymentStepType.DeployWebSite:
                    case DeploymentStepType.DeployDacpac:
                    case DeploymentStepType.CopyFiles:
                        this.UpdateProjectReference(new ProjectRelatedDeploymentStepModel
                        {
                            BundleVersionId = deploymentStep.BundleVersionId,
                            DeploymentStepId = deploymentStep.Id,
                            ProjectId = 0
                        }, deploymentStep);
                        break;
            }

            deploymentStep.MachineRoles.Clear();

            this.Entities.DeploymentStep.Remove(deploymentStep);
            this.Entities.SaveChanges();

            return this.RedirectToAction("VersionDeployment", "Bundles", new {id = deploymentStep.BundleVersionId});
        }

        private void SaveRoles(DeploymentStepModel model, DeploymentStep deploymentStep)
        {
            List<MachineRole> machineRoles = this.Entities.MachineRole.ToList();

            deploymentStep.MachineRoles.Clear();

            if (string.IsNullOrWhiteSpace(model.Roles))
            {
                return;
            }

            foreach (string role in model.Roles.ToLowerInvariant().Split(',').Select(r => r.Trim()))
            {
                MachineRole machineRole = machineRoles.FirstOrDefault(mr => mr.Name.ToLowerInvariant() == role);

                if (machineRole != null)
                {
                    deploymentStep.MachineRoles.Add(machineRole);
                }
            }
        }

        private DeploymentStep GetDeploymentStep(DeploymentStepModel model, DeploymentStepType deploymentStepType)
        {
            DeploymentStep deploymentStep;

            if (model.DeploymentStepId == 0)
            {
                deploymentStep = new DeploymentStep();
                deploymentStep.Type = deploymentStepType;
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

            return deploymentStep;
        }

        private void UpdateProjectReference(ProjectRelatedDeploymentStepModel model, DeploymentStep deploymentStep)
        {
            BundleVersion bundleVersion = this.Entities.BundleVersion
                .Include("ProjectVersions")
                .Include("DeploymentSteps.Properties")
                .First(bv => bv.Id == model.BundleVersionId);

            var activeProjectId = deploymentStep.GetIntProperty("ProjectId");

            if (activeProjectId > 0 && model.DeploymentStepId > 0) // remove unused project
            {
                if (bundleVersion.DeploymentSteps.Where(ds => ds.Id != model.DeploymentStepId).All(ds => ds.GetIntProperty("ProjectId") != activeProjectId))
                {
                    ProjectVersion activeProjectVersion = this.Entities.ProjectVersion.First(pv => pv.Id == activeProjectId);
                    bundleVersion.ProjectVersions.Remove(activeProjectVersion);
                }
            }

            if (model.ProjectId > 0) // add project
            {
                ProjectVersion projectVersion = this.Entities.ProjectVersion.First(pv => pv.Id == model.ProjectId);

                if (!bundleVersion.ProjectVersions.Contains(projectVersion))
                {
                    bundleVersion.ProjectVersions.Add(projectVersion);
                }
            }
        }

    }
}