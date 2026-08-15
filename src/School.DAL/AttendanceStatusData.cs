using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DAL.Interfaces;
using School.DTO.AttendanceStatusDTOs.Requests;
using School.DTO.AttendanceStatusDTOs.Responses;
using System.Data;

namespace School.DAL
{
    public class AttendanceStatusData : BaseData, IAttendanceStatusData
    {
        public AttendanceStatusData(IConfiguration configuration) : base(configuration) { }

        #region Helper Methods

        private static AttendanceStatusResponse MapAttendanceStatus(SqlDataReader reader)
        {
            return new AttendanceStatusResponse
            {
                StatusID = reader.GetInt32(reader.GetOrdinal("StatusID")),
                StatusName = reader.GetString(reader.GetOrdinal("StatusName"))
            };
        }

        private static void AddParameters(SqlCommand command, AttendanceStatusRequest status)
        {
            command.Parameters.Add("@StatusName", SqlDbType.NVarChar).Value = status.StatusName.Trim();
        }

        #endregion

        #region Public Methods

        public Task<List<AttendanceStatusResponse>> GetAllAttendanceStatusesAsync() =>
            QueryListAsync("SP_GetAllAttendanceStatuses", null, MapAttendanceStatus);

        public Task<AttendanceStatusResponse?> GetAttendanceStatusByIdAsync(int statusId) =>
            QuerySingleAsync("SP_GetAttendanceStatusByID", cmd => cmd.Parameters.Add("@StatusID", SqlDbType.Int).Value = statusId,
                MapAttendanceStatus);

        public Task<AttendanceStatusResponse?> GetAttendanceStatusByNameAsync(string statusName) =>
            QuerySingleAsync("SP_GetAttendanceStatusByName", cmd => cmd.Parameters.Add("@StatusName", SqlDbType.NVarChar).Value = statusName.Trim(),
                MapAttendanceStatus);

        public Task<int> AddAttendanceStatusAsync(AttendanceStatusRequest status) =>
            InsertAsync<int>("SP_AddAttendanceStatus", cmd => AddParameters(cmd, status), "@StatusID", SqlDbType.Int);

        public Task<bool> UpdateAttendanceStatusAsync(int statusID, AttendanceStatusRequest status) =>
            ExecuteNonQueryAsync("SP_UpdateAttendanceStatus",
                cmd =>
                {
                    cmd.Parameters.Add("@StatusID", SqlDbType.Int).Value = statusID;
                    AddParameters(cmd, status);
                });

        public Task<bool> DeleteAttendanceStatusAsync(int statusId) =>
            ExecuteNonQueryAsync("SP_DeleteAttendanceStatus",
                cmd => cmd.Parameters.Add("@StatusID", SqlDbType.Int).Value = statusId);

        public Task<bool> IsAttendanceStatusExistAsync(int statusId) =>
            ExecuteExistsAsync("SP_IsAttendanceStatusExists", cmd => cmd.Parameters.Add("@StatusID", SqlDbType.Int).Value = statusId);

        #endregion
    }
}