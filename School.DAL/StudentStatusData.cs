using Microsoft.Data.SqlClient;
using School.DTO.StudentStatusDTOs;
using System.Data;

namespace School.DAL
{
    public class StudentStatusData
    {
        private readonly string _connectionString;

        public StudentStatusData(string connectionString)
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

        private static StudentStatusDTO MapStudentStatus(SqlDataReader reader)
        {
            return new StudentStatusDTO
            {
                StatusID = reader.GetInt32(reader.GetOrdinal("StatusID")),
                StatusName = reader.GetString(reader.GetOrdinal("StatusName")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            };
        }

        private static void AddParameters(SqlCommand command, StudentStatusDTO status)
        {
            command.Parameters.Add("@StatusName", SqlDbType.NVarChar).Value =
                status.StatusName.Trim();
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = status.IsActive;
        }

        private static async Task<List<StudentStatusDTO>> ReadStudentStatusesAsync(SqlCommand command)
        {
            List<StudentStatusDTO> statuses = [];

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                statuses.Add(MapStudentStatus(reader));

            return statuses;
        }

        #endregion

        #region Public Methods

        public async Task<List<StudentStatusDTO>> GetAllStudentStatusesAsync()
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_GetAllStatuses", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            return await ReadStudentStatusesAsync(command);
        }

        public async Task<StudentStatusDTO?> GetStudentStatusByIdAsync(int statusId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_GetStatusByID", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@StatusID", SqlDbType.Int).Value = statusId;

            return (await ReadStudentStatusesAsync(command)).FirstOrDefault();
        }

        public async Task<StudentStatusDTO?> GetStudentStatusByNameAsync(string statusName)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_GetStatusByName", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@StatusName", SqlDbType.NVarChar).Value = statusName.Trim();

            return (await ReadStudentStatusesAsync(command)).FirstOrDefault();
        }

        public async Task<int> AddStudentStatusAsync(StudentStatusDTO status)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_AddStatus", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            AddParameters(command, status);

            SqlParameter outputStatusId = new("@StatusID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            command.Parameters.Add(outputStatusId);

            await command.ExecuteNonQueryAsync();

            return (int)outputStatusId.Value;
        }

        public async Task<bool> UpdateStudentStatusAsync(StudentStatusDTO status)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_UpdateStatus", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@StatusID", SqlDbType.Int).Value = status.StatusID;

            AddParameters(command, status);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteStudentStatusAsync(int statusId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_DeleteStatus", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@StatusID", SqlDbType.Int).Value = statusId;

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> IsStudentStatusExistAsync(int statusId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_IsStatusExists", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@StatusID", SqlDbType.Int).Value = statusId;

            return Convert.ToBoolean(await command.ExecuteScalarAsync());
        }

        #endregion
    }
}