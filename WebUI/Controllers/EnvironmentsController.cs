using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AspNetDeploy.Contracts;
using AspNetDeploy.Contracts.MachineSummary;
using AspNetDeploy.Model;

namespace AspNetDeploy.WebUI.Controllers
{
    public class EnvironmentsController : AuthorizedAccessController
    {
        private readonly ITaskRunner taskRunner;
        private readonly ISatelliteMonitor satelliteMonitor;

        public EnvironmentsController(ILoggingService loggingService, ITaskRunner taskRunner, ISatelliteMonitor satelliteMonitor) : base(loggingService)
        {
            this.taskRunner = taskRunner;
            this.satelliteMonitor = satelliteMonitor;
        }

        public ActionResult List()
        {
            List<Environment> environments = this.Entities.Environment
                .Include("Properties")
                .Include("Machines.MachineRoles")
                .ToList();

            Dictionary<Machine, SatelliteState> dictionary = environments.SelectMany(e => e.Machines)
                .Distinct()
                .AsParallel()
                .Select(m => new {m, alive = this.satelliteMonitor.IsAlive(m)})
                .ToDictionary(k => k.m, k => k.alive);

            Dictionary<Machine, IServerSummary> summaries = environments.SelectMany(e => e.Machines)
                .Distinct()
                .AsParallel()
                .Select(m => new { m, summary = dictionary[m] == SatelliteState.Alive ? this.satelliteMonitor.GetServerSummary(m) : null })
                .ToDictionary(k => k.m, k => k.summary);

            this.ViewBag.MachineStates = dictionary;
            this.ViewBag.MachineSummaries = summaries;
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