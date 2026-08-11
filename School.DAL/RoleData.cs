using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DAL.Interfaces;
using School.DTO.RoleDTOs.Requests;
using School.DTO.RoleDTOs.Responses;
using System.Data;

namespace School.DAL
{
    public class RoleData : BaseData, IRoleData
    {
        public RoleData(IConfiguration configuration) : base(configuration) { }
        #region Helper Methods
        private static RoleResponse MapRoleData(SqlDataReader reader)
        {
            return new RoleResponse
            {
                RoleID = reader.GetInt32(reader.GetOrdinal("RoleID")),
                RoleName = reader.GetString(reader.GetOrdinal("RoleName")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            };
        }

        private static void AddParameters(SqlCommand command, CreateRoleRequest role)
        {
            command.Parameters.Add("@RoleName", SqlDbType.NVarChar).Value = role.RoleName.Trim();
            command.Parameters.Add("@Description", SqlDbType.NVarChar).Value = (object?)role.Description ?? DBNull.Value;
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = role.IsActive;
        }
        #endregion
        #region Public Methods
        public Task<int> AddRoleAsync(CreateRoleRequest role) =>
            InsertAsync<int>("SP_Add_Role", cmd => AddParameters(cmd, role), "@RoleID",
                SqlDbType.Int);

        public Task<bool> DeleteRoleAsync(int roleId) =>
              ExecuteNonQueryAsync("SP_DeleteRole", cmd => cmd.Parameters.Add("@RoleID", SqlDbType.Int).Value = roleId);

        public Task<List<RoleResponse>> GetAllRolesAsync() => QueryListAsync("SP_GetAllRoles", null, MapRoleData);

        public Task<RoleResponse?> GetRoleByIdAsync(int roleId) =>
            QuerySingleAsync("SP_GetRoleByID", cmd => cmd.Parameters.Add("@RoleID", SqlDbType.Int).Value = roleId, MapRoleData);

        public Task<RoleResponse?> GetRoleByNameAsync(string roleName) =>
            QuerySingleAsync("SP_GetRoleByName", cmd => cmd.Parameters.Add("@RoleName", SqlDbType.NVarChar).Value = roleName.Trim(), MapRoleData);

        public Task<bool> IsRoleExistAsync(int roleId) =>
                        ExecuteExistsAsync("SP_IsRoleExist", cmd => cmd.Parameters.Add("@RoleID", SqlDbType.Int).Value = roleId);

        public Task<bool> UpdateRoleAsync(int roleId, UpdateRoleRequest role) =>
            ExecuteNonQueryAsync("SP_UpdateRole", cmd =>
            {
                cmd.Parameters.Add("@RoleID", SqlDbType.Int).Value = roleId;
                cmd.Parameters.Add("@RoleName", SqlDbType.NVarChar).Value = role.RoleName;
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = (object?)role.Description ?? DBNull.Value;
                cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = role.IsActive;
            });
        #endregion
    }
}