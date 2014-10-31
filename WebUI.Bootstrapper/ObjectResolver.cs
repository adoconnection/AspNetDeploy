
using AspNetDeploy.Bootstrapper;
using InnovativeManagementSystems.UnityIocContainer;

namespace AspNetDeploy.WebUI.Bootstrapper
{
    public class ObjectResolver : UnityTypeResolver
    {
        public override void Initialize()
        {
            this.MapRegistry(new RepositoriesRegistry());
        }
    }
}