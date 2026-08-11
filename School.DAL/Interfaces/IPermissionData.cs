using School.DTO.PermissionDTOs.Requests;
using School.DTO.PermissionDTOs.Responses;

namespace School.DAL.Interfaces
{
    public interface IPermissionData
    {
        Task<List<PermissionResponse>> GetAllPermissionsAsync();

        Task<PermissionResponse?> GetPermissionByIdAsync(int permissionId);

        Task<PermissionResponse?> GetPermissionByNameAsync(string permissionName);

        Task<int> AddPermissionAsync(CreatePermissionRequest permission);

        Task<bool> UpdatePermissionAsync(int permissionId, UpdatePermissionRequest permission);

        Task<bool> DeletePermissionAsync(int permissionId);

        Task<bool> IsPermissionExistAsync(int permissionId);
    }
}