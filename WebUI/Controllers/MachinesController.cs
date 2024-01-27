using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;
using AspNetDeploy.WebUI.Models;
using System.Xml;
using System.Xml.Linq;
using Ionic.Zip;
using Ionic.Zlib;
using MachineServices;

namespace AspNetDeploy.WebUI.Controllers
{
    public class MachinesController : AuthorizedAccessController
    {
        private readonly IProjectPackagerFactory _projectPackagerFactory;
        public MachinesController(ILoggingService loggingService, IProjectPackagerFactory projectPackagerFactory) : base(loggingService)
        {
            this._projectPackagerFactory = projectPackagerFactory;
        }

        public ActionResult List()
        {
            List<Machine> machines = this.Entities.Machine
                .Include("MachineRoles")
                .Include("Environments")
                .ToList();

            List<MachineInfo> machinesInfo = machines.Select(m => new MachineInfo()
            {
                Id = m.Id,
                Name = m.Name,
                Url = m.URL,
                Roles = m.MachineRoles.Select(mr => mr.Name).ToList(),
                Environments = m.Environments.Select(env => env.Name).ToList()
            })
                .ToList();

            return View(machinesInfo);
        }

        [HttpGet]
        public ActionResult DownloadMachineInstance(int id)
        {
            Machine machine = this.Entities.Machine.FirstOrDefault(m => m.Id == id);

            if (machine == null)
            {
                return this.RedirectToAction("List");
            }

            ViewBag.Machine = machine;

            return this.View(new MachineInstanceModel()
            {
                Id = id
            });
        }

        [HttpPost]
        public ActionResult DownloadMachineInstance(MachineInstanceModel model)
        {
            Machine machine = this.Entities.Machine.FirstOrDefault(m => m.Id == model.Id);

            if (machine == null)
            {
                return RedirectToAction("List");
            }

            machine.URL = model.Uri;

            MachineInstanceFactory machineInstanceFactory = new MachineInstanceFactory();
            ZipFile machineInstance = machineInstanceFactory.Create(new MachineConfigModel()
            {
                IsAuthorizationEnabled = model.IsAuthorizationEnabled,
                BackupsPath = model.BackupsPath,
                PackagesPath = model.PackagesPath,
                Uri = model.Uri,
            });

            this.Entities.SaveChanges();

            return this.ZipFileResult(machineInstance);
        }

        private XDocument BuildConfigForMachine(MachineInstanceModel model)
        { 
            return new XDocument(
                new XElement("configuration",
                    new XElement("appSettings",
                        new XElement("add", new XAttribute("key", "PackagesPath"), new XAttribute("value", model.PackagesPath)),
                        new XElement("add", new XAttribute("key", "BackupsPath"), new XAttribute("value", model.BackupsPath)),
                        new XElement("add", new XAttribute("key", "Service.URI"), new XAttribute("value", model.Uri)),
                        new XElement("add", new XAttribute("key", "Authorization.Enabled"), new XAttribute("value", model.IsAuthorizationEnabled))
                    )
                )
            );
        }

        private ActionResult ZipFileResult(ZipFile zipFile)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                zipFile.Save(memoryStream);
                memoryStream.Position = 0;

                return File(memoryStream.ToArray(), "application/zip", "machine.zip");
            }
        }
    }
}