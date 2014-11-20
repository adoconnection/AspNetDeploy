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

        public WCFSatelliteDeploymentAgent(IVariableProcessor variableProcessor, string endpoint)
        {
            this.variableProcessor = variableProcessor;
            this.client = new DeploymentServiceClient("WSHttpBinding_IDeploymentService", endpoint);
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

        public void Commit()
        {
            this.client.Commit();
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

                default:
                    throw new AspNetDeployException("Deployment step type is not supported: " + deploymentStep.Type);
            }
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