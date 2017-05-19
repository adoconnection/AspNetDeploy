using System;
using System.Collections.Generic;
using System.Linq;
using AspNetDeploy.Model;
using AspNetDeploy.Notifications;
using AspNetDeploy.Notifications.Model;
using EventHandlers;

namespace AspNetDeploy.CommandProcessors.Domain.SourceControls.Commands
{
    public class SourceControlAddSvn : IAppCommandProcessor
    {
        public string CommandName
        {
            get
            {
                return "App/SourceControls/Add/Svn";
            }
        }

        public void Process(AppCommand message)
        {
            Guid userGuid = message.UserGuid;
            string name = message.Data.name;
            string url = message.Data.url;
            string login = message.Data.login;
            string password = message.Data.password;

            AspNetDeployEntities entities = new AspNetDeployEntities();

            SourceControl sourceControl = new SourceControl();
            sourceControl.Name = name;
            sourceControl.Type = SourceControlType.Svn;
            sourceControl.IsDeleted = false;
            entities.SourceControl.Add(sourceControl);

            sourceControl.SetStringProperty("URL", url);
            sourceControl.SetStringProperty("login", login);
            sourceControl.SetStringProperty("password", password);

            entities.SaveChanges();

            IList<Guid> userGuids = entities.User.Where(u => !u.IsDisabled).Select(u => u.Guid).ToList();

            Serializers.SourceControlSerializer serializer = new Serializers.SourceControlSerializer();

            EventsHub.TransmitApp.InvokeSafe(new AppUsersResponse()
            {
                UserGuids = userGuids,
                Name = "App/SourceControls/Update",
                Data = serializer.SerializeDetails(sourceControl)
            });
        }
    }
}