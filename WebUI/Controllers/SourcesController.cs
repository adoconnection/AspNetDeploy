using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Mvc;
using SharpSvn;
using VisualStudioSolutionInfo;
using WebUI.Models;

namespace WebUI.Controllers
{
    public class SourcesController : GenericController
    {
        public ActionResult Index()
        {
            List<SourceControl> sourceControls = this.Entities.SourceControl
                .Include("Projects")
                .Include("Properties")
                .Include("Group")
                .ToList();

            this.ViewBag.SourceControls = sourceControls;

            return this.View();
        }

        public ActionResult Details(int id)
        {
            SourceControl sourceControl = this.Entities.SourceControl
                .Include("Projects")
                .Include("Properties")
                .Include("Group")
                .First( sc => sc.Id == id);

            this.ViewBag.SourceControl = sourceControl;

            return this.View();
        }

        public ActionResult Add(SourceControlType sourceControlType = SourceControlType.Undefined)
        {
            if (sourceControlType == SourceControlType.Undefined)
            {
                return this.View("AddChooseSource");
            }

            this.ViewBag.SourceControlType = sourceControlType;

            return this.View("AddConfigure");
        }
    }
}