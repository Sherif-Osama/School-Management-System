using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace School.DAL.Common
{
    public abstract class BaseData
    {
        private readonly string _connectionString;

        protected BaseData(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ??
                                            throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");
        }

        protected async Task<SqlConnection> GetOpenConnectionAsync()
        {
            SqlConnection connection = new(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

        protected static SqlCommand CreateStoredProcedure(SqlConnection connection, string procedureName)
        {
            return new SqlCommand(procedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };
        }
    }
}