using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DAL.Interfaces;
using School.DTO.AssociationsDTOs.TeacherSubjectDTOs.Requests;
using School.DTO.AssociationsDTOs.TeacherSubjectDTOs.Responses;
using System.Data;

namespace School.DAL
{
    public class TeacherSubjectData : BaseData, ITeacherSubjectData
    {
        public TeacherSubjectData(IConfiguration configuration) : base(configuration) { }

        #region Helper Methods

        private static TeacherSubjectResponse MapTeacherSubjectDetails(SqlDataReader reader)
        {
            return new TeacherSubjectResponse
            {
                TeacherID = reader.GetInt32(reader.GetOrdinal("TeacherID")),
                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                SecondName = reader.GetString(reader.GetOrdinal("SecondName")),
                ThirdName = reader.GetString(reader.GetOrdinal("ThirdName")),
                LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? null : reader.GetString(reader.GetOrdinal("LastName")),
                SubjectID = reader.GetInt32(reader.GetOrdinal("SubjectID")),
                SubjectName = reader.GetString(reader.GetOrdinal("SubjectName"))
            };
        }

        private static void AddParameters(SqlCommand command, TeacherSubjectRequest teacherSubject)
        {
            command.Parameters.Add("@TeacherID", SqlDbType.Int).Value = teacherSubject.TeacherID;
            command.Parameters.Add("@SubjectID", SqlDbType.Int).Value = teacherSubject.SubjectID;
        }

        #endregion

        #region Public Methods

        public Task<List<TeacherSubjectResponse>> GetAllTeacherSubjectsAsync() =>
            QueryListAsync("SP_GetAllTeacherSubjects", null, MapTeacherSubjectDetails);

        public Task<List<TeacherSubjectResponse>> GetSubjectsByTeacherIdAsync(int teacherId) =>
            QueryListAsync("SP_GetSubjectsByTeacherID", cmd => cmd.Parameters.Add("@TeacherID", SqlDbType.Int).Value = teacherId,
                MapTeacherSubjectDetails);

        public Task<List<TeacherSubjectResponse>> GetTeachersBySubjectIdAsync(int subjectId) =>
            QueryListAsync("SP_GetTeachersBySubjectID", cmd => cmd.Parameters.Add("@SubjectID", SqlDbType.Int).Value = subjectId,
                MapTeacherSubjectDetails);

        public Task<bool> AssignSubjectToTeacherAsync(TeacherSubjectRequest relation) =>
            ExecuteNonQueryAsync("SP_AssignSubjectToTeacher", cmd => AddParameters(cmd, relation));

        public Task<bool> RemoveSubjectFromTeacherAsync(TeacherSubjectRequest relation) =>
            ExecuteNonQueryAsync("SP_RemoveSubjectFromTeacher", cmd => AddParameters(cmd, relation));

        public Task<bool> IsTeacherSubjectExistAsync(TeacherSubjectRequest relation) =>
            ExecuteExistsAsync("SP_IsTeacherSubjectExists", cmd => AddParameters(cmd, relation));

        #endregion
    }
}