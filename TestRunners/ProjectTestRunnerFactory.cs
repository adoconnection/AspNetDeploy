using AspNetDeploy.Contracts;
using AspNetDeploy.Contracts.Exceptions;
using AspNetDeploy.Model;
using TestRunners.VsTests;

namespace TestRunners
{
    public class ProjectTestRunnerFactory : IProjectTestRunnerFactory
    {
        private readonly IPathServices pathServices;

        public ProjectTestRunnerFactory(IPathServices pathServices)
        {
            this.pathServices = pathServices;
        }

        public IProjectTestRunner Create(ProjectType projectType, IVariableProcessor variableProcessor)
        {
            if (projectType.HasFlag(ProjectType.Test))
            {
                return new VisualStudioTestRunner(variableProcessor, this.pathServices);
            }

            throw new AspNetDeployException("Project type is not supported: " + projectType);
        }
    }
}