using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DTO.AttendanceStatusDTOs;
using System.Data;
namespace School.DAL
{
    public class AttendanceStatusData : BaseData
    {
        public AttendanceStatusData(IConfiguration configuration) : base(configuration) { }
        #region Helper Methods

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
            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetAllAttendanceStatuses");
            return await ReadAttendanceStatusesAsync(command);
        }

        public async Task<AttendanceStatusDTO?> GetAttendanceStatusByIdAsync(int statusId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetAttendanceStatusByID");
            command.Parameters.Add("@StatusID", SqlDbType.Int).Value = statusId;
            return (await ReadAttendanceStatusesAsync(command)).FirstOrDefault();
        }

        public async Task<AttendanceStatusDTO?> GetAttendanceStatusByNameAsync(string statusName)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetAttendanceStatusByName");
            command.Parameters.Add("@StatusName", SqlDbType.NVarChar).Value = statusName.Trim();
            return (await ReadAttendanceStatusesAsync(command)).FirstOrDefault();
        }

        public async Task<int> AddAttendanceStatusAsync(AttendanceStatusDTO status)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_AddAttendanceStatus");
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
            using SqlCommand command = CreateStoredProcedure(connection, "SP_UpdateAttendanceStatus");
            command.Parameters.Add("@StatusID", SqlDbType.Int).Value = status.StatusID;
            AddParameters(command, status);
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAttendanceStatusAsync(int statusId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_DeleteAttendanceStatus");
            command.Parameters.Add("@StatusID", SqlDbType.Int).Value = statusId;
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> IsAttendanceStatusExistAsync(int statusId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_IsAttendanceStatusExists");
            command.Parameters.Add("@StatusID", SqlDbType.Int).Value = statusId;
            return Convert.ToBoolean(await command.ExecuteScalarAsync());
        }

        #endregion
    }
}