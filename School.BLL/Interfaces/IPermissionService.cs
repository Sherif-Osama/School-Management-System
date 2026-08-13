using School.DTO.PermissionDTOs.Requests;
using School.DTO.PermissionDTOs.Responses;

namespace School.BLL.Interfaces
{
    public interface IPermissionService
    {
        Task<List<PermissionResponse>> GetAllPermissionsAsync();

        Task<PermissionResponse> GetPermissionByIdAsync(int permissionId);

        Task<PermissionResponse> GetPermissionByNameAsync(string permissionName);

        Task<int> AddPermissionAsync(CreatePermissionRequest permission);

        Task<bool> UpdatePermissionAsync(int permissionID, UpdatePermissionRequest permission);

        Task<bool> DeletePermissionAsync(int permissionId);
    }
}