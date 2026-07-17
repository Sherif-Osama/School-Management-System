using Microsoft.Data.SqlClient;
using School.DTO.ExamTypeDTOs;
using System.Data;

namespace School.DAL
{
    public class ExamTypeData
    {
        private readonly string _connectionString;

        public ExamTypeData(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region Helper Methods

        private async Task<SqlConnection> GetOpenConnectionAsync()
        {
            SqlConnection connection = new(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

        private static ExamTypeDTO MapExamType(SqlDataReader reader)
        {
            return new ExamTypeDTO
            {
                ExamTypeID = reader.GetInt32(reader.GetOrdinal("ExamTypeID")),
                ExamName = reader.GetString(reader.GetOrdinal("ExamName"))
            };
        }

        private static void AddParameters(SqlCommand command, ExamTypeDTO examType)
        {
            command.Parameters.Add("@ExamName", SqlDbType.NVarChar).Value =
                examType.ExamName.Trim();
        }

        private static async Task<List<ExamTypeDTO>> ReadExamTypesAsync(SqlCommand command)
        {
            List<ExamTypeDTO> examTypes = [];

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                examTypes.Add(MapExamType(reader));

            return examTypes;
        }

        #endregion

        #region Public Methods

        public async Task<List<ExamTypeDTO>> GetAllExamTypesAsync()
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_GetAllExamTypes", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            return await ReadExamTypesAsync(command);
        }

        public async Task<ExamTypeDTO?> GetExamTypeByIdAsync(int examTypeId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_GetExamTypeByID", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@ExamTypeID", SqlDbType.Int).Value = examTypeId;

            return (await ReadExamTypesAsync(command)).FirstOrDefault();
        }

        public async Task<ExamTypeDTO?> GetExamTypeByNameAsync(string examName)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_GetExamTypeByName", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@ExamName", SqlDbType.NVarChar).Value = examName.Trim();

            return (await ReadExamTypesAsync(command)).FirstOrDefault();
        }

        public async Task<int> AddExamTypeAsync(ExamTypeDTO examType)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_AddExamType", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            AddParameters(command, examType);

            SqlParameter outputExamTypeId = new("@ExamTypeID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            command.Parameters.Add(outputExamTypeId);

            await command.ExecuteNonQueryAsync();

            return (int)outputExamTypeId.Value;
        }

        public async Task<bool> UpdateExamTypeAsync(ExamTypeDTO examType)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_UpdateExamType", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@ExamTypeID", SqlDbType.Int).Value = examType.ExamTypeID;

            AddParameters(command, examType);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteExamTypeAsync(int examTypeId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_DeleteExamType", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@ExamTypeID", SqlDbType.Int).Value = examTypeId;

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> IsExamTypeExistAsync(int examTypeId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_IsExamTypeExists", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@ExamTypeID", SqlDbType.Int).Value = examTypeId;

            return Convert.ToBoolean(await command.ExecuteScalarAsync());
        }

        #endregion
    }
}
