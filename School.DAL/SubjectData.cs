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

        #endregion

        #region Public Methods

        public Task<List<SubjectDTO>> GetAllSubjectsAsync() =>
            QueryListAsync("SP_GetAllSubjects", null, MapSubject);

        public Task<SubjectDTO?> GetSubjectByIdAsync(int subjectId) =>
            QuerySingleAsync("SP_GetSubjectByID", cmd => cmd.Parameters.Add("@SubjectID", SqlDbType.Int).Value = subjectId,
                MapSubject);

        public Task<SubjectDTO?> GetSubjectByNameAsync(string subjectName) =>
            QuerySingleAsync("SP_GetSubjectByName", cmd => cmd.Parameters.Add("@SubjectName", SqlDbType.NVarChar).Value = subjectName,
                MapSubject);

        public Task<int> AddSubjectAsync(SubjectDTO subject) =>
            InsertAsync<int>("SP_AddSubject", cmd => AddParameters(cmd, subject), "@SubjectID", SqlDbType.Int);

        public Task<bool> UpdateSubjectAsync(SubjectDTO subject) =>
            ExecuteNonQueryAsync("SP_UpdateSubject",
                cmd =>
                {
                    cmd.Parameters.Add("@SubjectID", SqlDbType.Int).Value = subject.SubjectID;
                    AddParameters(cmd, subject);
                });

        public Task<bool> DeleteSubjectAsync(int subjectId) =>
            ExecuteNonQueryAsync("SP_DeleteSubject", cmd => cmd.Parameters.Add("@SubjectID", SqlDbType.Int).Value = subjectId);

        public Task<bool> IsSubjectExistAsync(int subjectId) =>
            ExecuteExistsAsync("SP_IsSubjectExists", cmd => cmd.Parameters.Add("@SubjectID", SqlDbType.Int).Value = subjectId);

        #endregion
    }
}