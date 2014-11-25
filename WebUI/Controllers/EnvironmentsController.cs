using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;

namespace AspNetDeploy.WebUI.Controllers
{
    public class EnvironmentsController : AuthorizedAccessController
    {
        private readonly ITaskRunner taskRunner;
        private readonly ISatelliteMonitor satelliteMonitor;

        public EnvironmentsController(ITaskRunner taskRunner, ISatelliteMonitor satelliteMonitor)
        {
            this.taskRunner = taskRunner;
            this.satelliteMonitor = satelliteMonitor;
        }


        public ActionResult Index()
        {
            List<Environment> environments = this.Entities.Environment
                .Include("Properties")
                .Include("Machines.MachineRoles")
                .ToList();

            Dictionary<Machine, SatelliteState> dictionary = environments.SelectMany(e => e.Machines)
                .Distinct()
                .Select(m => new {m, alive = this.satelliteMonitor.IsAlive(m)})
                .ToDictionary(k => k.m, k => k.alive);

            this.ViewBag.MachineStates = dictionary;
            this.ViewBag.Environments = environments;

            return this.View();

        }

        public ActionResult Details(int id)
        {
            Environment environment = this.Entities.Environment
                .Include("Properties")
                .Include("Machines.MachineRoles")
                .Include("DataFieldValues.DataField")
                .First( e => e.Id == id);

            List<DataField> dataFields = this.Entities.DataField.ToList();

            this.ViewBag.Environment = environment;
            this.ViewBag.DataFields = dataFields;

            return this.View();

        }

        [HttpPost]
        public ActionResult GetMachineStates()
        {
            List<Machine> machines = this.Entities.Machine.ToList();

            var states = machines.Select(
                m => new
                {
                    id = m.Id,
                    state = this.taskRunner.GetMachineState(m.Id).ToString()
                })
                .ToList();

            return this.Json(states, JsonRequestBehavior.AllowGet);
        }
    }
}