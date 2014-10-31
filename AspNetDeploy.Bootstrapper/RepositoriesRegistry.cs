using AspNetDeploy.Contracts;
using AspNetDeploy.SourceControls;
using ObjectFactory;

namespace AspNetDeploy.Bootstrapper
{
    public class RepositoriesRegistry : Registry
    {
        public RepositoriesRegistry()
        {
            this.Map<ISourceControlRepositoryFactory, SourceControlRepositoryFactory>(LifecycleType.Application);
        }
    }
}