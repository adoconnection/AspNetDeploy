using System.Linq;
using AspNetDeploy.Model;
using AspNetDeploy.Notifications;
using AspNetDeploy.Notifications.Model;
using EventHandlers;

namespace AspNetDeploy.CommandProcessors.Users
{
    public class ActiveUserProfile : IAppCommandProcessor
    {
        public string CommandName
        {
            get
            {
                return "App/ActiveUser/Info";
            }
        }

        public void Process(AppCommand message)
        {
            AspNetDeployEntities entities = new AspNetDeployEntities();
            User user = entities.User.First(acc => acc.Guid == message.UserGuid);

            EventsHub.TransmitApp.InvokeSafe(new AppConnectionResponse
            {
                ConnectionId = message.ConnectionId,
                Name = "App/ActiveUser/Info",
                Data = new
                {
                    guid = user.Guid,
                    name = user.Name,
                }
            });
        }
    }
}