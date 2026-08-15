using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DAL.Interfaces;
using School.DTO.StudentStatusDTOs.Requests;
using School.DTO.StudentStatusDTOs.Responses;
using System.Data;

namespace School.DAL
{
    public class StudentStatusData : BaseData, IStudentStatusData
    {
        public StudentStatusData(IConfiguration configuration) : base(configuration) { }

        #region Helper Methods

        private static StudentStatusResponse MapStudentStatus(SqlDataReader reader)
        {
            return new StudentStatusResponse
            {
                StatusID = reader.GetInt32(reader.GetOrdinal("StatusID")),
                StatusName = reader.GetString(reader.GetOrdinal("StatusName")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            };
        }

        private static void AddParameters(SqlCommand command, CreateStudentStatusRequest status)
        {
            command.Parameters.Add("@StatusName", SqlDbType.NVarChar).Value = status.StatusName.Trim();
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = status.IsActive;
        }

        #endregion

        #region Public Methods

        public Task<List<StudentStatusResponse>> GetAllStudentStatusesAsync() => QueryListAsync("SP_GetAllStatuses", null, MapStudentStatus);

        public Task<StudentStatusResponse?> GetStudentStatusByIdAsync(int statusId)
        {
            return QuerySingleAsync("SP_GetStatusByID", cmd => cmd.Parameters.Add("@StatusID", SqlDbType.Int).Value = statusId,
                MapStudentStatus);
        }

        public Task<StudentStatusResponse?> GetStudentStatusByNameAsync(string statusName)
        {
            return QuerySingleAsync("SP_GetStatusByName", cmd => cmd.Parameters.Add("@StatusName", SqlDbType.NVarChar).Value = statusName.Trim(),
                            MapStudentStatus);
        }


        public Task<int> AddStudentStatusAsync(CreateStudentStatusRequest status)
            => InsertAsync<int>("SP_AddStatus", cmd => AddParameters(cmd, status), "@StatusID", SqlDbType.Int);

        public Task<bool> UpdateStudentStatusAsync(int StatusId, UpdateStudentStatusRequest status) =>
            ExecuteNonQueryAsync("SP_UpdateStatus",
              cmd =>
            {
                cmd.Parameters.Add("@StatusID", SqlDbType.Int).Value = StatusId;
                cmd.Parameters.Add("@StatusName", SqlDbType.NVarChar).Value = status.StatusName;
                cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = status.IsActive;
            });

        public Task<bool> DeleteStudentStatusAsync(int statusId) => ExecuteNonQueryAsync("SP_DeleteStatus", cmd => cmd.Parameters.Add("@StatusID", SqlDbType.Int).Value = statusId);

        public Task<bool> IsStudentStatusExistAsync(int statusId) => ExecuteExistsAsync("SP_IsStatusExists", cmd => cmd.Parameters.Add("@StatusID", SqlDbType.Int).Value = statusId);
        #endregion
    }
}