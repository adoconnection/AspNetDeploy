using System.Collections.Generic;
using System.Data;
using System.Linq;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;
using Dapper;

namespace AspNetDeploy.Dapper
{
    public class BundleRepository
    {
        private readonly DapperDataContext dataContext;

        public BundleRepository(IDataContext dataContext)
        {
            this.dataContext = (DapperDataContext) dataContext;
        }

        public IList<Bundle> List()
        {
            return this.dataContext.Connection
                .Query<Bundle>("SELECT * FROM [Bundle]")
                .ToList();
        }
    }
}