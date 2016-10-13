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
                .Include("DataFieldValues.Environments")
                .Include("DataFieldValues.Machines")
                .Include("BundleVersions")
                .First(df => df.Id == id && !df.IsDeleted);

            List<Environment> environments = this.Entities.Environment.ToList();

            this.ViewBag.DataField = dataField;
            this.ViewBag.Environments = environments;

            return this.View();
        }

        [HttpGet]
        public ActionResult Add(int environmentId)
        {
            VariableEditModel model = new VariableEditModel()
            {
                EnvironmentId = environmentId
            };

            Environment environment = this.Entities.Environment
                .First(e => e.Id == environmentId);

            this.ViewBag.Environment = environment;

            return this.View(model);
        }

        [HttpGet]
        public ActionResult Edit(int id, int environmentId)
        {
            this.CheckPermission(UserRoleAction.EnvironmentChangeVariables);

            DataField dataField = this.Entities.DataField
                .Include("DataFieldValues.Environments")
                .Include("DataFieldValues.Machines")
                .Include("BundleVersions")
                .First(df => df.Id == id && !df.IsDeleted);

            Environment environment = this.Entities.Environment
                .First(e => e.Id == environmentId);

            this.ViewBag.DataField = dataField;
            this.ViewBag.Environment = environment;

            VariableEditModel model = new VariableEditModel()
            {
                Name = dataField.Key,
                Value = dataField.DataFieldValues.Where(v => v.Environments.Any(e => e == environment)).Select(v => v.Value).FirstOrDefault(),
                VariableId = id,
                EnvironmentId = environmentId,
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
                    .Include("DataFieldValues.Environments")
                    .Include("DataFieldValues.Machines")
                    .Include("BundleVersions")
                    .First(df => df.Id == model.VariableId && !df.IsDeleted);
            }

            Environment environment = this.Entities.Environment
                .First(e => e.Id == model.EnvironmentId);

            if (!this.ModelState.IsValid)
            {
                this.ViewBag.DataField = dataField;
                this.ViewBag.Environment = environment;

                return this.RedirectToAction("Edit", new { id = model.VariableId, environmentId = model.EnvironmentId });
            }

            DataFieldValue value = dataField.DataFieldValues.FirstOrDefault(v => v.Environments.Any(e => e == environment));

            if (value == null)
            {
                value = new DataFieldValue();
                value.DataField = dataField;
                value.Environments.Add(environment);
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