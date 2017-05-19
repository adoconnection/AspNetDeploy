
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(AspNetDeploy.WebUI.Startup))]
namespace AspNetDeploy.WebUI
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}