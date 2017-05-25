using System.Linq;
using AspNetDeploy.CommandProcessors.Domain.SourceControlVersions.Serializers;
using AspNetDeploy.Model;

namespace AspNetDeploy.CommandProcessors.Domain.SourceControlVersions.Commands
{
    public class SourceControlVersionsList : AppCommandProcessor
    {
        public override string CommandName
        {
            get
            {
                return "App/SourceControlVersions/List";
            }
        }

        public override void Process()
        {
            if (!this.HasPermission(UserRoleAction.SourceVersionsManage))
            {
                return;
            }

            int id = this.Data.id;
            bool includeArchived = this.Data.includeArchived ?? false;

            SourceControl sourceControl = this.Entities.SourceControl.FirstOrDefault( sc => sc.Id == id && !sc.IsDeleted);

            if (sourceControl == null)
            {
                this.TrnsmitUnableToExecute("SourceControlNotFound", id);
                return;
            }

            IQueryable<SourceControlVersion> query = this.Entities.SourceControlVersion.Where(scv => !scv.IsDeleted);

            if (!includeArchived)
            {
                query = query.Where(scv => scv.WorkState != SourceControlVersionWorkState.Archived);
            }

            query = query
                .OrderByDescending(scv => scv.Name.Length)
                .ThenByDescending(scv => scv.Name);

            this.TransmitConnection(
                "App/SourceControlVersions/List",
                new
                {
                    id,
                    versions = query.Select(SourceControlVersionSerializer.SerializeDetails).ToList()
                });
        }
    }
}