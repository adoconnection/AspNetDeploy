using System;
using System.Collections.Generic;
using System.Linq;
using AspNetDeploy.Model;
using AspNetDeploy.Notifications;
using AspNetDeploy.Notifications.Model;
using EventHandlers;

namespace AspNetDeploy.CommandProcessors
{
    public abstract class AppCommandProcessor : IAppCommandProcessor
    {
        public AspNetDeployEntities Entities { get; } = new AspNetDeployEntities();

        public abstract string CommandName { get; }
        public abstract void Process(AppCommand message);

        protected void TransmitAllUsers(string name, object data)
        {
            IList<Guid> userGuids = this.Entities.User.Where(u => !u.IsDisabled).Select(u => u.Guid).ToList();

            EventsHub.TransmitApp.InvokeSafe(new AppUsersResponse()
            {
                UserGuids = userGuids,
                Name = name,
                Data = data
            });
        }

        protected void TrnsmitUnableToExecute(string connectionId, string type, int? id = null)
        {
            EventsHub.TransmitApp.InvokeSafe(new AppConnectionResponse()
            {
                ConnectionId = connectionId,
                Name = "App/Error/UnableToRunCommand",
                Data = new
                {
                    command = this.CommandName,
                    type,
                    id
                }
            });
        }
    }
}