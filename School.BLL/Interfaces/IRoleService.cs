using School.DTO.RoleDTOs.Requests;
using School.DTO.RoleDTOs.Responses;

namespace School.BLL.Interfaces
{
    public interface IRoleService
    {
        Task<List<RoleResponse>> GetAllRolesAsync();
        Task<RoleResponse?> GetRoleByIdAsync(int roleId);
        Task<RoleResponse?> GetRoleByNameAsync(string roleName);
        Task<int> AddRoleAsync(CreateRoleRequest role);
        Task<bool> UpdateRoleAsync(int roleId, UpdateRoleRequest role);
        Task<bool> DeleteRoleAsync(int roleId);
    }
}