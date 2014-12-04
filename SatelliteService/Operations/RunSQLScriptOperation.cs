using System.Data.SqlClient;
using System.Xml;
using SatelliteService.Contracts;

namespace SatelliteService.Operations
{
    public class RunSQLScriptOperation : Operation
    {
        private dynamic configuration;

        public RunSQLScriptOperation(IBackupRepository backupRepository) : base(backupRepository)
        {
        }

        public void Configure(dynamic configuration)
        {
            this.configuration = configuration;
        }

        public override void Run()
        {
            SqlConnection sqlConnection = new SqlConnection((string)configuration.connectionString);

            sqlConnection.Open();

            const string commandTemplate = 
                @"BEGIN TRANSACTION
                       {0}
                  COMMIT TRANSACTION";

            SqlCommand command = new SqlCommand(string.Format(commandTemplate, (string)configuration.command), sqlConnection);

            command.ExecuteNonQuery();
            sqlConnection.Close();
        }


        public override void Rollback()
        {
        }
    }
}