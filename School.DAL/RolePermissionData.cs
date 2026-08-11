using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DAL.Interfaces;
using School.DTO.AssociationsDTOs.RolePermissionDTOs;
using System.Data;

namespace School.DAL
{
    public class RolePermissionData : BaseData, IRolePermissionData
    {
        public RolePermissionData(IConfiguration configuration)
            : base(configuration)
        {
        }

        #region Helper Methods

        private static RolePermissionResponse MapRolePermission(SqlDataReader reader)
        {
            return new RolePermissionResponse
            {
                RoleID = reader.GetInt32(reader.GetOrdinal("RoleID")),
                RoleName = reader.GetString(reader.GetOrdinal("RoleName")),
                IsRoleActive = reader.GetBoolean(reader.GetOrdinal("IsRoleActive")),
                PermissionID = reader.GetInt32(reader.GetOrdinal("PermissionID")),
                PermissionName = reader.GetString(reader.GetOrdinal("PermissionName")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                IsPermissionActive = reader.GetBoolean(reader.GetOrdinal("IsPermissionActive"))
            };
        }

        private static void AddParameters(SqlCommand command, RolePermissionRequest rolePermission)
        {
            command.Parameters.Add("@RoleID", SqlDbType.Int).Value = rolePermission.RoleID;
            command.Parameters.Add("@PermissionID", SqlDbType.Int).Value = rolePermission.PermissionID;
        }

        private static string MapPermissionName(SqlDataReader reader)
        {
            return reader.GetString(reader.GetOrdinal("PermissionName"));
        }
        #endregion

        #region Public Methods
        public Task<List<RolePermissionResponse>> GetAllRolePermissionsAsync() =>
            QueryListAsync("SP_GetAllRolePermissions", null, MapRolePermission);

        public Task<RolePermissionResponse?> GetRolePermissionAsync(int roleId, int permissionId) =>
            QuerySingleAsync("SP_GetRolePermission",
                cmd =>
                {
                    cmd.Parameters.Add("@RoleID", SqlDbType.Int).Value = roleId;
                    cmd.Parameters.Add("@PermissionID", SqlDbType.Int).Value = permissionId;
                }, MapRolePermission);

        public Task<List<RolePermissionResponse>> GetPermissionsByRoleIdAsync(int roleId) =>
            QueryListAsync("SP_GetPermissionsByRoleID",
                cmd =>
                {
                    cmd.Parameters.Add("@RoleID", SqlDbType.Int).Value = roleId;
                }, MapRolePermission);

        public Task<List<string>> GetPermissionNamesByUserIdAsync(int userId) =>
    QueryListAsync("SP_GetUserPermissionNames",
        cmd =>
        {
            cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
        }, MapPermissionName);

        public Task<bool> AddRolePermissionAsync(RolePermissionRequest rolePermission) =>
          ExecuteNonQueryAsync("SP_AddRolePermission", cmd => AddParameters(cmd, rolePermission));

        public Task<bool> DeleteRolePermissionAsync(int roleId, int permissionId) =>
            ExecuteNonQueryAsync("SP_DeleteRolePermission",
                cmd =>
                {
                    cmd.Parameters.Add("@RoleID", SqlDbType.Int).Value = roleId;
                    cmd.Parameters.Add("@PermissionID", SqlDbType.Int).Value = permissionId;
                });

        public Task<bool> IsRolePermissionExistAsync(int roleId, int permissionId) =>
            ExecuteExistsAsync("SP_IsRolePermissionExist",
                cmd =>
                {
                    cmd.Parameters.Add("@RoleID", SqlDbType.Int).Value = roleId;
                    cmd.Parameters.Add("@PermissionID", SqlDbType.Int).Value = permissionId;
                });
        #endregion
    }
}