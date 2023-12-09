using System.ServiceModel;

namespace SatelliteService
{
    public interface IDeploymentService
    {
        
        bool IsReady();

        
        bool BeginPublication(int publicationId);

        
        bool ExecuteNextOperation();

        
        bool Complete();

        
        bool Rollback();

        
        void UploadPackageBuffer(byte[] buffer);

        
        void ResetPackage();

        
        void DeployWebSite(string json);

        void DeployWebSite(dynamic config);

        
        void ProcessConfigFile(string json);

        void ProcessConfigFile(dynamic config);

        
        void RunPowerShellScript(string json);

        void RunPowerShellScript(dynamic config);

        
        void CopyFiles(string json);

        void CopyFiles(dynamic config);

        
        void UpdateHostsFile(string json);

        void UpdateHostsFile(dynamic config);

        
        void RunSQLScript(string json);

        void RunSQLScript(dynamic config);

        
        void ApplyDacpac(string jsonConfig);

        void ApplyDacpac(dynamic config);

        
        ExceptionInfo GetLastException();
    }
}