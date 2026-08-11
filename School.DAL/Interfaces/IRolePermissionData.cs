using School.DTO.AssociationsDTOs.RolePermissionDTOs;

namespace School.DAL.Interfaces
{
    public interface IRolePermissionData
    {
        Task<List<RolePermissionResponse>> GetAllRolePermissionsAsync();

        Task<RolePermissionResponse?> GetRolePermissionAsync(int roleId, int permissionId);

        Task<List<RolePermissionResponse>> GetPermissionsByRoleIdAsync(int roleId);

        Task<bool> AddRolePermissionAsync(RolePermissionRequest rolePermission);

        Task<bool> DeleteRolePermissionAsync(int roleId, int permissionId);

        Task<bool> IsRolePermissionExistAsync(int roleId, int permissionId);

        Task<List<string>> GetPermissionNamesByUserIdAsync(int userId);
    }
}