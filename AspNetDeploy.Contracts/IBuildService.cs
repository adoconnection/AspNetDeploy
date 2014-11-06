using System.Security.Cryptography.X509Certificates;

namespace AspNetDeploy.Contracts
{
    public interface IBuildService
    {
        void Build(string solutionFilePath, IContinuousIntegrationLogger logger);
    }
}