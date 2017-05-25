using System.Collections.Generic;
using System.Linq;
using AspNetDeploy.Model;
using AspNetDeploy.SourceControls;

namespace AspNetDeploy.CommandProcessors.Domain.SourceControlVersions
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

            int sourceControlId = this.Data.sourceControlId;
            bool includeArchived = this.Data.includeArchived ?? false;

            SourceControl sourceControl = this.Entities.SourceControl.FirstOrDefault( sc => sc.Id == sourceControlId && !sc.IsDeleted);

            if (sourceControl == null)
            {
                this.TrnsmitUnableToExecute("SourceControlNotFound", sourceControlId);
                return;
            }

            IQueryable<SourceControlVersion> query = this.Entities.SourceControlVersion
                .Include("SourceControl")
                .Where(scv => !scv.IsDeleted && !scv.SourceControl.IsDeleted);

            if (!includeArchived)
            {
                query = query.Where(scv => scv.WorkState != SourceControlVersionWorkState.Archived);
            }

            query = query
                .OrderByDescending(scv => scv.Name.Length)
                .ThenByDescending(scv => scv.Name);

            List<SourceControlVersion> sourceControlVersions = query.ToList();

            SourceControlModelFactory sourceControlModelFactory = new SourceControlModelFactory();

            this.TransmitConnection(
                "App/SourceControlVersions/List",
                new
                {
                    id = sourceControlId,
                    versions = sourceControlVersions.Select(scv => sourceControlModelFactory.Create(scv.SourceControl.Type).VersionListSerializer(scv)).ToList()
                });
        }
    }
}