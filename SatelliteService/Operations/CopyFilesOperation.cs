using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SatelliteService.Contracts;
using SatelliteService.Helpers;

namespace SatelliteService.Operations
{
    public class CopyFilesOperation : Operation
    {
        private readonly IPackageRepository packageRepository;
        private dynamic configuration;

        private readonly IList<Guid> replacedFiles = new List<Guid>();
        private readonly IList<string> createdFiles = new List<string>();
        private readonly IList<string> createdDirectories = new List<string>();
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
                    this.RunReplace();
                    break;

                case "append":
                    this.RunAppend();
                    break;
            }

            
        }

        private void RunAppend()
        {
            packageRepository.ExtractProject(
                (int)this.configuration.projectId,
                (string)this.configuration.destination,
                (entryDestinationPath, isDirectory) =>
                {
                    if (isDirectory)
                    {
                        string[] strings = entryDestinationPath.TrimEnd('/').Split('/');
                        int length = strings.Length;

                        for (int i = 0; i < length; i++)
                        {
                            string directory = string.Join("/", strings.Take(i + 1));

                            if (!Directory.Exists(directory))
                            {
                                createdDirectories.Add(directory);
                            }
                        }
                    }
                    else
                    {
                        if (!File.Exists(entryDestinationPath))
                        {
                            createdFiles.Add(entryDestinationPath);
                        }
                        else
                        {
                            Guid fileGuid = this.BackupRepository.StoreFile(entryDestinationPath);
                            replacedFiles.Add(fileGuid);

                            File.Delete(entryDestinationPath);
                        }
                    }
                });
        }

        private void RunReplace()
        {
            this.backupDirectoryGuid = this.BackupRepository.StoreDirectory((string) this.configuration.destination);
            DirectoryHelper.DeleteContents((string) this.configuration.destination);

            packageRepository.ExtractProject(
                (int)this.configuration.projectId,
                (string)this.configuration.destination,
                (entryDestinationPath, isDirectory) => { });
        }


        public override void Rollback()
        {
            if (this.destinationExists)
            {
                switch (((string)this.configuration.mode).ToLower())
                {
                    case "replace":
                        this.RollbackReplace();
                        break;

                    case "append":
                        this.RollbackAppend();
                        break;
                }

                
            }
        }

        private void RollbackAppend()
        {
            foreach (Guid value in replacedFiles)
            {
                this.BackupRepository.RestoreFile(value);
            }

            foreach (string value in createdDirectories)
            {
                if (Directory.Exists(value))
                {
                    Directory.Delete(value, true);
                }
            }

            foreach (string value in createdFiles)
            {
                if (File.Exists(value))
                {
                    File.Delete(value);
                }
            }
        }

        private void RollbackReplace()
        {
            this.BackupRepository.RestoreDirectory(this.backupDirectoryGuid.Value);
        }
    }
}