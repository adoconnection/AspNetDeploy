using System.Collections.Generic;
using System.Linq;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;
using AspNetDeploy.SourceControls;

namespace AspNetDeploy.CommandProcessors.Domain.SourceControlVersions.Commands
{
    public class SourceControlVersionAdd : AppCommandProcessor
    {
        public override string CommandName
        {
            get
            {
                return "App/SourceControlVersions/Add";
            }
        }

        public override void Process()
        {
            if (!this.HasPermission(UserRoleAction.SourceVersionsManage))
            {
                return;
            }

            int sourceControlId = this.Data.sourceControlId;
            int? parentId = this.Data.parentId;
            string name = this.Data.name;

            SourceControl sourceControl = this.Entities.SourceControl.FirstOrDefault(sc => sc.Id == sourceControlId && !sc.IsDeleted);

            if (sourceControl == null)
            {
                this.TrnsmitUnableToExecute("SourceControlNotFound", sourceControlId);
                return;
            }

            SourceControlVersion parentSourceControlVersion = null;

            if (parentId.HasValue)
            {
                parentSourceControlVersion = this.Entities.SourceControlVersion.FirstOrDefault(scv => scv.Id == parentId.Value && !scv.IsDeleted);

                if (parentSourceControlVersion == null)
                {
                    this.TrnsmitUnableToExecute("SourceControlVersionNotFound", parentId.Value);
                    return;
                }
            }

            SourceControlModelFactory sourceControlModelFactory = new SourceControlModelFactory();
            ISourceControlModel sourceControlModel = sourceControlModelFactory.Create(sourceControl.Type);

            IDictionary<string, string> result = sourceControlModel.VersionPropertyValidator(this.Data);

            if (result.Keys.Count > 0)
            {
                this.TrnsmitUnableToExecute("SourceControlVersionValidationError");
                return;
            }

            SourceControlVersion sourceControlVersion = new SourceControlVersion();
            sourceControlVersion.SourceControl = sourceControl;
            sourceControlVersion.WorkState = SourceControlVersionWorkState.LoadRequired;
            sourceControlVersion.Name = name;
            sourceControlVersion.ParentSourceControlVersion = parentSourceControlVersion;
            sourceControlVersion.IsDeleted = false;

            this.Entities.SourceControlVersion.Add(sourceControlVersion);

            sourceControlModel.VersionPropertyUpdater(sourceControlVersion, this.Data);

            this.Entities.SaveChanges();

            this.TransmitAllUsers(
                "App/SourceControlVersions/Update",
                sourceControlModel.VersionListSerializer(sourceControlVersion));

            this.TransmitConnection(
                "App/SourceControlVersions/Add/Success",
                new
                {
                    id = sourceControlVersion.Id
                });
        }
    }
}