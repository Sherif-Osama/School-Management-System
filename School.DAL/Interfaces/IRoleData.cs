using School.DTO.RoleDTOs;

namespace School.DAL.Interfaces
{
    public interface IRoleData
    {
        Task<List<RoleDTO>> GetAllRolesAsync();
        Task<RoleDTO?> GetRoleByIdAsync(int roleId);
        Task<RoleDTO?> GetRoleByNameAsync(string roleName);
        Task<int> AddRoleAsync(RoleDTO role);
        Task<bool> UpdateRoleAsync(RoleDTO role);
        Task<bool> IsRoleExistAsync(int roleId);
        Task<bool> DeleteRoleAsync(int roleId);
    }
}