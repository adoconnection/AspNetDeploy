using AspNetDeploy.Notifications.Model;

namespace AspNetDeploy.CommandProcessors.Users
{
    public class ActiveUserProfile : AppCommandProcessor
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
            ClieneEntities entities = new ClieneEntities();
            User user = entities.Users.First(acc => acc.Guid == message.UserGuid);

            EventsHub.TransmitApp.InvokeSafe(new AppConnectionResponse
            {
                ConnectionId = message.ConnectionId,
                Name = "App/ActiveUser/Info",
                Data = new
                {
                    guid = user.Guid,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    displayName = user.DisplayName
                }
            });
        }
    }
}