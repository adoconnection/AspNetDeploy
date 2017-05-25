using System.Linq;
using AspNetDeploy.Model;

namespace AspNetDeploy.CommandProcessors.Domain.SourceControlVersions.Commands
{
    public class SourceControlVersionDelete : AppCommandProcessor
    {
        public override string CommandName
        {
            get
            {
                return "App/SourceControlVersions/Delete";
            }
        }

        public override void Process()
        {
            if (!this.HasPermission(UserRoleAction.SourceVersionsManage))
            {
                return;
            }

            int id = this.Data.id;

            SourceControlVersion sourceControlVersion = this.Entities.SourceControlVersion.FirstOrDefault(scv => scv.Id == id && !scv.IsDeleted);

            if (sourceControlVersion == null)
            {
                this.TrnsmitUnableToExecute("SourceControlVersionNotFound", id);
                return;
            }

            sourceControlVersion.IsDeleted = true;
            sourceControlVersion.WorkState = SourceControlVersionWorkState.ArchiveRequired;

            this.Entities.SaveChanges();

            this.TransmitAllUsers(
                "App/SourceControlVersions/Update",
                new
                {
                    id = sourceControlVersion.Id
                });
        }
    }
}