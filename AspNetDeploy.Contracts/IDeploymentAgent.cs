using System;
using AspNetDeploy.Contracts.Exceptions;
using AspNetDeploy.Contracts.MachineSummary;
using AspNetDeploy.Model;

namespace AspNetDeploy.Contracts
{
    public interface IDeploymentAgent
    {
        bool IsReady();
        int GetVersion();
        bool BeginPublication(int publicationId);
        bool ExecuteNextOperation();
        bool Complete();
        void Rollback();

        void UploadPackage(string file, Action<int, int> progress = null);
        void ResetPackage();
        void ProcessDeploymentStep(DeploymentStep deploymentStep);
        IExceptionInfo GetLastException();
        IServerSummary GetServerSummary();
    }
}