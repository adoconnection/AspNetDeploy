using System.ServiceModel;

namespace SatelliteService
{
    [ServiceContract]
    public interface IDeploymentService
    {
        [OperationContract]
        bool IsReady();

        [OperationContract]
        bool BeginPublication(int publicationId);

        [OperationContract]
        bool ExecuteNextOperation();

        [OperationContract]
        bool Complete();

        [OperationContract]
        bool Rollback();

        [OperationContract]
        void UploadPackageBuffer(byte[] buffer);

        [OperationContract]
        void ResetPackage();

        [OperationContract]
        void DeployWebSite(string json);

        [OperationContract]
        void ProcessConfigFile(string json);

        [OperationContract]
        void RunPowerShellScript(string json);

        [OperationContract]
        void CopyFiles(string json);

        [OperationContract]
        void UpdateHostsFile(string json);

        [OperationContract]
        void RunSQLScript(string json);

        [OperationContract]
        void ApplyDacpac(string jsonConfig);

        [OperationContract]
        ExceptionInfo GetLastException();
    }
}