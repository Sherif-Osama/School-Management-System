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

        protected async Task<List<T>> QueryListAsync<T>(string procedureName, Action<SqlCommand>? addParameters, Func<SqlDataReader, T> map)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, procedureName);
            addParameters?.Invoke(command);

            List<T> results = [];
            using SqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                results.Add(map(reader));

            return results;
        }

        protected async Task<T?> QuerySingleAsync<T>(string procedureName, Action<SqlCommand> addParameters, Func<SqlDataReader, T> map) where T : class
        {
            List<T> results = await QueryListAsync(procedureName, addParameters, map);
            return results.FirstOrDefault();
        }

        protected async Task<TId> InsertAsync<TId>(string procedureName, Action<SqlCommand> addParameters, string outputParameterName, SqlDbType outputParameterType)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, procedureName);
            addParameters(command);

            SqlParameter outputId = new(outputParameterName, outputParameterType)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(outputId);

            await command.ExecuteNonQueryAsync();
            return (TId)outputId.Value;
        }

        protected async Task<bool> ExecuteNonQueryAsync(string procedureName, Action<SqlCommand> addParameters)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, procedureName);
            addParameters(command);
            return await command.ExecuteNonQueryAsync() > 0;
        }

        protected async Task<bool> ExecuteExistsAsync(string procedureName, Action<SqlCommand> addParameters)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, procedureName);
            addParameters(command);
            return Convert.ToBoolean(await command.ExecuteScalarAsync());
        }

        protected async Task<string?> ExecuteScalarStringAsync(string procedureName, Action<SqlCommand> addParameters)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, procedureName);
            addParameters(command);
            object? result = await command.ExecuteScalarAsync();
            return result == null || result == DBNull.Value ? null : result.ToString();
        }
    }
}