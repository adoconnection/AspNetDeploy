using System;
using System.Collections.Generic;
using System.IO;
using SatelliteService.Contracts;
using SatelliteService.Helpers;

namespace SatelliteService.Operations
{
    public class CopyFilesOperation : Operation
    {
        private readonly IPackageRepository packageRepository;
        private dynamic configuration;

        private IDictionary<string, Guid> replacedFiles = new Dictionary<string, Guid>();
        private IList<string> createdFiles = new List<string>();
        private IList<string> createdDirectories = new List<string>();
        private Guid? backupDirectoryGuid;
        private bool destinationExists;

        public CopyFilesOperation(IBackupRepository backupRepository, IPackageRepository packageRepository) : base(backupRepository)
        {
            this.packageRepository = packageRepository;
        }

        public void Configure(dynamic configuration)
        {
            this.configuration = configuration;
        }

        public override void Run()
        {
            if (Directory.Exists((string) this.configuration.destination))
            {
                this.destinationExists = true;
            }

            switch (((string)this.configuration.mode).ToLower())
            {
                case "replace":
                    this.backupDirectoryGuid = this.BackupRepository.StoreDirectory((string)this.configuration.destination);
                    DirectoryHelper.DeleteContents((string)this.configuration.destination);
                    break;

                case "append":

                    break;
            }

                packageRepository.ExtractProject(
                    (int)this.configuration.projectId, 
                    (string)this.configuration.destination,
                    (entryDestinationPath, isDirectory) =>
                    {
                        if (isDirectory)
                        {
                            
                        }

                    });
        }


        public override void Rollback()
        {
            if (this.destinationExists)
            {
                switch (((string)this.configuration.mode).ToLower())
                {
                    case "replace":
                        this.BackupRepository.RestoreDirectory(this.backupDirectoryGuid.Value);
                        break;

                    case "append":
                        this.BackupRepository.RestoreDirectory(this.backupDirectoryGuid.Value);
                        break;
                }

                
            }
        }
    }
}