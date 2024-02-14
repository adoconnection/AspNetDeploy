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

        public IDeploymentAgent Create(Machine machine, Package package)
        {
            IVariableProcessor variableProcessor = this.variableProcessorFactory.Create(package.Id, machine.Id);
            return new WCFSatelliteDeploymentAgent(variableProcessor, machine.URL, machine.Login, machine.Password);
        }
    }
}