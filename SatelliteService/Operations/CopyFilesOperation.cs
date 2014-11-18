using System;
using SatelliteService.Contracts;
using SatelliteService.Helpers;

namespace SatelliteService.Operations
{
    public class CopyFilesOperation : Operation
    {
        private readonly IPackageRepository packageRepository;
        private dynamic configuration;

        private Guid? backupDirectoryGuid;

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
            this.backupDirectoryGuid = this.BackupRepository.StoreDirectory((string)this.configuration.destination);
            DirectoryHelper.DeleteContents((string)this.configuration.destination);

            packageRepository.ExtractFiles(
                (int)this.configuration.archiveId, 
                (string)this.configuration.destination);
        }


        public override void Rollback()
        {
            if (this.backupDirectoryGuid.HasValue)
            {
                this.BackupRepository.RestoreDirectory(this.backupDirectoryGuid.Value);
            }
        }
    }
}