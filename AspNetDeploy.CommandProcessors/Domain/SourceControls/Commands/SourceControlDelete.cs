using System;
using System.Collections.Generic;
using System.Linq;
using AspNetDeploy.Model;
using AspNetDeploy.Notifications;
using AspNetDeploy.Notifications.Model;
using EventHandlers;

namespace AspNetDeploy.CommandProcessors.Domain.SourceControls.Commands
{
    public class SourceControlDelete : AppCommandProcessor
    {
        public override string CommandName
        {
            get
            {
                return "App/SourceControls/Delete";
            }
        }

        public override void Process(AppCommand message)
        {
            Guid userGuid = message.UserGuid;
            int id = message.Data.id;
            string safeWord = message.Data.safeWord;

            if (safeWord != "DELETE")
            {
                this.TrnsmitUnableToExecute(message.ConnectionId, "SafeWordRequired", id);
                return;
            }

            SourceControl sourceControl = this.Entities.SourceControl.FirstOrDefault(sc => sc.Id == id && !sc.IsDeleted);

            if (sourceControl == null)
            {
                this.TrnsmitUnableToExecute(message.ConnectionId, "SourceControlNotFound", id);
                return;
            }

            sourceControl.IsDeleted = true;

            this.Entities.SaveChanges();

            Serializers.SourceControlSerializer serializer = new Serializers.SourceControlSerializer();

            this.TransmitAllUsers(
                "App/SourceControls/Delete",
                serializer.SerializeDeleted(sourceControl));
        }
    }
}