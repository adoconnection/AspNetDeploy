using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AspNetDeploy.Contracts;
using AspNetDeploy.Contracts.Exceptions;
using AspNetDeploy.Model;
using AspNetDeploy.WebUI.Models;

namespace AspNetDeploy.WebUI.Controllers
{
    public class SourcesController : AuthorizedAccessController
    {
        private readonly ITaskRunner taskRunner;

        public SourcesController(ILoggingService loggingService, ITaskRunner taskRunner) : base(loggingService)
        {
            this.taskRunner = taskRunner;
        }

        public ActionResult List()
        {
            List<SourceControl> sourceControls = this.Entities.SourceControl
                .Include("SourceControlVersions.Properties")
                .Include("SourceControlVersions.ProjectVersions.BundleVersions")
                .Include("Properties")
                .OrderBy(b => b.OrderIndex)
                .ToList();

            this.ViewBag.SourceControls = sourceControls.Select( 
                sc => new SourceControlInfo
                {
                    SourceControl = sc,
                    SourceControlVersionsInfo = sc.SourceControlVersions.Select( scv => new SourceControlVersionInfo()
                    {
                        SourceControlVersion = scv,
                        State = this.taskRunner.GetSourceControlState(scv.Id),
                        ProjectVersionsInfo = scv.ProjectVersions
                        .Select(pv =>
                            new ProjectVersionInfo
                            {
                                ProjectVersion = pv,
                                ProjectState = this.taskRunner.GetProjectState(pv.Id)
                            })
                        .ToList()
                    }).ToList(),
                })
                .ToList();

            return this.View();
        }

        public ActionResult ProjectVersionDetails(int id)
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

        [HttpPost]
        public ActionResult GetSourceControlStates()
        {
            List<SourceControlVersion> sourceControlVersions = this.Entities.SourceControlVersion.ToList();

            var states = sourceControlVersions.Select(
                scv => new 
                {
                    id = scv.Id,
                    state = this.taskRunner.GetSourceControlState(scv.Id).ToString(),
                    projects = scv.ProjectVersions.Select(pv => new
                    {
                        id = pv.Id,
                        state = this.taskRunner.GetProjectState(pv.Id).ToString()
                    }).ToList()
                })
                .ToList();

            return this.Json(states, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(int id)
        {
            SourceControl sourceControl = this.Entities.SourceControl
                .Include("SourceControlVersions.Properties")
                .Include("Properties")
                .First( sc => sc.Id == id);

            this.ViewBag.SourceControl = sourceControl;

            return this.View();
        }

        public ActionResult CreateNewVersion(int id)
        {
            SourceControlVersion sourceControlVersion = this.Entities.SourceControlVersion
                .Include("SourceControl.Properties")
                .Include("Properties")
                .First( sc => sc.Id == id);

            this.ViewBag.SourceControlVersion = sourceControlVersion;

            if (sourceControlVersion.SourceControl.Type == SourceControlType.Svn)
            {
                return this.View("CreateNewVersionSVN");
            }

            if (sourceControlVersion.SourceControl.Type == SourceControlType.FileSystem)
            {
                return this.View("CreateNewVersionFileSystem");
            }
            
            throw new NotSupportedException();
        }

        [HttpPost]
        public ActionResult CreateNewSvnVersion(CreateNewSvnVersion model)
        {
            this.CheckPermission(UserRoleAction.VersionCreate);
            return CreateNewVersionInternal(model, scv => scv.SetStringProperty("URL", model.NewVersionURL.Trim('/')));
        }

        [HttpPost]
        public ActionResult CreateNewFileSystemVersion(CreateNewFileSystemVersion model)
        {
            this.CheckPermission(UserRoleAction.VersionCreate);
            return CreateNewVersionInternal(model, scv => scv.SetStringProperty("Path", model.NewVersionPath.Trim('/')));
        }

        [HttpGet]
        public ActionResult Add(SourceControlType sourceControlType = SourceControlType.Undefined)
        {
            if (sourceControlType == SourceControlType.Undefined)
            {
                return this.View("AddChooseSource");
            }

            this.ViewBag.SourceControlType = sourceControlType;

            return this.View("AddConfigure");
        }

        private ActionResult CreateNewVersionInternal(CreateNewSourceControlVersion model, Action<SourceControlVersion> fillProperties)
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction("CreateNewVersion", new {id = model.FromSourceControlVersionId});
            }

            SourceControlVersion sourceControlVersion = this.Entities.SourceControlVersion
                .Include("SourceControl.Properties")
                .Include("Properties")
                .First(sc => sc.Id == model.FromSourceControlVersionId);

            SourceControlVersion newSourceControlVersion = new SourceControlVersion();

            newSourceControlVersion.SourceControl = sourceControlVersion.SourceControl;
            newSourceControlVersion.Name = model.NewVersionName;

            fillProperties(newSourceControlVersion);

            newSourceControlVersion.ParentSourceControlVersion = sourceControlVersion;

            if (sourceControlVersion.IsHead)
            {
                sourceControlVersion.IsHead = false;
                newSourceControlVersion.IsHead = true;
            }

            this.Entities.SourceControlVersion.Add(newSourceControlVersion);
            this.Entities.SaveChanges();

            return this.RedirectToAction("Details", new {id = sourceControlVersion.SourceControl.Id});
        }

        [HttpPost]
        [ActionName("Add")]
        public ActionResult AddPost(SourceControlType sourceControlType, string url)
        {
            return this.Content(sourceControlType + " "  +url);
        }
    }
}