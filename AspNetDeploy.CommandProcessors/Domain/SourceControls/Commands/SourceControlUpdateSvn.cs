using System;
using System.Linq;
using AspNetDeploy.Model;
using AspNetDeploy.Notifications.Model;

namespace AspNetDeploy.CommandProcessors.Domain.SourceControls.Commands
{
    public class SourceControlUpdateSvn : AppCommandProcessor
    {
        public override string CommandName
        {
            get
            {
                return "App/SourceControls/Update/Svn";
            }
        }

        public override void Process(AppCommand message)
        {
            Guid userGuid = message.UserGuid;

            int id = message.Data.id;
            string name = message.Data.name;
            string url = message.Data.url;
            string login = message.Data.login;
            string password = message.Data.password;

            SourceControl sourceControl = this.Entities.SourceControl.FirstOrDefault(sc => sc.Id == id && !sc.IsDeleted);

            if (sourceControl == null)
            {
                this.TrnsmitUnableToExecute(message.ConnectionId, "SourceControlNotFound", id);
                return;
            }

            sourceControl.Name = name;
            sourceControl.SetStringProperty("URL", url);
            sourceControl.SetStringProperty("login", login);
            sourceControl.SetStringProperty("password", password);

            this.Entities.SaveChanges();

            Serializers.SourceControlSerializer serializer = new Serializers.SourceControlSerializer();

            this.TransmitAllUsers(
                "App/SourceControls/Update",
                serializer.SerializeDetails(sourceControl));
        }
    }
}