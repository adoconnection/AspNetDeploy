using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using AspNetDeploy.Contracts;
using AspNetDeploy.Contracts.Exceptions;
using AspNetDeploy.DeploymentServices.WCFSatellite.SatelliteServiceReference;
using AspNetDeploy.Model;
using Newtonsoft.Json;

namespace AspNetDeploy.DeploymentServices.WCFSatellite
{
    public class WCFSatelliteDeploymentAgent : IDeploymentAgent, IDisposable
    {
        private readonly IVariableProcessor variableProcessor;
        private readonly DeploymentServiceClient client;

        public WCFSatelliteDeploymentAgent(IVariableProcessor variableProcessor, string endpoint, string login, string password, TimeSpan? openTimeoutSpan = null)
        {
            this.variableProcessor = variableProcessor;

            WSHttpBinding binding = new WSHttpBinding(SecurityMode.TransportWithMessageCredential);
            EndpointAddress endpointAddress = new EndpointAddress(new Uri(endpoint));

            binding.MaxBufferPoolSize = 1024 * 1024 * 10;
            binding.MaxReceivedMessageSize = 1024 * 1024 * 10;
            binding.ReaderQuotas.MaxArrayLength = 1024 * 1024 * 10;

            binding.OpenTimeout = openTimeoutSpan ?? new TimeSpan(0, 10, 0);
            binding.CloseTimeout = new TimeSpan(0, 10, 0);
            binding.SendTimeout = new TimeSpan(0, 10, 0);
            binding.ReceiveTimeout = new TimeSpan(3, 0, 0);

            binding.BypassProxyOnLocal = false;
            binding.UseDefaultWebProxy = true;

            binding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;

            this.client = new DeploymentServiceClient(binding, endpointAddress);
            this.client.ClientCredentials.UserName.UserName = login;
            this.client.ClientCredentials.UserName.Password = password;
        }

        public void Dispose()
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
            
            return this.client.IsReady();
        }

        public bool BeginPublication(int publicationId)
        {
            return this.client.BeginPublication(publicationId);
        }
        public bool ExecuteNextOperation()
        {
            return this.client.ExecuteNextOperation();
        }
        public bool Complete()
        {
            return this.client.Complete();
        }

        public void Rollback()
        {
            this.client.Rollback();
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

                    client.UploadPackageBuffer(buffer);

                    if (progress != null)
                    {
                        progress(position, length);
                    }
                }
            }
        }

        public void ResetPackage()
        {
            this.client.ResetPackage();
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

                default:
                    throw new AspNetDeployException("Deployment step type is not supported: " + deploymentStep.Type);
            }
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

            this.client.DeployWebSite(configuration);
        }

        private void ProcessWebSiteDeploymentStep(DeploymentStep deploymentStep)
        {
            string configuration = JsonConvert.SerializeObject(new
            {
                destination = this.variableProcessor.ProcessValue(deploymentStep.GetStringProperty("IIS.DestinationPath")),
                siteName = this.variableProcessor.ProcessValue(deploymentStep.GetStringProperty("IIS.SiteName")),
                applicationPoolName = this.variableProcessor.ProcessValue(deploymentStep.GetStringProperty("IIS.SiteName")),
                projectId = deploymentStep.GetIntProperty("ProjectId"),
                bindings = ((IEnumerable<dynamic>) deploymentStep.GetDynamicProperty("IIS.Bindings")).Select(b => new
                {
                    protocol = this.variableProcessor.ProcessValue((string) b.protocol),
                    port = this.variableProcessor.ProcessValue((string) b.port),
                    host = this.variableProcessor.ProcessValue((string) b.host),
                })
            });

            this.client.DeployWebSite(configuration);
        }

        private void ProcessConfigurationStep(DeploymentStep deploymentStep)
        {
            string configuration = JsonConvert.SerializeObject(new
            {
                file = this.variableProcessor.ProcessValue(deploymentStep.GetStringProperty("File")),
                content = this.variableProcessor.ProcessValue(deploymentStep.GetStringProperty("SetValues"))
            });

            this.client.ProcessConfigFile(configuration);
        }
    }
}