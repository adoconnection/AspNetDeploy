using System;
using System.IO;
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
        private readonly DeploymentServiceClient client;

        public WCFSatelliteDeploymentAgent()
        {
            client = new DeploymentServiceClient("WSHttpBinding_IDeploymentService", "http://localhost:8090/AspNetDeploySatellite/DeploymentService");
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

                default:
                    throw new AspNetDeployException("Deployment step type is not supported: " + deploymentStep.Type);
            }
        }

        private void ProcessWebSiteDeploymentStep(DeploymentStep deploymentStep)
        {
            /*JsonConvert.SerializeObject(new
            {
                
            })*/

            //this.client.DeployWebSite();
        }
    }
}