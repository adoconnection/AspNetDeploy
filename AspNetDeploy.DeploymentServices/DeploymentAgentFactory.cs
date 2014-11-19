using AspNetDeploy.Contracts;
using AspNetDeploy.DeploymentServices.WCFSatellite;
using AspNetDeploy.Model;

namespace AspNetDeploy.DeploymentServices
{
    public class DeploymentAgentFactory : IDeploymentAgentFactory
    {
        public IDeploymentAgent Create(Machine machine)
        {
            return new WCFSatelliteDeploymentAgent();
        }
    }
}