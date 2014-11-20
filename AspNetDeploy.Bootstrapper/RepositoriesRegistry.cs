using AspNetDeploy.BuildServices;
using AspNetDeploy.Contracts;
using AspNetDeploy.DeploymentServices;
using AspNetDeploy.SolutionParsers;
using AspNetDeploy.SourceControls;
using AspNetDeploy.Variables;
using LocalEnvironment;
using ObjectFactory;
using Packagers;
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
            this.Map<IBuildServiceFactory, BuildServiceFactory>(LifecycleType.Application);
            this.Map<IProjectPackagerFactory, ProjectPackagerFactory>(LifecycleType.Application);
            this.Map<IPathServices, PathServices>(LifecycleType.Application);
            this.Map<IDeploymentAgentFactory, DeploymentAgentFactory>(LifecycleType.Application);
            this.Map<IVariableProcessorFactory, VariableProcessorFactory>();
        }
    }
}