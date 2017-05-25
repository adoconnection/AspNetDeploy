using System.Collections.Generic;
using System.Linq;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;
using AspNetDeploy.SourceControls;

namespace AspNetDeploy.CommandProcessors.Domain.SourceControlVersions.Commands
{
    public class SourceControlVersionUpdate : AppCommandProcessor
    {
        public override string CommandName
        {
            get
            {
                return "App/SourceControlVersions/Update";
            }
        }

        public override void Process()
        {
            if (!this.HasPermission(UserRoleAction.SourceVersionsManage))
            {
                return;
            }

            int id = this.Data.id;
            int? parentId = this.Data.parentId;
            string name = this.Data.name;

            SourceControlVersion sourceControlVersion = this.Entities.SourceControlVersion.Include("SourceControl").FirstOrDefault(scv => scv.Id == id && !scv.IsDeleted && !scv.SourceControl.IsDeleted);

            if (sourceControlVersion == null)
            {
                this.TrnsmitUnableToExecute("SourceControlVersionNotFound", id);
                return;
            }

            SourceControlVersion parentSourceControlVersion = null;

            if (parentId.HasValue)
            {
                parentSourceControlVersion = this.Entities.SourceControlVersion.FirstOrDefault(scv => scv.Id == parentId.Value && !scv.IsDeleted && !scv.SourceControl.IsDeleted);

                if (parentSourceControlVersion == null)
                {
                    this.TrnsmitUnableToExecute("SourceControlVersionNotFound", parentId.Value);
                    return;
                }
            }

            
            sourceControlVersion.Name = name;
            sourceControlVersion.ParentSourceControlVersion = parentSourceControlVersion;

            SourceControlModelFactory sourceControlModelFactory = new SourceControlModelFactory();
            ISourceControlModel sourceControlModel = sourceControlModelFactory.Create(sourceControlVersion.SourceControl.Type);

            IDictionary<string, string> result = sourceControlModel.VersionPropertyValidator(this.Data);

            if (result.Keys.Count > 0)
            {
                this.TrnsmitUnableToExecute("SourceControlVersionValidationError");
                return;
            }

            this.Entities.SaveChanges();

            this.TransmitAllUsers(
                "App/SourceControlVersions/Update",
                sourceControlModel.VersionListSerializer(sourceControlVersion));
        }
    }
}