using System.Data.Entity;

namespace AspNetDeploy.Model
{
    public class MssqlDatabaseInitializer : DropCreateDatabaseAlways<AspNetDeployEntities>
    {
        public override void InitializeDatabase(AspNetDeployEntities context)
        {
            base.InitializeDatabase(context);
        }

        protected override void Seed(AspNetDeployEntities context)
        {
            base.Seed(context);
        }
    }
}