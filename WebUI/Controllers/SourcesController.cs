using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using AspNetDeploy.ContinuousIntegration;
using AspNetDeploy.Contracts;
using AspNetDeploy.Dapper;
using AspNetDeploy.Model;
using AspNetDeploy.WebUI.Models;
using AspNetDeploy.WebUI.Models.SourceControls;
using Dapper;
using ObjectFactory;

namespace AspNetDeploy.WebUI.Controllers
{
    public class SourcesController : AuthorizedAccessController
    {
        private readonly ITaskRunner taskRunner;
        private readonly SourceControlManager sourceControlManager;
        private readonly ProjectRepository projectRepository;
        private readonly SourceRepository sourceRepository;

        public SourcesController(ILoggingService loggingService, ITaskRunner taskRunner,
            SourceControlManager sourceControlManager, ProjectRepository projectRepository,
            SourceRepository sourceRepository) : base(loggingService)
        {
            this.taskRunner = taskRunner;
            this.sourceControlManager = sourceControlManager;
            this.projectRepository = projectRepository;
            this.sourceRepository = sourceRepository;
        }

        public ActionResult List()
        {
            IList<SourceControl> sourceControls = sourceRepository.List().ToList();

            var list = sourceControls
                .OrderBy(b => b.OrderIndex)
                .Select(sc => new
                {
                    sourceControl = sc,
                    sourceControlVersions = sc.SourceControlVersions
                        .Where( scv => scv.ArchiveState != SourceControlVersionArchiveState.Archived)
                        .OrderByDescending(scv => scv.Id)
                        .ToList()
                }).ToList();

            IList<ProjectVersion> projectVersions = projectRepository.ListForSources();

            this.ViewBag.SourceControls = list
                .OrderBy(item => item.sourceControl.OrderIndex)
                .Select(
                    item => new SourceControlInfo
                    {
                        SourceControl = item.sourceControl,
                        SourceControlVersionsInfo = item.sourceControlVersions
                            .OrderByDescending(scv => scv.Id)
                            .Select(scv => new SourceControlVersionInfo()
                            {
                                SourceControlVersion = scv,
                                ProjectVersionsInfo = projectVersions.Where(pv => pv.SourceControlVersionId == scv.Id)
                                    .Select(pv =>
                                        new ProjectVersionInfo
                                        {
                                            ProjectVersion = pv,
                                        })
                                    .ToList()
                            })
                            .ToList(),
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
            List<SourceControlVersion> sourceControlVersions =
                this.Entities.SourceControlVersion.AsNoTracking().ToList();

            var states = sourceControlVersions.Select(
                    scv => new
                    {
                        id = scv.Id,
                        state = this.taskRunner.GetSourceControlVersionState(scv.Id).ToString(),
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
                .First(sc => sc.Id == id);

            this.ViewBag.SourceControl = sourceControl;

            return this.View();
        }

        public ActionResult ArchiveVersion(int id)
        {
            this.CheckPermission(UserRoleAction.SourceVersionsManage);

            SourceControlVersion sourceControlVersion = this.Entities.SourceControlVersion
                .Include("Properties")
                .Include("ProjectVersions.BundleVersions")
                .First(sc => sc.Id == id);

            if (sourceControlVersion.ArchiveState == SourceControlVersionArchiveState.Archived ||
                sourceControlVersion.ArchiveState == SourceControlVersionArchiveState.Archiving)
            {
                return this.RedirectToAction("Details", new {id = sourceControlVersion.SourceControl.Id});
            }

            sourceControlVersion.ArchiveState = SourceControlVersionArchiveState.Archiving;
            this.Entities.SaveChanges();

            return this.RedirectToAction("Details", new {id = sourceControlVersion.SourceControl.Id});
        }

        public ActionResult CreateNewVersion(int id)
        {
            this.CheckPermission(UserRoleAction.SourceVersionsManage);

            SourceControlVersion sourceControlVersion = this.Entities.SourceControlVersion
                .Include("SourceControl.Properties")
                .Include("Properties")
                .First(sc => sc.Id == id);

            this.ViewBag.SourceControlVersion = sourceControlVersion;

            if (sourceControlVersion.SourceControl.Type == SourceControlType.Svn)
            {
                return this.View("CreateNewVersionSVN");
            }

            if (sourceControlVersion.SourceControl.Type == SourceControlType.Git)
            {
                return this.View("CreateNewVersionGit");
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
            this.CheckPermission(UserRoleAction.SourceVersionsManage);

            this.CheckPermission(UserRoleAction.VersionCreate);
            return CreateNewVersionInternal(model, scv => scv.SetStringProperty("URL", model.NewVersionURL.Trim('/')));
        }

        [HttpPost]
        public ActionResult CreateNewGitVersion(CreateNewGitVersion model)
        {
            this.CheckPermission(UserRoleAction.SourceVersionsManage);

            this.CheckPermission(UserRoleAction.VersionCreate);
            return CreateNewVersionInternal(model, scv => scv.SetStringProperty("Branch", model.NewVersionBranch.Trim('/')));
        }

        [HttpPost]
        public ActionResult CreateNewFileSystemVersion(CreateNewFileSystemVersion model)
        {
            this.CheckPermission(UserRoleAction.SourceVersionsManage);

            this.CheckPermission(UserRoleAction.VersionCreate);
            return CreateNewVersionInternal(model, scv => scv.SetStringProperty("Path", model.NewVersionPath.Trim('/')));
        }

        [HttpGet]
        public ActionResult Add()
        {
            this.CheckPermission(UserRoleAction.SourceVersionsManage);

            return this.View();
        }

        [HttpGet]
        public ActionResult AddSvn()
        {
            this.CheckPermission(UserRoleAction.SourceVersionsManage);

            return this.View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSvn(AddSvnModel model)
        {
            this.CheckPermission(UserRoleAction.SourceVersionsManage);

            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            SourceControl sourceControl = new SourceControl
            {
                Type = SourceControlType.Svn,
                Name = model.Name,
                IsDeleted = false,
                OrderIndex = this.Entities.SourceControl.Count()
            };

            sourceControl.SetStringProperty("URL", model.Url.Trim());
            sourceControl.SetStringProperty("Login", model.Login.Trim());
            sourceControl.SetStringProperty("Password", model.Password.Trim());

            SourceControlVersion sourceControlVersion = new SourceControlVersion();
            sourceControlVersion.SourceControl = sourceControl;
            sourceControlVersion.SetStringProperty("URL", "/");

            TestSourceResult testSourceResult = this.sourceControlManager.TestConnection(sourceControlVersion);

            if (!testSourceResult.IsSuccess)
            {
                this.ModelState.AddModelError("URL", testSourceResult.ErrorMessage);
                return this.View(model);
            }

            sourceControlVersion.SourceControl = null;

            this.Entities.SourceControl.Add(sourceControl);
            this.Entities.SaveChanges();

            return this.RedirectToAction("List");
        }

        [HttpGet]
        public ActionResult AddGit()
        {
            this.CheckPermission(UserRoleAction.SourceVersionsManage);

            return this.View();
        }

        [HttpPost]
        public ActionResult AddGit(AddGitModel model)
        {
            this.CheckPermission(UserRoleAction.SourceVersionsManage);

            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            SourceControl sourceControl = new SourceControl
            {
                Type = SourceControlType.Git,
                Name = model.Name,
                IsDeleted = false,
                OrderIndex = this.Entities.SourceControl.Count()
            };

            sourceControl.SetStringProperty("URL", model.Url.Trim());
            sourceControl.SetStringProperty("Login", model.Login.Trim());
            sourceControl.SetStringProperty("AccessToken", model.AccessToken.Trim());

            SourceControlVersion sourceControlVersion = new SourceControlVersion();
            sourceControlVersion.SourceControl = sourceControl;
            sourceControlVersion.SetStringProperty("URL", "/");

            TestSourceResult testSourceResult = this.sourceControlManager.TestConnection(sourceControlVersion);

            if (!testSourceResult.IsSuccess)
            {
                this.ModelState.AddModelError("URL", testSourceResult.ErrorMessage);
                return this.View(model);
            }

            sourceControlVersion.SourceControl = null;

            this.Entities.SourceControl.Add(sourceControl);
            this.Entities.SaveChanges();

            return this.RedirectToAction("List");
        }

        private ActionResult CreateNewVersionInternal(CreateNewSourceControlVersion model,
            Action<SourceControlVersion> fillProperties)
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
            newSourceControlVersion.ArchiveState = SourceControlVersionArchiveState.Normal;

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
            return this.Content(sourceControlType + " " + url);
        }
    }
}