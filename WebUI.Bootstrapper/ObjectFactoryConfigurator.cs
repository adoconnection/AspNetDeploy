using System.Web.Mvc;
using ObjectFactory;

namespace AspNetDeploy.WebUI.Bootstrapper
{
    public class ObjectFactoryConfigurator
    {
        public static void Configure()
        {
            Factory.SetTypeResolver(new ObjectResolver());
        }

        public static void SetControllerFactory(ControllerBuilder current)
        {
            current.SetControllerFactory(typeof(ControllerFactory)); 
        }
    }
}
