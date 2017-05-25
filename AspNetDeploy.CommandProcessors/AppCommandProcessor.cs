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
        public abstract string CommandName { get; }
        public abstract void Process();

        private string connectionId;

        protected dynamic Data { get; private set; }
        protected User User { get; private set; }

        public AspNetDeployEntities Entities { get; } = new AspNetDeployEntities();

        public void Process(AppCommand message)
        {
            this.User = this.Entities.User.FirstOrDefault( u => u.Guid == message.UserGuid);
            this.connectionId = message.ConnectionId;
            this.Data = message.Data;

            this.Process();
        }

        protected bool HasPermission(UserRoleAction roleAction)
        {
            return RolePermissions.MappingDictionary[this.User.Role].Contains(roleAction);
        }

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

        protected void TransmitConnection(string name, object data)
        {
            EventsHub.TransmitApp.InvokeSafe(new AppConnectionResponse()
            {
                ConnectionId = this.connectionId,
                Name = name,
                Data = data
            });
        }

        protected void TrnsmitUnableToExecute(string type, int? id = null)
        {
            EventsHub.TransmitApp.InvokeSafe(new AppConnectionResponse()
            {
                ConnectionId = this.connectionId,
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