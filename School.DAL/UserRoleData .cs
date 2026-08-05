using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DAL.Interfaces;
using School.DTO.AssociationsDTOs.UserRoleDTOs;
using System.Data;

namespace School.DAL
{
    public class UserRoleData : BaseData, IUserRoleData
    {
        public UserRoleData(IConfiguration configuration) : base(configuration) { }

        #region Helper Methods

        private static UserRoleDetailsDTO MapUserRole(SqlDataReader reader)
        {
            return new UserRoleDetailsDTO
            {
                UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                Username = reader.GetString(reader.GetOrdinal("Username")),
                IsUserActive = reader.GetBoolean(reader.GetOrdinal("IsUserActive")),
                RoleID = reader.GetInt32(reader.GetOrdinal("RoleID")),
                RoleName = reader.GetString(reader.GetOrdinal("RoleName")),
                RoleDescription = reader.IsDBNull(reader.GetOrdinal("RoleDescription")) ? null : reader.GetString(reader.GetOrdinal("RoleDescription")),
                IsRoleActive = reader.GetBoolean(reader.GetOrdinal("IsRoleActive"))
            };
        }

        private static void AddParameters(SqlCommand command, UserRoleDTO userRole)
        {
            command.Parameters.Add("@UserID", SqlDbType.Int).Value = userRole.UserID;
            command.Parameters.Add("@RoleID", SqlDbType.Int).Value = userRole.RoleID;
        }

        #endregion

        #region Public Methods

        public Task<List<UserRoleDetailsDTO>> GetAllUserRolesAsync() =>
            QueryListAsync("SP_GetAllUserRoles", null, MapUserRole);

        public Task<UserRoleDetailsDTO?> GetUserRoleAsync(int userId, int roleId) =>
            QuerySingleAsync("SP_GetUserRole",
                cmd =>
                {
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                    cmd.Parameters.Add("@RoleID", SqlDbType.Int).Value = roleId;
                },
                MapUserRole);

        public Task<List<UserRoleDetailsDTO>> GetRolesByUserIdAsync(int userId) =>
            QueryListAsync("SP_GetRolesByUserID",
                cmd =>
                {
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                }, MapUserRole);

        public Task<bool> AddUserRoleAsync(UserRoleDTO userRole) =>
            ExecuteNonQueryAsync("SP_AddUserRole", cmd => AddParameters(cmd, userRole));

        public Task<bool> DeleteUserRoleAsync(int userId, int roleId) =>
            ExecuteNonQueryAsync("SP_DeleteUserRole",
                cmd =>
                {
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                    cmd.Parameters.Add("@RoleID", SqlDbType.Int).Value = roleId;
                });

        public Task<bool> IsUserRoleExistAsync(int userId, int roleId) =>
            ExecuteExistsAsync("SP_IsUserRoleExist",
                cmd =>
                {
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                    cmd.Parameters.Add("@RoleID", SqlDbType.Int).Value = roleId;
                });
        #endregion
    }
}