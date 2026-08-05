using School.DTO.AssociationsDTOs.RolePermissionDTOs;

namespace School.BLL.Interfaces
{
    public interface IRolePermissionService
    {
        Task<List<RolePermissionDetailsDTO>> GetAllRolePermissionsAsync();

        Task<RolePermissionDetailsDTO?> GetRolePermissionAsync(int roleId, int permissionId);

        Task<List<RolePermissionDetailsDTO>> GetPermissionsByRoleIdAsync(int roleId);

        Task<bool> AddRolePermissionAsync(RolePermissionDTO rolePermission);

        Task<bool> DeleteRolePermissionAsync(int roleId, int permissionId);
    }
}
