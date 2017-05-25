using System.Linq;
using AspNetDeploy.Model;

namespace AspNetDeploy.CommandProcessors.Domain.SourceControlVersions
{
    public class SourceControlVersionArchive : AppCommandProcessor
    {
        public override string CommandName
        {
            get
            {
                return "App/SourceControlVersions/Archive";
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