using School.DTO.AssociationsDTOs.RolePermissionDTOs;

namespace School.BLL.Interfaces
{
    public interface IRolePermissionService
    {
        Task<List<RolePermissionResponse>> GetAllRolePermissionsAsync();

        Task<RolePermissionResponse?> GetRolePermissionAsync(int roleId, int permissionId);

        Task<List<RolePermissionResponse>> GetPermissionsByRoleIdAsync(int roleId);

        Task<bool> AddRolePermissionAsync(RolePermissionRequest rolePermission);

        Task<bool> DeleteRolePermissionAsync(int roleId, int permissionId);
    }
}
