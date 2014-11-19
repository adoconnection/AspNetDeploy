using System.IO;
using Microsoft.SqlServer.Dac;
using SatelliteService.Contracts;

namespace SatelliteService.Operations
{
    public class DacpacOperation : Operation
    {
        private dynamic configuration;

        public DacpacOperation(IBackupRepository backupRepository) : base(backupRepository)
        {
        }

        public void Configure(dynamic configuration)
        {
            this.configuration = configuration;
        }

        public override void Run()
        {
            DacServices dbServices = new DacServices((string) this.configuration.connectionString);
            DacPackage dbPackage = DacPackage.Load(new MemoryStream(), DacSchemaModelStorageType.Memory);

            DacDeployOptions dbDeployOptions = new DacDeployOptions();
            dbDeployOptions.BackupDatabaseBeforeChanges = true;
            dbDeployOptions.BlockOnPossibleDataLoss = true;
            dbDeployOptions.SqlCommandVariableValues.Add("debug", "false");

            dbServices.Deploy(dbPackage, (string)this.configuration.targetDatabase, true, dbDeployOptions);
        }

        public override void Rollback()
        {
        }
    }
}