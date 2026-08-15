using School.BLL.Common;
using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.PermissionDTOs.Requests;
using School.DTO.PermissionDTOs.Responses;

namespace School.BLL
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionData _permissionData;
        private static int MinPermissionNameLength => 2;
        private static int MaxPermissionNameLength => 100;
        public PermissionService(IPermissionData permissionData)
        {
            _permissionData = permissionData;
        }

        #region Validation

        private static void ValidatePermission(CreatePermissionRequest permission)
        {
            ArgumentNullException.ThrowIfNull(permission);

            permission.PermissionName = ValidationHelper.ValidateString(
                permission.PermissionName, nameof(permission.PermissionName), MinPermissionNameLength, MaxPermissionNameLength);

            ValidateDescription(permission.Description);
        }

        private static void ValidatePermission(UpdatePermissionRequest permission)
        {
            ArgumentNullException.ThrowIfNull(permission);

            permission.PermissionName = ValidationHelper.ValidateString(
                permission.PermissionName, nameof(permission.PermissionName), MinPermissionNameLength, MaxPermissionNameLength);

            ValidateDescription(permission.Description);
        }

        private static void ValidateDescription(string? description)
        {
            if (description?.Length > 255) throw new ArgumentException("Description cannot exceed 255 characters.", nameof(description));
        }

        #endregion

        #region Public
        public Task<List<PermissionResponse>> GetAllPermissionsAsync()
        {
            return _permissionData.GetAllPermissionsAsync();
        }

        public async Task<PermissionResponse> GetPermissionByIdAsync(int permissionId)
        {
            ValidationHelper.ValidateId(permissionId);

            PermissionResponse? permission = await _permissionData.GetPermissionByIdAsync(permissionId);

            if (permission == null)
                throw new KeyNotFoundException($"Permission with ID {permissionId} does not exist.");

            return permission;
        }

        public async Task<PermissionResponse> GetPermissionByNameAsync(string permissionName)
        {
            permissionName = ValidationHelper.ValidateString(permissionName, nameof(permissionName), MinPermissionNameLength, MaxPermissionNameLength);

            PermissionResponse? permission = await _permissionData.GetPermissionByNameAsync(permissionName);

            if (permission == null)
                throw new KeyNotFoundException($"Permission '{permissionName}' does not exist.");

            return permission;
        }

        public async Task<int> AddPermissionAsync(CreatePermissionRequest permission)
        {
            ValidatePermission(permission);

            await EnsureHelper.EnsureUniqueAsync(_permissionData.GetPermissionByNameAsync, permission.PermissionName);

            int permissionId = await _permissionData.AddPermissionAsync(permission);

            if (permissionId <= 0)
                throw new InvalidOperationException("Failed to add permission.");

            return permissionId;
        }

        public async Task<bool> UpdatePermissionAsync(int permissionID, UpdatePermissionRequest permission)
        {
            ValidatePermission(permission);
            ValidationHelper.ValidateId(permissionID);

            await EnsureHelper.EnsureExistsAsync(_permissionData.IsPermissionExistAsync, permissionID, "Permission");
            await EnsureHelper.EnsureUniqueAsync(_permissionData.GetPermissionByNameAsync, permission.PermissionName, p => p.PermissionID, permissionID);

            bool isUpdated = await _permissionData.UpdatePermissionAsync(permissionID, permission);

            if (!isUpdated)
                throw new InvalidOperationException($"Failed to update permission with ID {permissionID}.");

            return isUpdated;
        }

        public async Task<bool> DeletePermissionAsync(int permissionId)
        {
            ValidationHelper.ValidateId(permissionId);

            await EnsureHelper.EnsureExistsAsync(_permissionData.IsPermissionExistAsync, permissionId, "Permission");

            bool isDeleted = await _permissionData.DeletePermissionAsync(permissionId);

            if (!isDeleted)
                throw new InvalidOperationException(
                    $"Failed to delete permission with ID {permissionId}.");

            return isDeleted;
        }
        #endregion
    }
}