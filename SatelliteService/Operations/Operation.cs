using System.Collections.Generic;
using SatelliteService.Contracts;

namespace SatelliteService.Operations
{
    public abstract class Operation
    {
        protected IDictionary<string, object> Variables { get; private set; }
        protected IBackupRepository BackupRepository { get; private set; }

        protected Operation(IBackupRepository backupRepository)
        {
            this.BackupRepository = backupRepository;
        }

        protected void Configure(IDictionary<string, object> variables)
        {
            this.Variables = variables;
        }

        public abstract void Run();
        public abstract void Rollback();

        /*protected string ApplyVariables(string value)
        {
            
        }*/
    }
}