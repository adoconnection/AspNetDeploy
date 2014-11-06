using AspNetDeploy.Contracts;
using AspNetDeploy.Contracts.Exceptions;
using AspNetDeploy.Model;
using AspNetDeploy.SolutionParsers.VisualStudio;

namespace AspNetDeploy.SolutionParsers
{
    public class SolutionParsersFactory : ISolutionParsersFactory
    {
        public ISolutionParser Create(SolutionType solutionType)
        {
            switch (solutionType)
            {
                case SolutionType.VisualStudio:
                    return new VisualStudio2013SolutionParser();

                default:
                    throw new AspNetDeployException("Unknown SolutionType: " + solutionType);
            }
        }
    }
}
