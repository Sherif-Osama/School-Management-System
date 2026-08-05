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

        private static RolePermissionDetailsDTO MapRolePermission(SqlDataReader reader)
        {
            return new RolePermissionDetailsDTO
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

        private static void AddParameters(SqlCommand command, RolePermissionDTO rolePermission)
        {
            command.Parameters.Add("@RoleID", SqlDbType.Int).Value = rolePermission.RoleID;
            command.Parameters.Add("@PermissionID", SqlDbType.Int).Value = rolePermission.PermissionID;
        }
        #endregion

        #region Public Methods
        public Task<List<RolePermissionDetailsDTO>> GetAllRolePermissionsAsync() =>
            QueryListAsync("SP_GetAllRolePermissions", null, MapRolePermission);

        public Task<RolePermissionDetailsDTO?> GetRolePermissionAsync(int roleId, int permissionId) =>
            QuerySingleAsync("SP_GetRolePermission",
                cmd =>
                {
                    cmd.Parameters.Add("@RoleID", SqlDbType.Int).Value = roleId;
                    cmd.Parameters.Add("@PermissionID", SqlDbType.Int).Value = permissionId;
                }, MapRolePermission);

        public Task<List<RolePermissionDetailsDTO>> GetPermissionsByRoleIdAsync(int roleId) =>
            QueryListAsync("SP_GetPermissionsByRoleID",
                cmd =>
                {
                    cmd.Parameters.Add("@RoleID", SqlDbType.Int).Value = roleId;
                }, MapRolePermission);

        public Task<bool> AddRolePermissionAsync(RolePermissionDTO rolePermission) =>
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