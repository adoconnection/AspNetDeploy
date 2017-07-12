using System.Linq;
using AspNetDeploy.Model;

namespace AspNetDeploy.CommandProcessors.Domain.Environments
{
    public class EnvironmentsUpdate : AppCommandProcessor
    {
        public override string CommandName
        {
            get
            {
                return "App/Environments/Update";
            }
        }

        public override void Process()
        {
            if (!this.HasPermission(UserRoleAction.EnvironmentsManage))
            {
                return;
            }

            int id = this.Data.id;
            string name = this.Data.name;


            SourceControl sourceControl = this.Entities.Environment.FirstOrDefault(e => e.Id == id);

            if (sourceControl == null)
            {
                this.TrnsmitUnableToExecute("SourceControlNotFound", id);
                return;
            }

            Environment environment = new Environment();
            environment.Name = name;
            this.Entities.Environment.Add(environment);

            this.Entities.SaveChanges();

            this.TransmitAllUsers(
                "App/Environments/Update",
                new
                {
                    id = environment.Id,
                    name = environment.Name
                });

            this.TransmitConnection(
                "App/Environments/Update/Success",
                new
                {
                    id = environment.Id
                });
        }
    }
}