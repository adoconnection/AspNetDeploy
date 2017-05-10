using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;
using AspNetDeploy.WebUI.Models;

namespace AspNetDeploy.WebUI.Controllers
{
    public class VariablesController : AuthorizedAccessController
    {
        public VariablesController(ILoggingService loggingService) : base(loggingService)
        {
        }

        public ActionResult Details(int id)
        {
            DataField dataField = this.Entities.DataField
                .Include("DataFieldValues.Environment.Machines")
                .Include("DataFieldValues.Machine")
                .Include("BundleVersions")
                .First(df => df.Id == id && !df.IsDeleted);

            List<Environment> environments = this.Entities.Environment.ToList();

            this.ViewBag.DataField = dataField;
            this.ViewBag.Environments = environments;

            return this.View();
        }

        [HttpGet]
        public ActionResult Add(int environmentId, int? machineId)
        {
            VariableEditModel model = new VariableEditModel()
            {
                EnvironmentId = environmentId
            };

            Environment environment = this.Entities.Environment.Include("Machines").First(e => e.Id == environmentId);

            if (model.MachineId.HasValue && environment.Machines.All(m => m.Id != model.MachineId))
            {
                return this.Redirect("/");
            }

            this.ViewBag.Environment = environment;

            return this.View(model);
        }

        [HttpGet]
        public ActionResult Edit(int id, int environmentId, int? machineId)
        {
            this.CheckPermission(UserRoleAction.EnvironmentChangeVariables);

            DataField dataField = this.Entities.DataField
                .Include("DataFieldValues.Environment")
                .Include("DataFieldValues.Machine")
                .Include("BundleVersions")
                .First(df => df.Id == id && !df.IsDeleted);

            Environment environment = this.Entities.Environment.First(e => e.Id == environmentId);

            this.ViewBag.DataField = dataField;
            this.ViewBag.Environment = environment;

            VariableEditModel model = new VariableEditModel()
            {
                Name = dataField.Key,
                Value = dataField.DataFieldValues.Where(v => v.EnvironmentId == environment.Id && v.MachineId == machineId).Select(v => v.Value).FirstOrDefault(),
                VariableId = id,
                EnvironmentId = environmentId,
                MachineId = machineId,
                IsSensitive = dataField.IsSensitive
            };

            return this.View(model);
        }

        [HttpPost]
        public ActionResult Save(VariableEditModel model)
        {
            this.CheckPermission(UserRoleAction.EnvironmentChangeVariables);

            DataField dataField;

            if (model.VariableId == 0)
            {
                dataField = new DataField();
                dataField.TypeId = 1;
                dataField.Mode = DataFieldMode.Global;
                dataField.IsDeleted = false;

                this.Entities.DataField.Add(dataField);
            }
            else
            {
                dataField = this.Entities.DataField
                    .Include("DataFieldValues.Environment")
                    .Include("DataFieldValues.Machine")
                    .Include("BundleVersions")
                    .First(df => df.Id == model.VariableId && !df.IsDeleted);
            }

            Environment environment = this.Entities.Environment.Include("Machines").First(e => e.Id == model.EnvironmentId);

            if (model.MachineId.HasValue && environment.Machines.All(m => m.Id != model.MachineId))
            {
                return this.Redirect("/");
            }

            if (!this.ModelState.IsValid)
            {
                this.ViewBag.DataField = dataField;
                this.ViewBag.Environment = environment;

                return this.RedirectToAction("Edit", new { id = model.VariableId, environmentId = model.EnvironmentId });
            }

            DataFieldValue value = dataField.DataFieldValues.FirstOrDefault(v => v.EnvironmentId == environment.Id && v.MachineId == model.MachineId);

            if (value == null)
            {
                value = new DataFieldValue();
                value.DataField = dataField;
                value.Environment = environment;
                value.MachineId = model.MachineId;
                this.Entities.DataFieldValue.Add(value);
            }

            dataField.Key = model.Name;
            dataField.IsSensitive = model.IsSensitive;

            value.Value = model.Value;

            this.Entities.SaveChanges();

            return this.RedirectToAction("Details", new { id = dataField.Id });
        }

    }
}