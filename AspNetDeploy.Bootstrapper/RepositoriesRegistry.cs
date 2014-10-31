using AspNetDeploy.Contracts;
using AspNetDeploy.SolutionParsers;
using AspNetDeploy.SourceControls;
using ObjectFactory;
using ThreadHostedTaskRunner;

namespace AspNetDeploy.Bootstrapper
{
    public class RepositoriesRegistry : Registry
    {
        public RepositoriesRegistry()
        {
            this.Map<ISourceControlRepositoryFactory, SourceControlRepositoryFactory>(LifecycleType.Application);
            this.Map<ISolutionParsersFactory, SolutionParsersFactory>(LifecycleType.Application);
            this.Map<ITaskRunner, ThreadTaskRunner>(LifecycleType.Application);
        }
    }
}