using SatelliteService.Contracts;

namespace SatelliteService.Operations
{
    public class ConfigOperation : Operation
    {
         private dynamic configuration;

         public ConfigOperation(IBackupRepository backupRepository) : base(backupRepository)
        {
        }

        public void Configure(dynamic configuration)
        {
            this.configuration = configuration;
        }

        public override void Run()
        {
            
        }

        public override void Rollback()
        {
            
        }
    }
}