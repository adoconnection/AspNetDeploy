using InnovativeManagementSystems.UnityIocContainer;

namespace SatelliteService.Bootstrapper
{
    public class ObjectResolver : UnityTypeResolver
    {
        public override void Initialize()
        {
            this.MapRegistry(new PrimaryRegistry());
        }
    }
}