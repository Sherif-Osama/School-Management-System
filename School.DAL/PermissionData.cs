using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DAL.Interfaces;
using School.DTO.PermissionDTOs;
using System.Data;

namespace School.DAL
{
    public class PermissionData : BaseData, IPermissionData
    {
        public PermissionData(IConfiguration configuration) : base(configuration) { }

        #region Helper Methods
        private static PermissionDTO MapPermission(SqlDataReader reader)
        {
            return new PermissionDTO
            {
                PermissionID = reader.GetInt32(reader.GetOrdinal("PermissionID")),
                PermissionName = reader.GetString(reader.GetOrdinal("PermissionName")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            };
        }

        private static void AddParameters(SqlCommand command, PermissionDTO permission)
        {
            command.Parameters.Add("@PermissionName", SqlDbType.NVarChar).Value = permission.PermissionName.Trim();
            command.Parameters.Add("@Description", SqlDbType.NVarChar).Value = (object?)permission.Description ?? DBNull.Value;
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = permission.IsActive;
        }
        #endregion

        #region Public Methods
        public Task<List<PermissionDTO>> GetAllPermissionsAsync() => QueryListAsync("SP_GetAllPermissions", null, MapPermission);

        public Task<PermissionDTO?> GetPermissionByIdAsync(int permissionId) =>
            QuerySingleAsync("SP_GetPermissionByID", cmd => cmd.Parameters.Add("@PermissionID", SqlDbType.Int).Value = permissionId, MapPermission);

        public Task<PermissionDTO?> GetPermissionByNameAsync(string permissionName) =>
            QuerySingleAsync("SP_GetPermissionByName", cmd => cmd.Parameters.Add("@PermissionName", SqlDbType.NVarChar).Value = permissionName.Trim(), MapPermission);

        public Task<int> AddPermissionAsync(PermissionDTO permission) =>
            InsertAsync<int>("SP_Add_Permission", cmd => AddParameters(cmd, permission), "@PermissionID", SqlDbType.Int);

        public Task<bool> UpdatePermissionAsync(PermissionDTO permission) =>
            ExecuteNonQueryAsync("SP_UpdatePermission",
                cmd =>
                {
                    cmd.Parameters.Add("@PermissionID", SqlDbType.Int).Value = permission.PermissionID;
                    AddParameters(cmd, permission);
                });

        public Task<bool> DeletePermissionAsync(int permissionId) =>
            ExecuteNonQueryAsync("SP_DeletePermission", cmd => cmd.Parameters.Add("@PermissionID", SqlDbType.Int).Value = permissionId);

        public Task<bool> IsPermissionExistAsync(int permissionId) =>
            ExecuteExistsAsync("SP_IsPermissionExist", cmd => cmd.Parameters.Add("@PermissionID", SqlDbType.Int).Value = permissionId);
        #endregion
    }
}