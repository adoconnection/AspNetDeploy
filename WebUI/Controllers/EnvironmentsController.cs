using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AspNetDeploy.Model;

namespace AspNetDeploy.WebUI.Controllers
{
    public class EnvironmentsController : GenericController
    {
        public ActionResult Index()
        {
            List<Environment> environments = this.Entities.Environment
                .Include("Properties")
                .Include("Machines.MachineRoles")
                .ToList();

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
    }
}