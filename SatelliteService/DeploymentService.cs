using System;
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
        private readonly IBackupRepository backupRepository;
        private readonly IPackageRepositoryFactory packageRepositoryFactory;

        private int nextOperationIndex = 0;
        private IList<Operation> queuedOperations = new List<Operation>();
        private IList<Operation> completedOperations = new List<Operation>();
        private Exception lastException = null;

        public DeploymentService()
        {
            this.pathRepository = Factory.GetInstance<IPathRepository>();
            this.backupRepository = Factory.GetInstance<IBackupRepository>();
            this.packageRepositoryFactory = Factory.GetInstance<IPackageRepositoryFactory>();
        }

        public bool IsReady()
        {
            return this.activePublicationId == 0;
        }

        public int GetVersion()
        {
            return 100;
        }

        public ExceptionInfo GetLastException()
        {
            if (this.lastException == null)
            {
                return null;
            }

            ExceptionInfoFactory factory = new ExceptionInfoFactory();
            return factory.Create(this.lastException);
        }

        public bool BeginPublication(int publicationId)
        {
            if (this.activePublicationId != 0)
            {
                return false;
            }

            Console.WriteLine("Begin publication: " + publicationId);

            this.activePublicationId = publicationId;
            this.queuedOperations = new List<Operation>();
            this.completedOperations = new List<Operation>();
            this.nextOperationIndex = 0;
            this.lastException = null;

            return true;
        }

        public bool ExecuteNextOperation()
        {
            if (this.nextOperationIndex == this.queuedOperations.Count)
            {
                return false;
            }

            try
            {
                Operation operation = this.queuedOperations[this.nextOperationIndex];

                Console.Write("Executing operation: " + this.nextOperationIndex);
                operation.Run();
                Console.Write("– OK\n\r");
                this.completedOperations.Add(operation);
                this.nextOperationIndex++;

                return true;
            }
            catch (Exception e)
            {
                this.lastException = e;

                Console.WriteLine("– Error\n\r");
                Console.WriteLine(e.Message);
                Console.WriteLine("---------");
                Console.WriteLine(e.Source);
                Console.WriteLine("---------");
                Console.WriteLine(e.StackTrace);

                Exception innerException = e.InnerException;

                while (innerException != null)
                {
                    Console.WriteLine("Inner:");
                    Console.WriteLine(innerException.Message);
                    Console.WriteLine("---------");
                    Console.WriteLine(innerException.Source);
                    Console.WriteLine("---------");
                    Console.WriteLine(innerException.StackTrace);
                    innerException = innerException.InnerException;
                }

                this.Rollback();

                return false;
            }
        }

        public bool Complete()
        {
            this.activePublicationId = 0;
            this.completedOperations.Clear();
            this.queuedOperations.Clear();
            this.lastException = null;

            Console.WriteLine("Publication complete");

            return true;
        }

        public bool Rollback()
        {
            Console.WriteLine("Rolling back");

            foreach (Operation operation in this.completedOperations.Reverse())
            {
                operation.Rollback();
            }

            this.activePublicationId = 0;
            this.completedOperations.Clear();

            Console.WriteLine("Rollback complete");

            return true;
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
            string packagePath = pathRepository.GetPackagePath(this.activePublicationId);
            WebSiteOperation operation = new WebSiteOperation(this.backupRepository, this.packageRepositoryFactory.Create(packagePath));
            operation.Configure(JsonConvert.DeserializeObject(jsonConfig));

            this.queuedOperations.Add(operation);
        }

        public void ProcessConfigFile(string jsonConfig)
        {
            ConfigOperation operation = new ConfigOperation(this.backupRepository);
            operation.Configure(JsonConvert.DeserializeObject(jsonConfig));

            this.queuedOperations.Add(operation);
        }

        public void RunPowerShellScript(string jsonConfig)
        {
            throw new System.NotImplementedException();
        }

        public void CopyFiles(string jsonConfig)
        {
            string packagePath = pathRepository.GetPackagePath(this.activePublicationId);
            CopyFilesOperation operation = new CopyFilesOperation(this.backupRepository, this.packageRepositoryFactory.Create(packagePath));
            operation.Configure(JsonConvert.DeserializeObject(jsonConfig));

            this.queuedOperations.Add(operation);
        }

        public void UpdateHostsFile(string jsonConfig)
        {
            UpdateHostsOperation operation = new UpdateHostsOperation(this.backupRepository);
            operation.Configure(JsonConvert.DeserializeObject(jsonConfig));

            this.queuedOperations.Add(operation);
        }

        public void RunSQLScript(string jsonConfig)
        {
            RunSQLScriptOperation operation = new RunSQLScriptOperation(this.backupRepository);
            operation.Configure(JsonConvert.DeserializeObject(jsonConfig));

            this.queuedOperations.Add(operation);
        }

        public void ApplyDacpac(string jsonConfig)
        {
            string packagePath = pathRepository.GetPackagePath(this.activePublicationId);
            DacpacOperation operation = new DacpacOperation(this.backupRepository, this.packageRepositoryFactory.Create(packagePath));
            operation.Configure(JsonConvert.DeserializeObject(jsonConfig));

            this.queuedOperations.Add(operation);
        }
    }
}