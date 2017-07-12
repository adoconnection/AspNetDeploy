using AspNetDeploy.BuildServices;
using AspNetDeploy.Contracts;
using AspNetDeploy.Dapper;
using AspNetDeploy.DeploymentServices;
using AspNetDeploy.DeploymentServices.SatelliteMonitoring;
using AspNetDeploy.Logging.DatabaseLogger;
using AspNetDeploy.Projects;
using AspNetDeploy.SourceControls;
using AspNetDeploy.Variables;
using BuildServices.NuGet;
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
            this.Map<IDataContext, DapperDataContext>(LifecycleType.HttpContext);


            this.Map<ISourceControlRepositoryFactory, SourceControlRepositoryFactory>(LifecycleType.Application);
            this.Map<ITaskRunner, ThreadTaskRunner>(LifecycleType.Application);
            this.Map<IBuildServiceFactory, BuildServiceFactory>(LifecycleType.Application);
            this.Map<IProjectPackagerFactory, ProjectPackagerFactory>(LifecycleType.Application);
            this.Map<IPathServices, PathServices>(LifecycleType.Application);
            this.Map<IDeploymentAgentFactory, DeploymentAgentFactory>(LifecycleType.Application);
            this.Map<IVariableProcessorFactory, VariableProcessorFactory>();
            this.Map<ISatelliteMonitor, SatelliteMonitor>();
            this.Map<IProjectParsingService, ProjectParsingService>();
            this.Map<ILoggingService, DatabaseLoggingService>();
            this.Map<IEnvironmentResourcesService, EnvironmentResourcesService>();
            this.Map<INugetPackageManager, NugetPackageManager>();
        }
    }
}