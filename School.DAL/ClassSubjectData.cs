using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DAL.Interfaces;
using School.DTO.AssociationsDTOs.ClassSubjectDTOs;
using System.Data;

namespace School.DAL
{
    public class ClassSubjectData : BaseData, IClassSubjectData
    {
        public ClassSubjectData(IConfiguration configuration) : base(configuration) { }

        #region Helper Methods

        private static ClassSubjectDetailsDTO MapClassSubjectDetails(SqlDataReader reader)
        {
            return new ClassSubjectDetailsDTO
            {
                ClassSubjectID = reader.GetInt32(reader.GetOrdinal("ClassSubjectID")),
                GradeID = reader.GetByte(reader.GetOrdinal("GradeID")),
                GradeName = reader.GetString(reader.GetOrdinal("GradeName")),
                ClassID = reader.GetInt32(reader.GetOrdinal("ClassID")),
                ClassName = reader.GetString(reader.GetOrdinal("ClassName")),
                AcademicYear = reader.GetString(reader.GetOrdinal("AcademicYear")),
                SubjectID = reader.GetInt32(reader.GetOrdinal("SubjectID")),
                SubjectName = reader.GetString(reader.GetOrdinal("SubjectName")),
                TeacherID = reader.GetInt32(reader.GetOrdinal("TeacherID")),
                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                SecondName = reader.GetString(reader.GetOrdinal("SecondName")),
                ThirdName = reader.GetString(reader.GetOrdinal("ThirdName")),
                LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? null : reader.GetString(reader.GetOrdinal("LastName"))
            };
        }

        private static void AddParameters(SqlCommand command, ClassSubjectDTO classSubject)
        {
            command.Parameters.Add("@ClassID", SqlDbType.Int).Value = classSubject.ClassID;
            command.Parameters.Add("@TeacherID", SqlDbType.Int).Value = classSubject.TeacherID;
            command.Parameters.Add("@SubjectID", SqlDbType.TinyInt).Value = classSubject.SubjectID;
        }

        #endregion

        #region Public Methods

        public Task<List<ClassSubjectDetailsDTO>> GetAllClassSubjectsAsync() =>
            QueryListAsync("SP_GetAllClassSubjects", null, MapClassSubjectDetails);

        public Task<ClassSubjectDetailsDTO?> GetClassSubjectByIdAsync(int classSubjectId) =>
            QuerySingleAsync("SP_GetClassSubjectByID", cmd => cmd.Parameters.Add("@ClassSubjectID", SqlDbType.Int).Value = classSubjectId,
                MapClassSubjectDetails);

        public Task<List<ClassSubjectDetailsDTO>> GetClassSubjectsByClassIdAsync(int classId) =>
            QueryListAsync("SP_GetClassSubjectsByClassID", cmd => cmd.Parameters.Add("@ClassID", SqlDbType.Int).Value = classId,
                MapClassSubjectDetails);

        public Task<List<ClassSubjectDetailsDTO>> GetClassSubjectsByTeacherIdAsync(int teacherId) =>
            QueryListAsync("SP_GetClassSubjectsByTeacherID", cmd => cmd.Parameters.Add("@TeacherID", SqlDbType.Int).Value = teacherId,
                MapClassSubjectDetails);

        public Task<List<ClassSubjectDetailsDTO>> GetClassSubjectsBySubjectIdAsync(int subjectId) =>
            QueryListAsync("SP_GetClassSubjectsBySubjectID", cmd => cmd.Parameters.Add("@SubjectID", SqlDbType.Int).Value = subjectId,
                MapClassSubjectDetails);

        public Task<ClassSubjectDetailsDTO?> GetClassSubjectByDetailsAsync(int classId, int teacherId, int subjectId) =>
            QuerySingleAsync("SP_GetClassSubjectByDetails",
                cmd =>
                {
                    cmd.Parameters.Add("@ClassID", SqlDbType.Int).Value = classId;
                    cmd.Parameters.Add("@TeacherID", SqlDbType.Int).Value = teacherId;
                    cmd.Parameters.Add("@SubjectID", SqlDbType.TinyInt).Value = subjectId;
                },
                MapClassSubjectDetails);

        public Task<int> AddClassSubjectAsync(ClassSubjectDTO classSubject) =>
            InsertAsync<int>("SP_AddClassSubject", cmd => AddParameters(cmd, classSubject), "@ClassSubjectID", SqlDbType.Int);

        public Task<bool> UpdateClassSubjectAsync(ClassSubjectDTO classSubject) =>
            ExecuteNonQueryAsync("SP_UpdateClassSubject",
                cmd =>
                {
                    cmd.Parameters.Add("@ClassSubjectID", SqlDbType.Int).Value = classSubject.ClassSubjectID;
                    AddParameters(cmd, classSubject);
                });

        public Task<bool> DeleteClassSubjectAsync(int classSubjectId) =>
            ExecuteNonQueryAsync("SP_DeleteClassSubject", cmd => cmd.Parameters.Add("@ClassSubjectID", SqlDbType.Int).Value = classSubjectId);

        public Task<bool> IsClassSubjectExistAsync(int classSubjectId) =>
            ExecuteExistsAsync("SP_IsClassSubjectExists", cmd => cmd.Parameters.Add("@ClassSubjectID", SqlDbType.Int).Value = classSubjectId);

        #endregion
    }
}