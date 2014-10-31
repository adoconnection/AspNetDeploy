using ObjectFactory;

namespace AspNetDeploy.WebUI.Bootstrapper
{
    public class ObjectFactoryConfigurator
    {
        public static void Configure()
        {
            Factory.SetTypeResolver(new ObjectResolver());
        }
    }
}
