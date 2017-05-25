using System;
using System.Collections.Generic;
using System.Linq;
using AspNetDeploy.CommandProcessors.Domain.SourceControls.Serializers;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;
using AspNetDeploy.SourceControls;

namespace AspNetDeploy.CommandProcessors.Domain.SourceControls.Commands
{
    public class SourceControlUpdate : AppCommandProcessor
    {
        public override string CommandName
        {
            get
            {
                return "App/SourceControls/Update";
            }
        }

        public override void Process()
        {
            if (!this.HasPermission(UserRoleAction.SourceVersionsManage))
            {
                return;
            }

            int id = this.Data.id;
            string name = this.Data.name;
            string type = this.Data.type;

            SourceControl sourceControl = this.Entities.SourceControl.FirstOrDefault(sc => sc.Id == id && !sc.IsDeleted);

            if (sourceControl == null)
            {
                this.TrnsmitUnableToExecute("SourceControlNotFound", id);
                return;
            }

            SourceControlType sourceControlType;

            if (!Enum.TryParse(type, out sourceControlType))
            {
                this.TrnsmitUnableToExecute("SourceControlInvalidType", id);
                return;
            }

            SourceControlModelFactory sourceControlModelFactory = new SourceControlModelFactory();
            ISourceControlModel sourceControlModel = sourceControlModelFactory.Create(sourceControlType);

            sourceControl.Name = name;

            IDictionary<string, string> result = sourceControlModel.PropertyValidator(this.Data);

            if (result.Keys.Count > 0)
            {
                this.TrnsmitUnableToExecute("SourceControlValidationError", id);
                return;
            }

            sourceControlModel.PropertyUpdater(sourceControl, this.Data);

            this.Entities.SaveChanges();

            this.TransmitAllUsers(
                "App/SourceControls/Update",
                sourceControlModel.DetailsSerializer(sourceControl));
        }
    }
}