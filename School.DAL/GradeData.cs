using Microsoft.Data.SqlClient;
using School.DTO.GradesDTOs;
using System.Data;

namespace School.DAL
{
    public class GradeData
    {
        private readonly string _connectionString;

        public GradeData(string connectionString)
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

        private static GradeDTO MapGrade(SqlDataReader reader)
        {
            return new GradeDTO
            {
                GradeID = reader.GetByte(reader.GetOrdinal("GradeID")),
                GradeName = reader.GetString(reader.GetOrdinal("GradeName"))
            };
        }

        private static void AddParameters(SqlCommand command, GradeDTO grade)
        {
            command.Parameters.Add("@GradeName", SqlDbType.NVarChar).Value = grade.GradeName;
        }

        private static async Task<List<GradeDTO>> ReadGradesAsync(SqlCommand command)
        {
            List<GradeDTO> grades = [];

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                grades.Add(MapGrade(reader));
            }

            return grades;
        }

        #endregion

        #region Public Methods

        public async Task<List<GradeDTO>> GetAllGradesAsync()
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_GetAllGrades", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            return await ReadGradesAsync(command);
        }

        public async Task<GradeDTO?> GetGradeByIdAsync(byte gradeId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_GetGradeByID", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@GradeID", SqlDbType.TinyInt).Value = gradeId;

            return (await ReadGradesAsync(command)).FirstOrDefault();
        }

        public async Task<GradeDTO?> GetGradeByNameAsync(string gradeName)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_GetGradeByName", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@GradeName", SqlDbType.NVarChar).Value = gradeName;

            return (await ReadGradesAsync(command)).FirstOrDefault();
        }

        public async Task<int> AddGradeAsync(GradeDTO grade)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_AddGrade", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            AddParameters(command, grade);

            SqlParameter outputGradeId = new("@GradeID", SqlDbType.TinyInt)
            {
                Direction = ParameterDirection.Output
            };

            command.Parameters.Add(outputGradeId);

            await command.ExecuteNonQueryAsync();

            return (int)outputGradeId.Value;
        }

        public async Task<bool> UpdateGradeAsync(GradeDTO grade)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_UpdateGrade", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@GradeID", SqlDbType.TinyInt).Value = grade.GradeID;

            AddParameters(command, grade);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteGradeAsync(byte gradeId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_DeleteGrade", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@GradeID", SqlDbType.TinyInt).Value = gradeId;

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> IsGradeExistAsync(byte gradeId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_IsGradeExists", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@GradeID", SqlDbType.TinyInt).Value = gradeId;

            return Convert.ToBoolean(await command.ExecuteScalarAsync());
        }

        #endregion
    }
}