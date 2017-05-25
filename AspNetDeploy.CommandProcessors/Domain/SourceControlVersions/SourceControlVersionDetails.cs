using System.Linq;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;
using AspNetDeploy.SourceControls;

namespace AspNetDeploy.CommandProcessors.Domain.SourceControlVersions.Commands
{
    public class SourceControlVersionDetails : AppCommandProcessor
    {
        public override string CommandName
        {
            get
            {
                return "App/SourceControlVersions/Details";
            }
        }

        public override void Process()
        {
            if (!this.HasPermission(UserRoleAction.SourceVersionsManage))
            {
                return;
            }

            int id = this.Data.id;

            SourceControlVersion sourceControlVersion = this.Entities.SourceControlVersion
                .Include("SourceControl")
                .FirstOrDefault(scv => scv.Id == id && !scv.IsDeleted);

            if (sourceControlVersion == null)
            {
                this.TrnsmitUnableToExecute("SourceControlVersionNotFound", id);
                return;
            }

            SourceControlModelFactory sourceControlModelFactory = new SourceControlModelFactory();
            ISourceControlModel sourceControlModel = sourceControlModelFactory.Create(sourceControlVersion.SourceControl.Type);

            this.TransmitConnection(
                "App/SourceControlVersions/Details",
                sourceControlModel.VersionDetailsSerializer(sourceControlVersion));
        }
    }
}