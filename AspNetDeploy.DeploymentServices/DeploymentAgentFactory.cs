using AspNetDeploy.Contracts;
using AspNetDeploy.DeploymentServices.WCFSatellite;
using AspNetDeploy.Model;

namespace AspNetDeploy.DeploymentServices
{
    public class DeploymentAgentFactory : IDeploymentAgentFactory
    {
        private readonly IVariableProcessorFactory variableProcessorFactory;

        public DeploymentAgentFactory(IVariableProcessorFactory variableProcessorFactory)
        {
            this.variableProcessorFactory = variableProcessorFactory;
        }

        public IDeploymentAgent Create(Machine machine, BundleVersion bundleVersion)
        {
            IVariableProcessor variableProcessor = this.variableProcessorFactory.Create(bundleVersion.Id, machine.Id);
            return new WCFSatelliteDeploymentAgent(variableProcessor, machine.URL);
        }
    }
}