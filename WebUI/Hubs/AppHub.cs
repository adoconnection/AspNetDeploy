using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetDeploy.CommandProcessors;
using AspNetDeploy.CommandProcessors.Domain.SourceControls;
using AspNetDeploy.CommandProcessors.Domain.SourceControls.Commands;
using AspNetDeploy.CommandProcessors.Domain.SourceControlVersions;
using AspNetDeploy.CommandProcessors.System;
using AspNetDeploy.CommandProcessors.Users;
using AspNetDeploy.Model;
using AspNetDeploy.Notifications.Model;
using Microsoft.AspNet.SignalR;

namespace AspNetDeploy.WebUI.Hubs
{
    [Authorize]
    public class AppHub : Hub
    {
        public static readonly ConcurrentDictionary<string, HashSet<string>> ConnectedUsers = new ConcurrentDictionary<string, HashSet<string>>();

        private static readonly IList<IAppCommandProcessor> UICommandProcessors = new List<IAppCommandProcessor>()
        {
            new ActiveUserProfile(),

            new SourceControlsList(),
            new SourceControlTypes(),
            new SourceControlAddSvn(),
            new SourceControlUpdateSvn(),
            new SourceControlDelete(),

            new SourceControlVersionsList()
        };

        public void Send(dynamic message)
        {
            Guid? userGuid = this.GetUserGuid();

            if (userGuid == null)
            {
                return;
            }

            foreach (IAppCommandProcessor commandProcessor in UICommandProcessors)
            {
                if (!commandProcessor.CommandName.Equals((string)message.name, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                commandProcessor.Process(new AppCommand
                {
                    UserGuid = userGuid.Value,
                    ConnectionId = this.Context.ConnectionId,
                    Data = message.data,
                    Name = message.name
                });

                return;
            }
        }


        public override Task OnDisconnected(bool stopCalled)
        {
            Guid? userGuid = this.GetUserGuid();

            if (userGuid == null)
            {
                return base.OnDisconnected(stopCalled);
            }

            HashSet<string> connections = ConnectedUsers.GetOrAdd(this.Context.User.Identity.Name, _ => new HashSet<string>());

            string connectionId = this.Context.ConnectionId;

            lock (connections)
            {
                connections.Remove(connectionId);

                if (connections.Count == 0)
                {
                    new UserOffline().Process(new AppCommand
                    {
                        UserGuid = userGuid.Value,
                        ConnectionId = this.Context.ConnectionId
                    });
                }
            }

            return base.OnDisconnected(stopCalled);
        }

        public override Task OnConnected()
        {
            Guid? userGuid = this.GetUserGuid();

            if (userGuid == null)
            {
                return base.OnConnected();
            }

            AspNetDeployEntities entities = new AspNetDeployEntities();
            User user = entities.User.FirstOrDefault(u => u.Guid == userGuid.Value);

            if (user == null)
            {
                return base.OnConnected();
            }

            string connectionId = this.Context.ConnectionId;

            HashSet<string> connections = ConnectedUsers.GetOrAdd(this.Context.User.Identity.Name, valueFactory => new HashSet<string>());

            lock (connections)
            {
                connections.Add(connectionId);

                if (connections.Count == 1)
                {
                    new UserOnline().Process(new AppCommand
                    {
                        UserGuid = userGuid.Value,
                        ConnectionId = this.Context.ConnectionId
                    });
                }
            }

            return base.OnConnected();
        }


        private Guid? GetUserGuid()
        {
            Guid result;

            if (Guid.TryParse(this.Context.User.Identity.Name, out result))
            {
                return result;
            }

            return null;
        }
    }
}