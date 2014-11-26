using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AspNetDeploy.Model;
using AspNetDeploy.WebUI.Models;

namespace AspNetDeploy.WebUI.Controllers
{
    public class VariablesController : AuthorizedAccessController
    {
        public ActionResult Details(int id)
        {
            DataField dataField = this.Entities.DataField
                .Include("DataFieldValues.Environments")
                .Include("DataFieldValues.Machines")
                .Include("BundleVersions")
                .First(f => f.Id == id);

            List<Environment> environments = this.Entities.Environment.ToList();

            this.ViewBag.DataField = dataField;
            this.ViewBag.Environments = environments;

            return this.View();
        }

        [HttpGet]
        public ActionResult Edit(int id, int environmentId)
        {
            this.CheckPermission(UserRoleAction.EnvironmentChangeVariables);

            DataField dataField = this.Entities.DataField
                .Include("DataFieldValues.Environments")
                .Include("DataFieldValues.Machines")
                .Include("BundleVersions")
                .First(f => f.Id == id);

            Environment environment = this.Entities.Environment
                .First(e => e.Id == environmentId);

            this.ViewBag.DataField = dataField;
            this.ViewBag.Environment = environment;

            VariableEditModel model = new VariableEditModel()
            {
                Name = dataField.Key,
                Value = dataField.DataFieldValues.Where(v => v.Environments.Any(e => e == environment)).Select(v => v.Value).FirstOrDefault(),
                VariableId = id,
                EnvironmentId = environmentId
            };

            return this.View(model);
        }

        [HttpPost]
        public ActionResult Edit(VariableEditModel model)
        {
            this.CheckPermission(UserRoleAction.EnvironmentChangeVariables);

            DataField dataField = this.Entities.DataField
                    .Include("DataFieldValues.Environments")
                    .Include("DataFieldValues.Machines")
                    .Include("BundleVersions")
                    .First(f => f.Id == model.VariableId);

            Environment environment = this.Entities.Environment
                .First(e => e.Id == model.EnvironmentId);

            if (!this.ModelState.IsValid)
            {
                this.ViewBag.DataField = dataField;
                this.ViewBag.Environment = environment;

                return this.View(model);
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
            value.Value = model.Value;

            this.Entities.SaveChanges();

            return this.RedirectToAction("Details", new {id = model.VariableId});
        }

    }
}