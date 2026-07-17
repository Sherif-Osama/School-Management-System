using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DAL.Interfaces;
using School.DTO.ExamTypeDTOs;
using System.Data;

namespace School.DAL
{
    public class ExamTypeData : BaseData, IExamTypeData
    {
        public ExamTypeData(IConfiguration configuration) : base(configuration) { }

        #region Helper Methods
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
            command.Parameters.Add("@ExamName", SqlDbType.NVarChar).Value = examType.ExamName.Trim();
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

            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetAllExamTypes");

            return await ReadExamTypesAsync(command);
        }

        public async Task<ExamTypeDTO?> GetExamTypeByIdAsync(int examTypeId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetExamTypeByID");

            command.Parameters.Add("@ExamTypeID", SqlDbType.Int).Value = examTypeId;

            return (await ReadExamTypesAsync(command)).FirstOrDefault();
        }

        public async Task<ExamTypeDTO?> GetExamTypeByNameAsync(string examName)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetExamTypeByName");

            command.Parameters.Add("@ExamName", SqlDbType.NVarChar).Value = examName.Trim();

            return (await ReadExamTypesAsync(command)).FirstOrDefault();
        }

        public async Task<int> AddExamTypeAsync(ExamTypeDTO examType)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = CreateStoredProcedure(connection, "SP_AddExamType");

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

            using SqlCommand command = CreateStoredProcedure(connection, "SP_UpdateExamType");

            command.Parameters.Add("@ExamTypeID", SqlDbType.Int).Value = examType.ExamTypeID;

            AddParameters(command, examType);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteExamTypeAsync(int examTypeId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = CreateStoredProcedure(connection, "SP_DeleteExamType");

            command.Parameters.Add("@ExamTypeID", SqlDbType.Int).Value = examTypeId;

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> IsExamTypeExistAsync(int examTypeId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = CreateStoredProcedure(connection, "SP_IsExamTypeExists");

            command.Parameters.Add("@ExamTypeID", SqlDbType.Int).Value = examTypeId;

            return Convert.ToBoolean(await command.ExecuteScalarAsync());
        }

        #endregion
    }
}
