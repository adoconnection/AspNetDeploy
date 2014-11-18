using ObjectFactory;

namespace SatelliteService.Bootstrapper
{
    public class ObjectFactoryConfigurator
    {
        public static void Configure()
        {
            Factory.SetTypeResolver(new ObjectResolver());
        } 
    }
}