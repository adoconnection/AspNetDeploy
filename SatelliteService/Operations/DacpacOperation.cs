using System.IO;
using Microsoft.SqlServer.Dac;
using SatelliteService.Contracts;

namespace SatelliteService.Operations
{
    public class DacpacOperation : Operation
    {
        private readonly IPackageRepository packageRepository;
        private dynamic configuration;

        public DacpacOperation(IBackupRepository backupRepository, IPackageRepository packageRepository) : base(backupRepository)
        {
            this.packageRepository = packageRepository;
        }

        public void Configure(dynamic configuration)
        {
            this.configuration = configuration;
        }

        public override void Run()
        {
            Stream dacpacStream = packageRepository.LoadProjectFile((int)this.configuration.projectId, (string)this.configuration.dacpacFileName);

            DacServices dbServices = new DacServices((string) this.configuration.connectionString);
            DacPackage dbPackage = DacPackage.Load(dacpacStream, DacSchemaModelStorageType.Memory);

            DacDeployOptions options = new DacDeployOptions();
            options.BackupDatabaseBeforeChanges = ((bool?)this.configuration.backupDatabaseBeforeChanges ?? true);
            options.BlockOnPossibleDataLoss = ((bool?)this.configuration.blockOnPossibleDataLoss ?? true); 
            options.IgnoreComments = true;
            options.IgnorePermissions = true;
            options.IgnorePartitionSchemes = true;
            options.IgnoreRoleMembership = true;
            options.DropObjectsNotInSource = ((bool?)this.configuration.dropObjectsNotInSource ?? false);
            options.DropPermissionsNotInSource = ((bool?)this.configuration.dropPermissionsNotInSource ?? false); 
            options.DropRoleMembersNotInSource = ((bool?)this.configuration.dropRoleMembersNotInSource ?? false); 
            options.DropIndexesNotInSource = ((bool?)this.configuration.dropIndexesNotInSource ?? false); 
            options.IgnoreIndexOptions = true;
            options.SqlCommandVariableValues.Add("debug", "false");

            dbServices.Deploy(dbPackage, (string)this.configuration.targetDatabase, true, options);
        }

        public override void Rollback()
        {
        }
    }
}