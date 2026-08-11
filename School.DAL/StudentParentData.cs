using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DAL.Interfaces;
using School.DTO.AssociationsDTOs.StudentParentDTOs;
using School.DTO.AssociationsDTOs.StudentParentDTOs.Requests;
using System.Data;

namespace School.DAL
{
    public class StudentParentData : BaseData, IStudentParentData
    {
        public StudentParentData(IConfiguration configuration) : base(configuration) { }

        #region Helper Methods

        private static StudentParentResponse MapStudentParentDetails(SqlDataReader reader)
        {
            return new StudentParentResponse
            {
                StudentID = reader.GetInt32(reader.GetOrdinal("StudentID")),
                StudentName = reader.GetString(reader.GetOrdinal("StudentName")),
                ParentID = reader.GetInt32(reader.GetOrdinal("ParentID")),
                ParentName = reader.GetString(reader.GetOrdinal("ParentName"))
            };
        }

        private static void AddParameters(SqlCommand command, StudentParentRequest studentParent)
        {
            command.Parameters.Add("@StudentID", SqlDbType.Int).Value = studentParent.StudentID;
            command.Parameters.Add("@ParentID", SqlDbType.Int).Value = studentParent.ParentID;
        }

        #endregion

        #region Public Methods

        public Task<List<StudentParentResponse>> GetAllStudentParentsAsync() =>
            QueryListAsync("SP_GetAllStudentParents", null, MapStudentParentDetails);

        public Task<List<StudentParentResponse>> GetParentsByStudentIdAsync(int studentId) =>
            QueryListAsync("SP_GetParentsByStudentID", cmd => cmd.Parameters.Add("@StudentID", SqlDbType.Int).Value = studentId,
                MapStudentParentDetails);

        public Task<List<StudentParentResponse>> GetStudentsByParentIdAsync(int parentId) =>
            QueryListAsync("SP_GetStudentsByParentID", cmd => cmd.Parameters.Add("@ParentID", SqlDbType.Int).Value = parentId,
                MapStudentParentDetails);

        public Task<bool> AddStudentParentAsync(StudentParentRequest relation) =>
            ExecuteNonQueryAsync("SP_AddStudentParent", cmd => AddParameters(cmd, relation));

        public Task<bool> DeleteStudentParentAsync(StudentParentRequest relation) =>
            ExecuteNonQueryAsync("SP_DeleteStudentParent", cmd => AddParameters(cmd, relation));

        public Task<bool> IsStudentParentExistAsync(StudentParentRequest relation) =>
            ExecuteExistsAsync("SP_IsStudentParentExists", cmd => AddParameters(cmd, relation));

        #endregion
    }
}