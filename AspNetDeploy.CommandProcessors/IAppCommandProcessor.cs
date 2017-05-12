using AspNetDeploy.Notifications.Model;

namespace AspNetDeploy.CommandProcessors
{
    public interface IAppCommandProcessor
    {
        string CommandName { get; }
        void Process(AppCommand message);
    }
}