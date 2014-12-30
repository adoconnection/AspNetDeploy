using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;
using AspNetDeploy.WebUI.Models;
using Newtonsoft.Json;
using Environment = AspNetDeploy.Model.Environment;

namespace AspNetDeploy.WebUI.Controllers
{
    public class BundlesController : AuthorizedAccessController
    {
        private readonly ITaskRunner taskRunner;

        public BundlesController(ILoggingService loggingService, ITaskRunner taskRunner) : base(loggingService)
        {
            this.taskRunner = taskRunner;
        }

        public ActionResult List()
        {
            List<Bundle> bundles = this.Entities.Bundle
                .Include("BundleVersions.ProjectVersions")
                .Include("BundleVersions.Properties")
                .OrderBy( b => b.OrderIndex)
                .ToList();

            this.ViewBag.Environments = this.Entities.Environment.ToList();

            this.ViewBag.Bundles = bundles.Select( b => new BundleInfo
            {
                Bundle = b,
                BundleVersionsInfo = b.BundleVersions
                    .Where(bv => !bv.IsDeleted)
                    .Select(bv => new BundleVersionInfo()
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

        [HttpGet]
        public ActionResult CreateNewVersion(int id)
        {
            this.CheckPermission(UserRoleAction.VersionCreate);

            BundleVersion bundleVersion = this.Entities.BundleVersion
                .Include("Bundle")
                .Include("Properties")
                .Include("ProjectVersions.SourceControlVersion.SourceControl")
                .First(bv => bv.Id == id );

            this.ViewBag.BundleVersion = bundleVersion;

            return this.View();
        }
        
        [HttpPost]
        public ActionResult CreateNewVersion(int fromBundleVersionId, string jsonData, string newVersionName)
        {
            this.CheckPermission(UserRoleAction.VersionCreate);

            if (string.IsNullOrWhiteSpace(newVersionName))
            {
                return this.RedirectToAction("CreateNewVersion", new {id = fromBundleVersionId});
            }

            BundleVersion sourceBundleVersion = this.Entities.BundleVersion
                .Include("Bundle")
                .Include("ProjectVersions.Project")
                .Include("ProjectVersions.SourceControlVersion.SourceControl")
                .Include("Properties")
                .Include("DeploymentSteps.Properties")
                .Include("DeploymentSteps.MachineRoles")
                .First(bv => bv.Id == fromBundleVersionId);

            IEnumerable<dynamic> projects = (IEnumerable<dynamic>)JsonConvert.DeserializeObject(jsonData);

            Dictionary<int, ProjectVersion> mapping = projects
                .Select( p => new
                {
                    projectVersionId = (int) p.projectVersionId,
                    sourceControlVersionId = (int) p.sourceControlVersionId
                })
                .ToList()
                .Select( p => new
                {
                    p.projectVersionId,
                    p.sourceControlVersionId,
                    projectVersion = sourceBundleVersion.ProjectVersions.First(pv => pv.Id == p.projectVersionId)
                })
                .ToList()
                .Select(p => new
                {
                    p.projectVersion,
                    newProjectVersion = this.Entities.SourceControlVersion
                        .Where( scv => scv.Id == p.sourceControlVersionId)    
                        .ToList()
                        .SelectMany( scv => scv.ProjectVersions)
                        .FirstOrDefault( pn => pn.Project == p.projectVersion.Project)
                })
                .ToList()
                .ToDictionary(k => k.projectVersion.Id, v => v.newProjectVersion);

            BundleVersion newBundleVersion = new BundleVersion();
            newBundleVersion.IsHead = false;
            newBundleVersion.Bundle = sourceBundleVersion.Bundle;
            newBundleVersion.Name = newVersionName.Trim();
            newBundleVersion.ParentBundleVersion = sourceBundleVersion;

            if (sourceBundleVersion.IsHead)
            {
                sourceBundleVersion.IsHead = false;
                newBundleVersion.IsHead = true;
            }


            this.Entities.BundleVersion.Add(newBundleVersion);

            IList<ProjectVersion> usedProjectVersions = new List<ProjectVersion>();

            foreach (DeploymentStep sourceDeploymentStep in sourceBundleVersion.DeploymentSteps)
            {
                DeploymentStep newDeploymentStep = new DeploymentStep();
                newDeploymentStep.BundleVersion = newBundleVersion;
                newDeploymentStep.OrderIndex = sourceDeploymentStep.OrderIndex;
                newDeploymentStep.Type = sourceDeploymentStep.Type;

                foreach (MachineRole machineRole in sourceDeploymentStep.MachineRoles)
                {
                    newDeploymentStep.MachineRoles.Add(machineRole);
                }

                this.Entities.DeploymentStep.Add(newDeploymentStep);

                foreach (DeploymentStepProperty sourceDeploymentStepProperty in sourceDeploymentStep.Properties)
                {
                    DeploymentStepProperty newDeploymentStepProperty = new DeploymentStepProperty();
                    newDeploymentStepProperty.DeploymentStep = newDeploymentStep;
                    newDeploymentStepProperty.Key = sourceDeploymentStepProperty.Key;

                    if (sourceDeploymentStepProperty.Key == "ProjectId")
                    {
                        var newProjectVersion = mapping[int.Parse(sourceDeploymentStepProperty.Value)];
                        newDeploymentStepProperty.Value = newProjectVersion.Id.ToString(CultureInfo.InvariantCulture);
                        usedProjectVersions.Add(newProjectVersion);
                    }
                    else
                    {
                        newDeploymentStepProperty.Value = sourceDeploymentStepProperty.Value;
                    }

                    this.Entities.DeploymentStepProperty.Add(newDeploymentStepProperty);
                }
            }

            foreach (ProjectVersion usedProjectVersion in usedProjectVersions)
            {
                newBundleVersion.ProjectVersions.Add(usedProjectVersion);
            }

            this.Entities.SaveChanges();

            return this.RedirectToAction("Details", "Bundles", new { id = newBundleVersion.Bundle.Id});
        }

        public ActionResult Details(int id)
        {
            Bundle bundle = this.Entities.Bundle
                .Include("BundleVersions.Properties")
                .Include("BundleVersions.ProjectVersions.SourceControlVersion.SourceControl")
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
                .Include("Properties")
                .Include("ProjectVersions.SourceControlVersion.Properties")
                .ToList();

            var states = bundleVersions.Select(this.GetBundleVersionInfo).ToList();

            return this.Json(states, JsonRequestBehavior.AllowGet);
        }

        private object GetBundleVersionInfo(BundleVersion bundleVersion)
        {
            BundleState bundleState = this.taskRunner.GetBundleState(bundleVersion.Id);

            double elapsedSecs = 0;
            double totalSecs = 0;

            if (bundleState == BundleState.Deploying)
            {
                

                Publication publication = this.Entities.Publication.Where( p => p.State == PublicationState.InProgress && p.Package.BundleVersionId == bundleVersion.Id).OrderByDescending( p => p.CreatedDate).FirstOrDefault();

                Environment environment = this.Entities.Environment.Include("PreviousEnvironment").FirstOrDefault( e => e.Id == publication.EnvironmentId);

                if (publication != null)
                {
                    elapsedSecs = (DateTime.UtcNow - publication.CreatedDate).TotalSeconds;
                    totalSecs = bundleVersion.GetDoubleProperty("LastDeploymentDuration-e" + publication.EnvironmentId);

                    if (Math.Abs(totalSecs) < 1 && environment != null && environment.PreviousEnvironment.Count > 0)
                    {
                        totalSecs = bundleVersion.GetDoubleProperty("LastDeploymentDuration-e" + environment.PreviousEnvironment.First().Id);
                    }
                }
            }

            if (bundleState == BundleState.Packaging)
            {
                Package thisPackage = this.Entities.Package.Where(p => p.BundleVersionId == bundleVersion.Id && p.PackageDate == null).OrderByDescending(p => p.CreatedDate).FirstOrDefault();
                Package previousPackage = this.Entities.Package.Where(p => p.BundleVersionId == bundleVersion.Id && p.PackageDate != null).OrderByDescending(p => p.CreatedDate).FirstOrDefault();

                if (thisPackage != null && previousPackage != null && previousPackage.PackageDate.HasValue)
                {
                    elapsedSecs = (DateTime.UtcNow - thisPackage.CreatedDate).TotalSeconds;
                    totalSecs = (previousPackage.PackageDate.Value - previousPackage.CreatedDate).TotalSeconds;
                }
            }

            if (bundleState == BundleState.Building)
            {
                totalSecs = bundleVersion.GetDoubleProperty("LastBuildDuration");

                if (totalSecs > 0)
                {
                    elapsedSecs = (DateTime.UtcNow - bundleVersion.GetDateTimeProperty("LastBuildStartDate", DateTime.UtcNow)).TotalSeconds;
                }
            }

            return new
            {
                id = bundleVersion.Id,
                elapsedSecs = (int)elapsedSecs,
                totalSecs = (int)totalSecs,
                state = bundleState.ToString(),
                projects = bundleVersion.ProjectVersions.Select(this.GetProjectVersionInfo).ToList()
            };
        }

        private object GetProjectVersionInfo(ProjectVersion pv)
        {
            ProjectState projectState = this.taskRunner.GetProjectState(pv.Id);

            return new
            {
                id = pv.Id,
                state = projectState.ToString()
            };
        }
    }
}