using School.DTO.PermissionDTOs;

namespace School.BLL.Interfaces
{
    public interface IPermissionService
    {
        Task<List<PermissionDTO>> GetAllPermissionsAsync();

        Task<PermissionDTO?> GetPermissionByIdAsync(int permissionId);

        Task<PermissionDTO?> GetPermissionByNameAsync(string permissionName);

        Task<int> AddPermissionAsync(PermissionDTO permission);

        Task<bool> UpdatePermissionAsync(PermissionDTO permission);

        Task<bool> DeletePermissionAsync(int permissionId);
    }
}