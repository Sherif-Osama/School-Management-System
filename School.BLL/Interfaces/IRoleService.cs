using School.DTO.RoleDTOs;

namespace School.BLL.Interfaces
{
    public interface IRoleService
    {
        Task<List<RoleDTO>> GetAllRolesAsync();
        Task<RoleDTO?> GetRoleByIdAsync(int roleId);
        Task<RoleDTO?> GetRoleByNameAsync(string roleName);
        Task<int> AddRoleAsync(RoleDTO role);
        Task<bool> UpdateRoleAsync(RoleDTO role);
        Task<bool> DeleteRoleAsync(int roleId);
    }
}