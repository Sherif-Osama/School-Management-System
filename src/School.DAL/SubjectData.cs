using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DAL.Interfaces;
using School.DTO.SubjectDTOs.Requests;
using School.DTO.SubjectDTOs.Responses;
using System.Data;

namespace School.DAL
{
    public class SubjectData : BaseData, ISubjectData
    {
        public SubjectData(IConfiguration configuration) : base(configuration) { }

        #region Helper Methods

        private static SubjectResponse MapSubject(SqlDataReader reader)
        {
            return new SubjectResponse
            {
                SubjectID = reader.GetInt32(reader.GetOrdinal("SubjectID")),
                SubjectName = reader.GetString(reader.GetOrdinal("SubjectName")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            };
        }

        private static void AddParameters(SqlCommand command, CreateSubjectRequest subject)
        {
            command.Parameters.Add("@SubjectName", SqlDbType.NVarChar).Value = subject.SubjectName;
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = subject.IsActive;
        }

        #endregion

        #region Public Methods

        public Task<List<SubjectResponse>> GetAllSubjectsAsync() =>
            QueryListAsync("SP_GetAllSubjects", null, MapSubject);

        public Task<SubjectResponse?> GetSubjectByIdAsync(int subjectId) =>
            QuerySingleAsync("SP_GetSubjectByID", cmd => cmd.Parameters.Add("@SubjectID", SqlDbType.Int).Value = subjectId,
                MapSubject);

        public Task<SubjectResponse?> GetSubjectByNameAsync(string subjectName) =>
            QuerySingleAsync("SP_GetSubjectByName", cmd => cmd.Parameters.Add("@SubjectName", SqlDbType.NVarChar).Value = subjectName,
                MapSubject);

        public Task<int> AddSubjectAsync(CreateSubjectRequest subject) =>
            InsertAsync<int>("SP_AddSubject", cmd => AddParameters(cmd, subject), "@SubjectID", SqlDbType.Int);

        public Task<bool> UpdateSubjectAsync(int subjectId, UpdateSubjectRequest subject) =>
            ExecuteNonQueryAsync("SP_UpdateSubject",
                cmd =>
                {
                    cmd.Parameters.Add("@SubjectID", SqlDbType.Int).Value = subjectId;
                    cmd.Parameters.Add("@SubjectName", SqlDbType.NVarChar).Value = subject.SubjectName;
                    cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = subject.IsActive;
                });

        public Task<bool> DeleteSubjectAsync(int subjectId) =>
            ExecuteNonQueryAsync("SP_DeleteSubject", cmd => cmd.Parameters.Add("@SubjectID", SqlDbType.Int).Value = subjectId);

        public Task<bool> IsSubjectExistAsync(int subjectId) =>
            ExecuteExistsAsync("SP_IsSubjectExists", cmd => cmd.Parameters.Add("@SubjectID", SqlDbType.Int).Value = subjectId);

        #endregion
    }
}