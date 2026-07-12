using Microsoft.Data.SqlClient;
using School.DTO.StudentParentDTOs;
using System.Data;

namespace School.DAL
{
    public class StudentParentData
    {
        private readonly string _connectionString;

        public StudentParentData(string connectionString)
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

            using SqlCommand command = new("SP_GetAllStudentParents", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            return await ReadStudentParentDetailsAsync(command);
        }

        public async Task<List<StudentParentDetailsDTO>> GetParentsByStudentIdAsync(int studentId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_GetParentsByStudentID", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@StudentID", SqlDbType.Int).Value = studentId;

            return await ReadStudentParentDetailsAsync(command);
        }

        public async Task<List<StudentParentDetailsDTO>> GetStudentsByParentIdAsync(int parentId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_GetStudentsByParentID", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@ParentID", SqlDbType.Int).Value = parentId;

            return await ReadStudentParentDetailsAsync(command);
        }

        public async Task<bool> AddStudentParentAsync(StudentParentDTO relation)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_AddStudentParent", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            AddParameters(command, relation);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteStudentParentAsync(StudentParentDTO relation)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_DeleteStudentParent", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            AddParameters(command, relation);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> IsStudentParentExistAsync(StudentParentDTO relation)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_IsStudentParentExists", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            AddParameters(command, relation);

            return Convert.ToBoolean(await command.ExecuteScalarAsync());
        }
        #endregion
    }
}
