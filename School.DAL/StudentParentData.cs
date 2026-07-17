using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DAL.Interfaces;
using School.DTO.AssociationsDTOs.StudentParentDTOs;
using System.Data;
namespace School.DAL
{
    public class StudentParentData : BaseData, IStudentParentData
    {

        public StudentParentData(IConfiguration configuration) : base(configuration) { }

        #region Helper Methods
        private static StudentParentDetailsDTO MapStudentParentDetails(SqlDataReader reader)
        {
            return new StudentParentDetailsDTO
            {
                StudentID = reader.GetInt32(reader.GetOrdinal("StudentID")),
                StudentName = reader.GetString(reader.GetOrdinal("StudentName")),
                ParentID = reader.GetInt32(reader.GetOrdinal("ParentID")),
                ParentName = reader.GetString(reader.GetOrdinal("ParentName"))
            };
        }

        private static void AddParameters(SqlCommand command, StudentParentDTO studentParent)
        {
            command.Parameters.Add("@StudentID", SqlDbType.Int).Value = studentParent.StudentID;
            command.Parameters.Add("@ParentID", SqlDbType.Int).Value = studentParent.ParentID;
        }

        private static async Task<List<StudentParentDetailsDTO>> ReadStudentParentDetailsAsync(SqlCommand command)
        {
            List<StudentParentDetailsDTO> studentParentDetails = [];

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                studentParentDetails.Add(MapStudentParentDetails(reader));
            }

            return studentParentDetails;
        }
        #endregion

        #region Public Method

        public async Task<List<StudentParentDetailsDTO>> GetAllStudentParentsAsync()
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetAllStudentParents");

            return await ReadStudentParentDetailsAsync(command);
        }

        public async Task<List<StudentParentDetailsDTO>> GetParentsByStudentIdAsync(int studentId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetParentsByStudentID");

            command.Parameters.Add("@StudentID", SqlDbType.Int).Value = studentId;

            return await ReadStudentParentDetailsAsync(command);
        }

        public async Task<List<StudentParentDetailsDTO>> GetStudentsByParentIdAsync(int parentId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetStudentsByParentID");

            command.Parameters.Add("@ParentID", SqlDbType.Int).Value = parentId;

            return await ReadStudentParentDetailsAsync(command);
        }

        public async Task<bool> AddStudentParentAsync(StudentParentDTO relation)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = CreateStoredProcedure(connection, "SP_AddStudentParent");

            AddParameters(command, relation);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteStudentParentAsync(StudentParentDTO relation)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = CreateStoredProcedure(connection, "SP_DeleteStudentParent");

            AddParameters(command, relation);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> IsStudentParentExistAsync(StudentParentDTO relation)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = CreateStoredProcedure(connection, "SP_IsStudentParentExists");

            AddParameters(command, relation);

            return Convert.ToBoolean(await command.ExecuteScalarAsync());
        }
        #endregion
    }
}
