using Microsoft.Data.SqlClient;
using School.DTO.SubjectDTO;
using System.Data;

namespace School.DAL
{
    public class SubjectData
    {
        private readonly string _connectionString;

        public SubjectData(string connectionString)
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

        private static SubjectDTO MapSubject(SqlDataReader reader)
        {
            return new SubjectDTO
            {
                SubjectID = reader.GetInt32(reader.GetOrdinal("SubjectID")),
                SubjectName = reader.GetString(reader.GetOrdinal("SubjectName")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            };
        }

        private static void AddParameters(SqlCommand command, SubjectDTO subject)
        {
            command.Parameters.Add("@SubjectName", SqlDbType.NVarChar).Value = subject.SubjectName;
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = subject.IsActive;
        }

        private static async Task<List<SubjectDTO>> ReadSubjectsAsync(SqlCommand command)
        {
            List<SubjectDTO> subjects = [];

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                subjects.Add(MapSubject(reader));
            }

            return subjects;
        }
        #endregion

        #region Public Methods
        public async Task<List<SubjectDTO>> GetAllSubjectsAsync()
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_GetAllSubjects", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            return await ReadSubjectsAsync(command);
        }

        public async Task<SubjectDTO?> GetSubjectByIdAsync(int subjectId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_GetSubjectByID", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@SubjectID", SqlDbType.Int).Value = subjectId;

            return (await ReadSubjectsAsync(command)).FirstOrDefault();
        }

        public async Task<SubjectDTO?> GetSubjectByNameAsync(string subjectName)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_GetSubjectByName", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@SubjectName", SqlDbType.NVarChar).Value = subjectName;

            return (await ReadSubjectsAsync(command)).FirstOrDefault();
        }

        public async Task<int> AddSubjectAsync(SubjectDTO subject)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_AddSubject", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            AddParameters(command, subject);

            SqlParameter outputSubjectId = new("@SubjectID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            command.Parameters.Add(outputSubjectId);

            await command.ExecuteNonQueryAsync();

            return (int)outputSubjectId.Value;
        }

        public async Task<bool> UpdateSubjectAsync(SubjectDTO subject)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_UpdateSubject", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@SubjectID", SqlDbType.Int).Value = subject.SubjectID;

            AddParameters(command, subject);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteSubjectAsync(int subjectId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_DeleteSubject", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@SubjectID", SqlDbType.Int).Value = subjectId;

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> IsSubjectExistAsync(int subjectId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_IsSubjectExists", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@SubjectID", SqlDbType.Int).Value = subjectId;

            return Convert.ToBoolean(await command.ExecuteScalarAsync());
        }
        #endregion
    }
}