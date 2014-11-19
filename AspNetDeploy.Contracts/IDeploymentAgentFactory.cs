using AspNetDeploy.Model;

namespace AspNetDeploy.Contracts
{
    public interface IDeploymentAgentFactory
    {
        IDeploymentAgent Create(Machine machine);
    }
}