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
    public class SourceControlDetails : AppCommandProcessor
    {
        public override string CommandName
        {
            get
            {
                return "App/SourceControls/Details";
            }
        }

        public override void Process()
        {
            if (!this.HasPermission(UserRoleAction.SourceVersionsManage))
            {
                return;
            }

            int id = this.Data.id;

            SourceControl sourceControl = this.Entities.SourceControl.FirstOrDefault(sc => sc.Id == id && !sc.IsDeleted);

            if (sourceControl == null)
            {
                this.TrnsmitUnableToExecute("SourceControlNotFound", id);
                return;
            }


            SourceControlModelFactory sourceControlModelFactory = new SourceControlModelFactory();
            ISourceControlModel sourceControlModel = sourceControlModelFactory.Create(sourceControl.Type);

            this.Entities.SaveChanges();

            this.TransmitConnection(
                "App/SourceControls/Details",
                sourceControlModel.DetailsSerializer(sourceControl));
        }
    }
}