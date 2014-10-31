using AspNetDeploy.Model;

namespace AspNetDeploy.Contracts
{
    public interface ISolutionParsersFactory
    {
        ISolutionParser Create(SolutionType solutionType);
    }
}