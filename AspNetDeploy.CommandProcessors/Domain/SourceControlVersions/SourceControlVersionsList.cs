using System;
using System.Data.Entity;
using System.Linq;
using AspNetDeploy.Model;
using AspNetDeploy.Notifications;
using AspNetDeploy.Notifications.Model;
using EventHandlers;

namespace AspNetDeploy.CommandProcessors.Domain.SourceControlVersions
{
    public class SourceControlVersionsList : IAppCommandProcessor
    {
        public string CommandName
        {
            get
            {
                return "App/SourceControlVersions/List";
            }
        }

        public void Process(AppCommand message)
        {
            Guid userGuid = message.UserGuid;
            int sourceControlId = message.Data.sourceControlId;
            bool normalOnly = message.Data.normalOnly ?? true;

            AspNetDeployEntities entities = new AspNetDeployEntities();

            IQueryable<SourceControlVersion> query = entities.SourceControlVersion;

            if (normalOnly)
            {
                query = query.Where(scv => scv.ArchiveState == SourceControlVersionArchiveState.Normal);
            }

            query = query
                .OrderByDescending(scv => scv.ArchiveState == SourceControlVersionArchiveState.Normal)
                .ThenByDescending(scv => scv.Name.Length)
                .ThenByDescending(scv => scv.Name);

            EventsHub.TransmitApp.InvokeSafe(new AppConnectionResponse()
            {
                ConnectionId = message.ConnectionId,
                Name = "App/SourceControlVersions/List",
                Data = new 
                {
                    id = sourceControlId,
                    versions = query
                        .Select(scv => new
                        {
                            id = scv.Id,
                            scv.Name,
                            scv.ArchiveState,
                            properties = scv.Properties.Select(scp => new { scp.Key, scp.Value }).ToList()
                        }).ToList()
                }
            });
        }
    }
}