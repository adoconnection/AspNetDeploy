namespace AspNetDeploy.CommandProcessors.System
{
    public class UserOnline : IAppCommandProcessor
    {
        public string CommandName
        {
            get
            {
                return "App/User/Online";
            }
        }

        public void Process(AppCommand message)
        {
            ClieneEntities entities = new ClieneEntities();
            User user = entities.Users.Include("ProductEmployees").First(acc => acc.Guid == message.UserGuid);

            user.IsOnline = true;
            entities.SaveChanges();

            Guid[] productGuids = user.ProductEmployees.Select(pe => pe.ProductGuid).Distinct().ToArray();
            IList<Guid> userGuids = entities.ProductEmployees.Where(pe => productGuids.Contains(pe.ProductGuid)).Select(pe => pe.UserGuid).Distinct().ToList();

            EventsHub.TransmitApp.InvokeSafe(new AppUsersResponse()
            {
                UserGuids = userGuids,
                Name = "App/User/Online",
                Data = new
                {
                    guid = user.Guid,
                }
            });
        }
    }
}