using System;
using System.Collections.Generic;
using System.Linq;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;
using AspNetDeploy.Notifications;
using AspNetDeploy.Notifications.Model;
using AspNetDeploy.SourceControls;
using EventHandlers;

namespace AspNetDeploy.CommandProcessors.Domain.SourceControls.Commands
{
    public class SourceControlAdd : AppCommandProcessor
    {
        public override string CommandName
        {
            get
            {
                return "App/SourceControls/Add";
            }
        }

        public override void Process()
        {
            if (!this.HasPermission(UserRoleAction.SourceVersionsManage))
            {
                return;
            }

            string name = this.Data.name;
            string type = this.Data.type;

            SourceControlType sourceControlType;

            if (!Enum.TryParse(type, true, out sourceControlType))
            {
                this.TrnsmitUnableToExecute("SourceControlInvalidType");
                return;
            }

            AspNetDeployEntities entities = new AspNetDeployEntities();

            SourceControl sourceControl = new SourceControl();
            sourceControl.Name = name;
            sourceControl.Type = sourceControlType;
            sourceControl.IsDeleted = false;
            entities.SourceControl.Add(sourceControl);

            SourceControlModelFactory sourceControlModelFactory = new SourceControlModelFactory();
            ISourceControlModel sourceControlModel = sourceControlModelFactory.Create(sourceControlType);

            sourceControl.Name = name;

            IDictionary<string, string> result = sourceControlModel.PropertyValidator(this.Data);

            if (result.Keys.Count > 0)
            {
                this.TrnsmitUnableToExecute("SourceControlValidationError");
                return;
            }

            sourceControlModel.PropertyUpdater(sourceControl, this.Data);

            entities.SaveChanges();

            this.TransmitAllUsers(
                "App/SourceControls/Update",
                sourceControlModel.ListSerializer(sourceControl));

            this.TransmitConnection(
                "App/SourceControls/Add/Success",
                new
                {
                    id = sourceControl.Id
                });
        }
    }
}