using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using Newtonsoft.Json;
using ObjectFactory;
using SatelliteService.Contracts;
using SatelliteService.Operations;

namespace SatelliteService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)] 
    public class DeploymentService : IDeploymentService
    {
        private int activePublicationId;

        private readonly IPathRepository pathRepository;
        private IList<Operation> queuedOperations;
        private IList<Operation> completedOperations;

        public DeploymentService()
        {
            this.pathRepository = Factory.GetInstance<IPathRepository>();
        }

        public bool IsReady()
        {
            return this.activePublicationId == 0;
        }

        public bool BeginPublication(int publicationId)
        {
            if (this.activePublicationId == 0)
            {
                this.activePublicationId = publicationId;
                this.queuedOperations = new List<Operation>();
                this.completedOperations = new List<Operation>();
                return true;
            }

            return false;
        }

        public void Commit()
        {
            foreach (Operation operation in this.queuedOperations)
            {
                try
                {
                    operation.Run();
                    this.completedOperations.Add(operation);
                }
                catch (Exception)
                {
                    this.Rollback();
                    throw;
                }
            }

            this.completedOperations.Clear();
        }

        public void Rollback()
        {
            foreach (Operation operation in this.completedOperations.Reverse())
            {
                operation.Rollback();
            }
        }

        public void ResetPackage()
        {
            if (File.Exists(this.pathRepository.GetPackagePath(this.activePublicationId)))
            {
                File.Delete(this.pathRepository.GetPackagePath(this.activePublicationId));
            }
        }

        public void UploadPackageBuffer(byte[] buffer)
        {
            using (FileStream fileStream = new FileStream(this.pathRepository.GetPackagePath(this.activePublicationId), FileMode.Append, FileAccess.Write, FileShare.None))
            {
                fileStream.Write(buffer, 0, buffer.Length);
            }
        }

        public void DeployWebSite(string jsonConfig)
        {
            WebSiteOperation operation = Factory.GetInstance<WebSiteOperation>();
            operation.Configure(JsonConvert.DeserializeObject(jsonConfig));

            this.queuedOperations.Add(operation);
        }

        public void ProcessConfigFile(string jsonConfig)
        {
            ConfigOperation operation = Factory.GetInstance<ConfigOperation>();
            operation.Configure(JsonConvert.DeserializeObject(jsonConfig), new Dictionary<string, object>());

            this.queuedOperations.Add(operation);
        }

        public void RunPowerShellScript()
        {
            throw new System.NotImplementedException();
        }

        public void CopyFiles()
        {
            throw new System.NotImplementedException();
        }

        public void UpdateHostsFile()
        {
            throw new System.NotImplementedException();
        }

        public void RunSQLScript()
        {
            throw new System.NotImplementedException();
        }
    }
}