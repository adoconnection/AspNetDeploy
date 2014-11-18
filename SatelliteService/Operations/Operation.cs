using System.Collections.Generic;
using SatelliteService.Contracts;

namespace SatelliteService.Operations
{
    public abstract class Operation
    {
        protected IBackupRepository BackupRepository { get; private set; }

        protected Operation(IBackupRepository backupRepository)
        {
            this.BackupRepository = backupRepository;
        }

        public abstract void Run();
        public abstract void Rollback();
    }
}