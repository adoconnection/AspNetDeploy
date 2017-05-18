using System;
using System.Collections.Generic;
using System.Linq;
using AspNetDeploy.Model;
using AspNetDeploy.Notifications;
using AspNetDeploy.Notifications.Model;
using EventHandlers;

namespace AspNetDeploy.CommandProcessors.System
{
    public class UserOffline : IAppCommandProcessor
    {
        public string CommandName
        {
            get
            {
                return "App/User/Offline";
            }
        }

        public void Process(AppCommand message)
        {
            AspNetDeployEntities entities = new AspNetDeployEntities();
            User user = entities.User.First(acc => acc.Guid == message.UserGuid);

            //user.IsOnline = false;
            entities.SaveChanges();

            IList<Guid> userGuids = entities.User.Where(u => !u.IsDisabled).Select(u => u.Guid).ToList();

            EventsHub.TransmitApp.InvokeSafe(new AppUsersResponse()
            {
                UserGuids = userGuids,
                Name = "App/User/Offline",
                Data = new
                {
                    guid = user.Guid,
                }
            });
        }
    }
}