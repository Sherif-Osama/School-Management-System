using Microsoft.Data.SqlClient;
using School.DTO.AttendanceStatusDTOs;
using System.Data;

namespace School.DAL
{
    public class AttendanceStatusData
    {
        private readonly string _connectionString;

        public AttendanceStatusData(string connectionString)
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

        private static AttendanceStatusDTO MapAttendanceStatus(SqlDataReader reader)
        {
            return new AttendanceStatusDTO
            {
                StatusID = reader.GetInt32(reader.GetOrdinal("StatusID")),
                StatusName = reader.GetString(reader.GetOrdinal("StatusName"))
            };
        }

        private static void AddParameters(SqlCommand command, AttendanceStatusDTO status)
        {
            command.Parameters.Add("@StatusName", SqlDbType.NVarChar).Value = status.StatusName.Trim();
        }

        private static async Task<List<AttendanceStatusDTO>> ReadAttendanceStatusesAsync(SqlCommand command)
        {
            List<AttendanceStatusDTO> statuses = [];

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                statuses.Add(MapAttendanceStatus(reader));

            return statuses;
        }

        #endregion

        #region Public Methods

        public async Task<List<AttendanceStatusDTO>> GetAllAttendanceStatusesAsync()
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_GetAllAttendanceStatuses", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            return await ReadAttendanceStatusesAsync(command);
        }

        public async Task<AttendanceStatusDTO?> GetAttendanceStatusByIdAsync(int statusId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_GetAttendanceStatusByID", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@StatusID", SqlDbType.Int).Value = statusId;

            return (await ReadAttendanceStatusesAsync(command)).FirstOrDefault();
        }

        public async Task<AttendanceStatusDTO?> GetAttendanceStatusByNameAsync(string statusName)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_GetAttendanceStatusByName", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@StatusName", SqlDbType.NVarChar).Value = statusName.Trim();

            return (await ReadAttendanceStatusesAsync(command)).FirstOrDefault();
        }

        public async Task<int> AddAttendanceStatusAsync(AttendanceStatusDTO status)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_AddAttendanceStatus", connection)
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

        public async Task<bool> UpdateAttendanceStatusAsync(AttendanceStatusDTO status)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_UpdateAttendanceStatus", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@StatusID", SqlDbType.Int).Value = status.StatusID;

            AddParameters(command, status);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAttendanceStatusAsync(int statusId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_DeleteAttendanceStatus", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@StatusID", SqlDbType.Int).Value = statusId;

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> IsAttendanceStatusExistAsync(int statusId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_IsAttendanceStatusExists", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@StatusID", SqlDbType.Int).Value = statusId;

            return Convert.ToBoolean(await command.ExecuteScalarAsync());
        }

        #endregion
    }
}