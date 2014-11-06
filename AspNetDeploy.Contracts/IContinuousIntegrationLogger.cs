using AspNetDeploy.Model;

namespace AspNetDeploy.Contracts
{
    public interface IContinuousIntegrationLogger
    {
        void SolutionBuildStarted(string solutionFileName);
        void SolutionBuildComplete(string solutionFileName);
        void SolutionBuildFailed(string solutionFileName);
        void ProjectBuildStarted(int projectId);
        void ProjectBuildComplete(int projectId);
        void ProjectBuildFailed(int projectId);
    }
}