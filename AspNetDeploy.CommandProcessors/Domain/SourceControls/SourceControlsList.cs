using System;
using System.Linq;
using AspNetDeploy.Model;
using AspNetDeploy.Notifications;
using AspNetDeploy.Notifications.Model;
using EventHandlers;

namespace AspNetDeploy.CommandProcessors.Domain.SourceControls
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

            EventsHub.TransmitApp.InvokeSafe(new AppConnectionResponse()
            {
                ConnectionId = message.ConnectionId,
                Name = "App/SourceControls/List",
                Data = entities.SourceControl
                    .Where( sc => !sc.IsDeleted)
                    .Select(sc => new
                    {
                        id = sc.Id,
                        sc.Name,
                        properties = sc.Properties.Select( scp => new { scp.Key, scp.Value }).ToList()
                    }).ToList()
                });
        }
    }
}