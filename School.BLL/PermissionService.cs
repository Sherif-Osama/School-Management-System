using School.BLL.Common;
using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.PermissionDTOs;

namespace School.BLL
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionData _permissionData;
        private static int minPermissionNameLength => 2;
        private static int maxPermissionNameLength => 100;
        public PermissionService(IPermissionData permissionData)
        {
            _permissionData = permissionData;
        }

        #region Validation
        private static void ValidatePermission(PermissionDTO permission)
        {
            ArgumentNullException.ThrowIfNull(permission);

            permission.PermissionName = ValidationHelper.ValidateString(permission.PermissionName, nameof(permission.PermissionName), minPermissionNameLength, maxPermissionNameLength);

            if (permission.Description?.Length > 255)
                throw new ArgumentException("Description cannot exceed 255 characters.", nameof(permission.Description));
        }
        #endregion

        #region Ensure
        private async Task EnsurePermissionExistsAsync(int permissionId)
        {
            if (!await _permissionData.IsPermissionExistAsync(permissionId))
                throw new KeyNotFoundException($"Permission with ID {permissionId} does not exist.");
        }

        private async Task EnsurePermissionNameUniqueAsync(string permissionName, int? permissionId = null)
        {
            PermissionDTO? existingPermission = await _permissionData.GetPermissionByNameAsync(permissionName);

            if (existingPermission != null &&
                (permissionId == null || existingPermission.PermissionID != permissionId))
            {
                throw new InvalidOperationException(
                    $"Permission '{permissionName}' already exists.");
            }
        }
        #endregion

        #region Public
        public Task<List<PermissionDTO>> GetAllPermissionsAsync()
        {
            return _permissionData.GetAllPermissionsAsync();
        }

        public async Task<PermissionDTO?> GetPermissionByIdAsync(int permissionId)
        {
            ValidationHelper.ValidateId(permissionId);

            PermissionDTO? permission = await _permissionData.GetPermissionByIdAsync(permissionId);

            if (permission == null)
                throw new KeyNotFoundException($"Permission with ID {permissionId} does not exist.");

            return permission;
        }

        public async Task<PermissionDTO?> GetPermissionByNameAsync(string permissionName)
        {
            permissionName = ValidationHelper.ValidateString(permissionName, nameof(permissionName), minPermissionNameLength, maxPermissionNameLength);

            PermissionDTO? permission = await _permissionData.GetPermissionByNameAsync(permissionName);

            if (permission == null)
                throw new KeyNotFoundException($"Permission '{permissionName}' does not exist.");

            return permission;
        }

        public async Task<int> AddPermissionAsync(PermissionDTO permission)
        {
            ValidatePermission(permission);

            await EnsurePermissionNameUniqueAsync(permission.PermissionName);

            int permissionId = await _permissionData.AddPermissionAsync(permission);

            if (permissionId <= 0)
                throw new InvalidOperationException("Failed to add permission.");

            return permissionId;
        }

        public async Task<bool> UpdatePermissionAsync(PermissionDTO permission)
        {
            ValidationHelper.ValidateId(permission.PermissionID);
            ValidatePermission(permission);

            await EnsurePermissionExistsAsync(permission.PermissionID);
            await EnsurePermissionNameUniqueAsync(permission.PermissionName, permission.PermissionID);

            bool isUpdated = await _permissionData.UpdatePermissionAsync(permission);

            if (!isUpdated)
                throw new InvalidOperationException($"Failed to update permission with ID {permission.PermissionID}.");

            return isUpdated;
        }

        public async Task<bool> DeletePermissionAsync(int permissionId)
        {
            ValidationHelper.ValidateId(permissionId);

            await EnsurePermissionExistsAsync(permissionId);

            bool isDeleted = await _permissionData.DeletePermissionAsync(permissionId);

            if (!isDeleted)
                throw new InvalidOperationException(
                    $"Failed to delete permission with ID {permissionId}.");

            return isDeleted;
        }
        #endregion
    }
}