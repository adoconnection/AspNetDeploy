using AspNetDeploy.Model;
using Environment = AspNetDeploy.Model.Environment;

namespace AspNetDeploy.CommandProcessors.Domain.Environments
{
    public class EnvironmentsAdd : AppCommandProcessor
    {
        public override string CommandName
        {
            get
            {
                return "App/Environments/Add";
            }
        }

        public override void Process()
        {
            if (!this.HasPermission(UserRoleAction.EnvironmentsManage))
            {
                return;
            }

            string name = this.Data.name;

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
                "App/Environments/Add/Success",
                new
                {
                    id = environment.Id
                });
        }
    }
}