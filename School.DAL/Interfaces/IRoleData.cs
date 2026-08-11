using School.DTO.RoleDTOs.Requests;
using School.DTO.RoleDTOs.Responses;

namespace School.DAL.Interfaces
{
    public interface IRoleData
    {
        Task<List<RoleResponse>> GetAllRolesAsync();
        Task<RoleResponse?> GetRoleByIdAsync(int roleId);
        Task<RoleResponse?> GetRoleByNameAsync(string roleName);
        Task<int> AddRoleAsync(CreateRoleRequest role);
        Task<bool> UpdateRoleAsync(int roleId, UpdateRoleRequest role);
        Task<bool> IsRoleExistAsync(int roleId);
        Task<bool> DeleteRoleAsync(int roleId);
    }
}