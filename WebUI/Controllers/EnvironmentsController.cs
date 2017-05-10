using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            IList<Machine> machines = environments.SelectMany(e => e.Machines).Distinct().ToList();

            Task<Dictionary<Machine, SatelliteState>> isAliveTask = (new TaskFactory<Dictionary<Machine, SatelliteState>>()).StartNew(() =>
            {
                return machines
                    .AsParallel()
                    .Select(m => new { m, alive = this.satelliteMonitor.IsAlive(m) })
                    .ToDictionary(k => k.m, k => k.alive);
            });

            Task<Dictionary<Machine, IServerSummary>> getsummaryTask = (new TaskFactory<Dictionary<Machine, IServerSummary>>()).StartNew(() =>
            {
                return machines
                    .AsParallel()
                    .Select(m => new {m, summary = this.satelliteMonitor.GetServerSummary(m)})
                    .ToDictionary(k => k.m, k => k.summary);
            });

            Task.WaitAll(isAliveTask, getsummaryTask);

            this.ViewBag.MachineStates = isAliveTask.Result;
            this.ViewBag.MachineSummaries = getsummaryTask.Result;
            this.ViewBag.Environments = environments;

            return this.View();

        }

        public ActionResult Details(int id)
        {
            Environment environment = this.Entities.Environment
                .Include("Properties")
                .Include("Machines.MachineRoles")
                .Include("DataFieldValues.DataField")
                .First( df => df.Id == id);

            List<DataField> dataFields = this.Entities.DataField
                .Where( df => !df.IsDeleted)
                .ToList();

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