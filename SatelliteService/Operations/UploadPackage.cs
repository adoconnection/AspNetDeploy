using SatelliteService.Contracts;

namespace SatelliteService.Operations
{
    public class UploadPackage : Operation
    {
        public UploadPackage(IBackupRepository backupRepository) : base(backupRepository)
        {
        }

        public override void Run()
        {
            throw new System.NotImplementedException();
        }

        public override void Rollback()
        {
            throw new System.NotImplementedException();
        }
    }
}