using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;
using AspNetDeploy.WebUI.Models;

namespace AspNetDeploy.WebUI.Controllers
{
    public class SourcesController : GenericController
    {
        private readonly ITaskRunner taskRunner;

        public SourcesController(ITaskRunner taskRunner)
        {
            this.taskRunner = taskRunner;
        }

        public ActionResult Index()
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

        [HttpPost]
        [ActionName("Add")]
        public ActionResult AddPost(SourceControlType sourceControlType, string url)
        {
            return this.Content(sourceControlType + " "  +url);
        }
    }
}