using System;
using System.Linq;
using AspNetDeploy.Model;
using AspNetDeploy.Notifications;
using AspNetDeploy.Notifications.Model;
using EventHandlers;

namespace AspNetDeploy.CommandProcessors.Domain.SourceControls.Commands
{
    public class SourceControlsList : IAppCommandProcessor
    {
        public string CommandName
        {
            get
            {
                return "App/SourceControls/List";
            }
        }

        public void Process(AppCommand message)
        {
            Guid userGuid = message.UserGuid;

            AspNetDeployEntities entities = new AspNetDeployEntities();
            Serializers.SourceControlSerializer serializer = new Serializers.SourceControlSerializer();
            

            EventsHub.TransmitApp.InvokeSafe(new AppConnectionResponse()
            {
                ConnectionId = message.ConnectionId,
                Name = "App/SourceControls/List",
                Data = entities.SourceControl
                    .Where( sc => !sc.IsDeleted)
                    .ToList()
                    .Select(sc => serializer.SerializeDetails(sc))
                    .ToList()
                });
        }
    }
}