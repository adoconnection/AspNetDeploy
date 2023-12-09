using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using AspNetDeploy.Contracts;
using AspNetDeploy.Contracts.Exceptions;
using AspNetDeploy.Contracts.MachineSummary;
using AspNetDeploy.Model;
using DeploymentServices;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using GrpcSatellite;
using Newtonsoft.Json;

namespace DeploymentServices.Grpc
{
    public class GrpcDeploymentAgent: IDeploymentAgent, IDisposable
    {
        private readonly IVariableProcessor variableProcessor;
        private readonly Deployment.DeploymentClient deploymentClient;

        public GrpcDeploymentAgent(IVariableProcessor variableProcessor, string endpoint, string login, string password)
        {
            this.variableProcessor = variableProcessor;
            
            var options = new GrpcChannelOptions();
            var channel = GrpcChannel.ForAddress(new Uri("https://localhost:7142"), options);
            this.deploymentClient = new Deployment.DeploymentClient(channel);
        }

        public bool IsReady()
        {
            return this.deploymentClient.IsReady(new Empty()).IsReady;
        }

        public int GetVersion()
        {
            throw new NotImplementedException();
        }

        public bool BeginPublication(int publicationId)
        {
            return this.deploymentClient.ExecuteNextOperation(new Empty()).IsSuccess;
        }

        public bool ExecuteNextOperation()
        {
            return this.deploymentClient.ExecuteNextOperation(new Empty()).IsSuccess;
        }

        public bool Complete()
        {
            return this.deploymentClient.ExecuteNextOperation(new Empty()).IsSuccess;
        }

        public void Rollback()
        {
            this.deploymentClient.Rollback(new Empty());
        }

        public void UploadPackage(string file, Action<int, int> progress = null)
        {
            using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                int length = (int)stream.Length;
                int bufferSize = 1024 * 500; //kb
                int position = 0;

                var uploadBufferRequest = this.deploymentClient.UploadPackageBuffer();

                while (length > 0)
                {
                    int readSize = length > bufferSize ? bufferSize : length;
                    byte[] buffer = new byte[readSize];

                    stream.Read(buffer, 0, readSize);
                    position += readSize;
                    length -= readSize;

                    uploadBufferRequest.RequestStream.WriteAsync(
                       new UploadPackageBufferRequest()
                        {
                            Buffer = UnsafeByteOperations.UnsafeWrap(buffer)
                        });

                    if (progress != null)
                    {
                        progress(position, length);
                    }

                    Thread.Sleep(50);
                }

                uploadBufferRequest.RequestStream.CompleteAsync();
            }
        }

        public void ResetPackage()
        {
            this.deploymentClient.ResetPackage(new Empty());
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

            this.deploymentClient.ApplyDacPac(new ApplyDacPacRequest()
            {
                DacPacFilename = "database.dacpac",
                ConnectionString = this.variableProcessor.ProcessValue(deploymentStep.GetStringProperty("ConnectionString")),
                TargetDatabase = this.variableProcessor.ProcessValue(deploymentStep.GetStringProperty("TargetDatabase")),
                ProjectId = deploymentStep.GetIntProperty("ProjectId"),
                BackupDatabaseBeforeChanges = (bool)customConfig.backupDatabaseBeforeChanges,
                BlockOnPossibleDataLos = (bool)customConfig.blockOnPossibleDataLoss,
            });
        }

        private void ProcessSQLStep(DeploymentStep deploymentStep)
        {
            this.deploymentClient.RunSQLScripts(new RunSQLScriptsRequest()
            {
                ConnectionString = 
                    this.variableProcessor.ProcessValue(deploymentStep.GetStringProperty("ConnectionString")),
                Command = this.variableProcessor.ProcessValue(deploymentStep.GetStringProperty("Command")),
            });
        }

        private void ProcessHostsStep(DeploymentStep deploymentStep)
        {
            this.deploymentClient.UpdateHostsFile(new UpdateHostsFileRequest()
            {
                ConfigurationJson = this.variableProcessor.ProcessValue(deploymentStep.GetStringProperty("ConfigurationJson"))
            });
        }

        private void ProcessCopyFilesStep(DeploymentStep deploymentStep)
        {
            string mode = "replace";

            if (!string.IsNullOrWhiteSpace(deploymentStep.GetStringProperty("CustomConfiguration")))
            {
                dynamic customConfig = deploymentStep.GetDynamicProperty("CustomConfiguration");

                mode = customConfig.mode ?? "replace";
            }

            this.deploymentClient.CopyFiles(new CopyFilesRequest()
            {
                Destination = this.variableProcessor.ProcessValue(deploymentStep.GetStringProperty("DestinationPath")),
                ProjectId = deploymentStep.GetIntProperty("ProjectId"),
                Mode = mode
            });
        }

        private void ProcessWebSiteDeploymentStep(DeploymentStep deploymentStep)
        {
            dynamic bindings = JsonConvert.DeserializeObject(this.variableProcessor.ProcessValue(deploymentStep.GetStringProperty("IIS.Bindings")) ?? "[]");

            this.deploymentClient.DeployWebSite(new DeployWebSiteRequest()
            {
                Destination = this.variableProcessor.ProcessValue(deploymentStep.GetStringProperty("IIS.DestinationPath")),
                SiteName = this.variableProcessor.ProcessValue(deploymentStep.GetStringProperty("IIS.SiteName")),
                ApplicationPoolName = this.variableProcessor.ProcessValue(deploymentStep.GetStringProperty("IIS.SiteName")),
                ProjectId = deploymentStep.GetIntProperty("ProjectId"),
                Bindings = JsonConvert.SerializeObject((IEnumerable<dynamic>)bindings)
            });
        }

        private void ProcessConfigurationStep(DeploymentStep deploymentStep)
        {
            this.deploymentClient.ProcessConfigFile(new ProccesConfigFileRequest()
            {
                File = this.variableProcessor.ProcessValue(deploymentStep.GetStringProperty("File")),
                Content = this.variableProcessor.ProcessValue(deploymentStep.GetStringProperty("SetValues"))
            });
        }

        public IExceptionInfo GetLastException()
        {
            throw new NotImplementedException();
        }

        public IServerSummary GetServerSummary()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}