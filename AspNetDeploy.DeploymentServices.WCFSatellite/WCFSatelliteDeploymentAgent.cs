using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using AspNetDeploy.Contracts;
using AspNetDeploy.Contracts.Exceptions;
using AspNetDeploy.Contracts.MachineSummary;
using AspNetDeploy.DeploymentServices.WCFSatellite.DeploymentServiceReference;
using AspNetDeploy.DeploymentServices.WCFSatellite.InformationServiceReference;
using AspNetDeploy.DeploymentServices.WCFSatellite.MonitoringServiceReference;
using AspNetDeploy.Model;
using Newtonsoft.Json;

namespace AspNetDeploy.DeploymentServices.WCFSatellite
{
    public class WCFSatelliteDeploymentAgent : IDeploymentAgent, IDisposable
    {
        private readonly IVariableProcessor variableProcessor;
        private readonly DeploymentServiceClient deploymentClient;
        private readonly MonitoringServiceClient monitoringClient;
        private readonly InformationServiceClient informationClient;

        public WCFSatelliteDeploymentAgent(IVariableProcessor variableProcessor, string endpoint, string login, string password, TimeSpan? openTimeoutSpan = null)
        {
            this.variableProcessor = variableProcessor;

            WSHttpBinding binding = new WSHttpBinding(SecurityMode.TransportWithMessageCredential);

            binding.MaxBufferPoolSize = 1024 * 1024 * 10;
            binding.MaxReceivedMessageSize = 1024 * 1024 * 10;
            binding.ReaderQuotas.MaxArrayLength = 1024 * 1024 * 10;

            binding.OpenTimeout = openTimeoutSpan ?? new TimeSpan(0, 10, 0);
            binding.CloseTimeout = openTimeoutSpan ?? new TimeSpan(0, 10, 0);
            binding.SendTimeout = new TimeSpan(3, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(3, 0, 0);
            
            binding.BypassProxyOnLocal = false;
            binding.UseDefaultWebProxy = true;

            binding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;

            this.deploymentClient = new DeploymentServiceClient(binding, new EndpointAddress(new Uri(endpoint + "/DeploymentService")));
            this.deploymentClient.ClientCredentials.UserName.UserName = login;
            this.deploymentClient.ClientCredentials.UserName.Password = password;

            this.monitoringClient = new MonitoringServiceClient(binding, new EndpointAddress(new Uri(endpoint + "/MonitoringService")));
            this.monitoringClient.ClientCredentials.UserName.UserName = login;
            this.monitoringClient.ClientCredentials.UserName.Password = password;

            this.informationClient = new InformationServiceClient(binding, new EndpointAddress(new Uri(endpoint + "/InformationService")));
            this.informationClient.ClientCredentials.UserName.UserName = login;
            this.informationClient.ClientCredentials.UserName.Password = password;
        }

        public void Dispose()
        {
            Dispose(this.deploymentClient);
            Dispose(this.monitoringClient);
            Dispose(this.informationClient);
        }

        private void Dispose(ICommunicationObject client)
        {
            if (client != null)
            {
                if (client.State == CommunicationState.Opened)
                {
                    client.Close();
                }
            }
        }

        public bool IsReady()
        {
            return this.deploymentClient.IsReady();
        }

        public int GetVersion()
        {
            return this.informationClient.GetVersion();
        }

        public IServerSummary GetServerSummary()
        {
            return this.monitoringClient.GetServerSummary();
        }

        public IExceptionInfo GetLastException()
        {
            return this.deploymentClient.GetLastException();
        }

        public bool BeginPublication(int publicationId)
        {
            return this.deploymentClient.BeginPublication(publicationId);
        }
        public bool ExecuteNextOperation()
        {
            return this.deploymentClient.ExecuteNextOperation();
        }
        public bool Complete()
        {
            return this.deploymentClient.Complete();
        }

        public void Rollback()
        {
            this.deploymentClient.Rollback();
        }

        public void UploadPackage(string file, Action<int, int> progress = null)
        {
            using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                int length = (int)stream.Length;
                int bufferSize = 1024 * 500; //kb
                int position = 0;

                while (length > 0)
                {
                    int readSize = length > bufferSize ? bufferSize : length;
                    byte[] buffer = new byte[readSize];

                    stream.Read(buffer, 0, readSize);
                    position += readSize;
                    length -= readSize;

                    this.deploymentClient.UploadPackageBuffer(buffer);

                    if (progress != null)
                    {
                        progress(position, length);
                    }
                }
            }
        }

        public void ResetPackage()
        {
            this.deploymentClient.ResetPackage();
        }

