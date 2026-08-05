using School.DTO.PermissionDTOs;

namespace School.DAL.Interfaces
{
    public interface IPermissionData
    {
        Task<List<PermissionDTO>> GetAllPermissionsAsync();

        Task<PermissionDTO?> GetPermissionByIdAsync(int permissionId);

        Task<PermissionDTO?> GetPermissionByNameAsync(string permissionName);

        Task<int> AddPermissionAsync(PermissionDTO permission);

        Task<bool> UpdatePermissionAsync(PermissionDTO permission);

        Task<bool> DeletePermissionAsync(int permissionId);

        Task<bool> IsPermissionExistAsync(int permissionId);
    }
}