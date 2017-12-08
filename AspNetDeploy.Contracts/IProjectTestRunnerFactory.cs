using AspNetDeploy.Model;

namespace AspNetDeploy.Contracts
{
    public interface IProjectTestRunnerFactory
    {
        IProjectTestRunner Create(ProjectType projectType, IVariableProcessor variableProcessor);
    }
}