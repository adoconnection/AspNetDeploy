using System.Collections.Generic;
using System.Data;

namespace AspNetDeploy.Model.Repositories
{
    public class BundleRepository
    {
        private IDbConnection connection;

        public BundleRepository(IDataContext dataContext)
        {
        }

        public IList<Bundle> List()
        {
            this.connection.q
        }
    }
}