        public void ProcessDeploymentStep(DeploymentStep deploymentStep)
        {
            switch (deploymentStep.Type)
            {
                case DeploymentStepType.DeployWebSite:
                    this.ProcessWebSiteDeploymentStep(deploymentStep);
                    break;

                case DeploymentStepType.Configuration:
                    this.ProcessConfigurationStep(deploymentStep);
                    break;

                case DeploymentStepType.CopyFiles:
                    this.ProcessCopyFilesStep(deploymentStep);
                    break;

                case DeploymentStepType.UpdateHostsFile:
                    this.ProcessHostsStep(deploymentStep);
                    break;

                case DeploymentStepType.RunSQLScript:
                    this.ProcessSQLStep(deploymentStep);
                    break;

                case DeploymentStepType.DeployDacpac:
                    this.ProcessDacpacStep(deploymentStep);
                    break;

                default:
                    throw new AspNetDeployException("Deployment step type is not supported: " + deploymentStep.Type);
            }
        }

        private void ProcessDacpacStep(DeploymentStep deploymentStep)
        {
            dynamic customConfig = JsonConvert.DeserializeObject(this.variableProcessor.ProcessValue(deploymentStep.GetStringProperty("CustomConfiguration")));

            string configuration = JsonConvert.SerializeObject(new
            {
                dacpacFileName = "database.dacpac",
                connectionString = this.variableProcessor.ProcessValue(deploymentStep.GetStringProperty("ConnectionString")),
                targetDatabase = this.variableProcessor.ProcessValue(deploymentStep.GetStringProperty("TargetDatabase")),
                projectId = deploymentStep.GetIntProperty("ProjectId"),
                backupDatabaseBeforeChanges = (bool)customConfig.backupDatabaseBeforeChanges,
                blockOnPossibleDataLoss = (bool)customConfig.blockOnPossibleDataLoss,
            });

            this.deploymentClient.ApplyDacpac(configuration);
        }

        private void ProcessSQLStep(DeploymentStep deploymentStep)
        {
            string configuration = JsonConvert.SerializeObject(new
            {
                connectionString = this.variableProcessor.ProcessValue(deploymentStep.GetStringProperty("ConnectionString")),
                command = this.variableProcessor.ProcessValue(deploymentStep.GetStringProperty("Command")),
            });

            this.deploymentClient.RunSQLScript(configuration);
        }

        private void ProcessHostsStep(DeploymentStep deploymentStep)
        {
            this.deploymentClient.UpdateHostsFile(this.variableProcessor.ProcessValue(deploymentStep.GetStringProperty("ConfigurationJson")));
        }

        private void ProcessCopyFilesStep(DeploymentStep deploymentStep)
        {
            string mode = "replace";

            if (!string.IsNullOrWhiteSpace(deploymentStep.GetStringProperty("CustomConfiguration")))
            {
                dynamic customConfig = deploymentStep.GetDynamicProperty("CustomConfiguration");

                mode = customConfig.mode ?? "replace";
            }

            string configuration = JsonConvert.SerializeObject(new
            {
                destination = this.variableProcessor.ProcessValue(deploymentStep.GetStringProperty("DestinationPath")),
                projectId = deploymentStep.GetIntProperty("ProjectId"),
                mode = mode
            });

            this.deploymentClient.CopyFiles(configuration);
        }

        private void ProcessWebSiteDeploymentStep(DeploymentStep deploymentStep)
        {
            dynamic bindings = JsonConvert.DeserializeObject(this.variableProcessor.ProcessValue(deploymentStep.GetStringProperty("IIS.Bindings")));

            string configuration = JsonConvert.SerializeObject(new
            {
                destination = this.variableProcessor.ProcessValue(deploymentStep.GetStringProperty("IIS.DestinationPath")),
                siteName = this.variableProcessor.ProcessValue(deploymentStep.GetStringProperty("IIS.SiteName")),
                applicationPoolName = this.variableProcessor.ProcessValue(deploymentStep.GetStringProperty("IIS.SiteName")),
                projectId = deploymentStep.GetIntProperty("ProjectId"),
                bindings = ((IEnumerable<dynamic>)bindings)
            });

            this.deploymentClient.DeployWebSite(configuration);
        }

        private void ProcessConfigurationStep(DeploymentStep deploymentStep)
        {
            string configuration = JsonConvert.SerializeObject(new
            {
                file = this.variableProcessor.ProcessValue(deploymentStep.GetStringProperty("File")),
                content = this.variableProcessor.ProcessValue(deploymentStep.GetStringProperty("SetValues"))
            });

            this.deploymentClient.ProcessConfigFile(configuration);
        }
    }
}