using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using AspNetDeploy.Contracts;

namespace AspNetDeploy.Dapper
{
    public class DapperDataContext : IDataContext
    {
        public IDbConnection Connection { get; } = null;

        public DapperDataContext()
        {
            this.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["AspNetDeployEntitiesClean"].ConnectionString);
        }

        public void Cleanup()
        {
            this.Connection?.Dispose();
        }
    }
}