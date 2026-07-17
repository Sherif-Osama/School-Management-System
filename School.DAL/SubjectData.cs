using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DAL.Interfaces;
using School.DTO.SubjectDTO;
using System.Data;

namespace School.DAL
{
    public class SubjectData : BaseData, ISubjectData
    {
        public SubjectData(IConfiguration configuration) : base(configuration) { }

        #region Helper Methods
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

            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetAllSubjects");

            return await ReadSubjectsAsync(command);
        }

        public async Task<SubjectDTO?> GetSubjectByIdAsync(int subjectId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetSubjectByID");

            command.Parameters.Add("@SubjectID", SqlDbType.Int).Value = subjectId;

            return (await ReadSubjectsAsync(command)).FirstOrDefault();
        }

        public async Task<SubjectDTO?> GetSubjectByNameAsync(string subjectName)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetSubjectByName");
            command.Parameters.Add("@SubjectName", SqlDbType.NVarChar).Value = subjectName;
            return (await ReadSubjectsAsync(command)).FirstOrDefault();
        }

        public async Task<int> AddSubjectAsync(SubjectDTO subject)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = CreateStoredProcedure(connection, "SP_AddSubject");

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

            using SqlCommand command = CreateStoredProcedure(connection, "SP_UpdateSubject");

            command.Parameters.Add("@SubjectID", SqlDbType.Int).Value = subject.SubjectID;

            AddParameters(command, subject);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteSubjectAsync(int subjectId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = CreateStoredProcedure(connection, "SP_DeleteSubject");

            command.Parameters.Add("@SubjectID", SqlDbType.Int).Value = subjectId;

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> IsSubjectExistAsync(int subjectId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = CreateStoredProcedure(connection, "SP_IsSubjectExists");

            command.Parameters.Add("@SubjectID", SqlDbType.Int).Value = subjectId;

            return Convert.ToBoolean(await command.ExecuteScalarAsync());
        }
        #endregion
    }
}