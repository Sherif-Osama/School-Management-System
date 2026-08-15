using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DAL.Interfaces;
using School.DTO.PermissionDTOs.Requests;
using School.DTO.PermissionDTOs.Responses;
using System.Data;

namespace School.DAL
{
    public class PermissionData : BaseData, IPermissionData
    {
        public PermissionData(IConfiguration configuration) : base(configuration) { }

        #region Helper Methods
        private static PermissionResponse MapPermission(SqlDataReader reader)
        {
            return new PermissionResponse
            {
                PermissionID = reader.GetInt32(reader.GetOrdinal("PermissionID")),
                PermissionName = reader.GetString(reader.GetOrdinal("PermissionName")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            };
        }

        private static void AddParameters(SqlCommand command, CreatePermissionRequest permission)
        {
            command.Parameters.Add("@PermissionName", SqlDbType.NVarChar).Value = permission.PermissionName.Trim();
            command.Parameters.Add("@Description", SqlDbType.NVarChar).Value = (object?)permission.Description ?? DBNull.Value;
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = permission.IsActive;
        }
        #endregion

        #region Public Methods
        public Task<List<PermissionResponse>> GetAllPermissionsAsync() => QueryListAsync("SP_GetAllPermissions", null, MapPermission);

        public Task<PermissionResponse?> GetPermissionByIdAsync(int permissionId) =>
            QuerySingleAsync("SP_GetPermissionByID", cmd => cmd.Parameters.Add("@PermissionID", SqlDbType.Int).Value = permissionId, MapPermission);

        public Task<PermissionResponse?> GetPermissionByNameAsync(string permissionName) =>
            QuerySingleAsync("SP_GetPermissionByName", cmd => cmd.Parameters.Add("@PermissionName", SqlDbType.NVarChar).Value = permissionName.Trim(), MapPermission);

        public Task<int> AddPermissionAsync(CreatePermissionRequest permission) =>
            InsertAsync<int>("SP_Add_Permission", cmd => AddParameters(cmd, permission), "@PermissionID", SqlDbType.Int);

        public Task<bool> UpdatePermissionAsync(int permissionId, UpdatePermissionRequest permission) =>
            ExecuteNonQueryAsync("SP_UpdatePermission",
                cmd =>
                {
                    cmd.Parameters.Add("@PermissionID", SqlDbType.Int).Value = permissionId;
                    cmd.Parameters.Add("@PermissionName", SqlDbType.NVarChar).Value = permission.PermissionName.Trim();
                    cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = (object?)permission.Description ?? DBNull.Value;
                    cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = permission.IsActive;
                });

        public Task<bool> DeletePermissionAsync(int permissionId) =>
            ExecuteNonQueryAsync("SP_DeletePermission", cmd => cmd.Parameters.Add("@PermissionID", SqlDbType.Int).Value = permissionId);

        public Task<bool> IsPermissionExistAsync(int permissionId) =>
            ExecuteExistsAsync("SP_IsPermissionExist", cmd => cmd.Parameters.Add("@PermissionID", SqlDbType.Int).Value = permissionId);
        #endregion
    }
}