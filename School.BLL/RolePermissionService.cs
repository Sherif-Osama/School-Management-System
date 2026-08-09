using School.BLL.Common;
using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.AssociationsDTOs.RolePermissionDTOs;

namespace School.BLL
{
    public class RolePermissionService : IRolePermissionService
    {
        private readonly IRolePermissionData _rolePermissionData;
        private readonly IRoleData _roleData;
        private readonly IPermissionData _permissionData;

        public RolePermissionService(IRolePermissionData rolePermissionData, IRoleData roleData, IPermissionData permissionData)
        {
            _rolePermissionData = rolePermissionData;
            _roleData = roleData;
            _permissionData = permissionData;
        }

        #region Validation

        private static void ValidateRolePermission(RolePermissionDTO rolePermission)
        {
            ArgumentNullException.ThrowIfNull(rolePermission);

            ValidationHelper.ValidateId(rolePermission.RoleID);
            ValidationHelper.ValidateId(rolePermission.PermissionID);
        }

        #endregion

        #region Ensure
        private async Task EnsureRolePermissionExistsAsync(int roleId, int permissionId)
        {
            if (!await _rolePermissionData.IsRolePermissionExistAsync(roleId, permissionId))
                throw new KeyNotFoundException("The role-permission relationship does not exist.");
        }

        private async Task EnsureRolePermissionUniqueAsync(int roleId, int permissionId)
        {
            if (await _rolePermissionData.IsRolePermissionExistAsync(roleId, permissionId))
                throw new InvalidOperationException("This permission is already assigned to the role.");
        }
        #endregion

        #region Public

        public Task<List<RolePermissionDetailsDTO>> GetAllRolePermissionsAsync() => _rolePermissionData.GetAllRolePermissionsAsync();


        public async Task<RolePermissionDetailsDTO?> GetRolePermissionAsync(int roleId, int permissionId)
        {
            ValidationHelper.ValidateId(roleId);
            ValidationHelper.ValidateId(permissionId);

            RolePermissionDetailsDTO? rolePermission = await _rolePermissionData.GetRolePermissionAsync(roleId, permissionId);

            if (rolePermission == null)
                throw new KeyNotFoundException("The role-permission relationship does not exist.");

            return rolePermission;
        }

        public async Task<List<RolePermissionDetailsDTO>> GetPermissionsByRoleIdAsync(int roleId)
        {
            ValidationHelper.ValidateId(roleId);

            await EnsureHelper.EnsureExistsAsync(_roleData.IsRoleExistAsync, roleId, "Role");

            return await _rolePermissionData.GetPermissionsByRoleIdAsync(roleId);
        }

        public async Task<bool> AddRolePermissionAsync(RolePermissionDTO rolePermission)
        {
            ValidateRolePermission(rolePermission);

            await EnsureHelper.EnsureExistsAsync(_roleData.IsRoleExistAsync, rolePermission.RoleID, "Role");
            await EnsureHelper.EnsureExistsAsync(_permissionData.IsPermissionExistAsync, rolePermission.PermissionID, "Permission");
            await EnsureRolePermissionUniqueAsync(rolePermission.RoleID, rolePermission.PermissionID);

            bool isAdded = await _rolePermissionData.AddRolePermissionAsync(rolePermission);

            if (!isAdded)
                throw new InvalidOperationException("Failed to assign permission to role.");

            return true;
        }

        public async Task<bool> DeleteRolePermissionAsync(int roleId, int permissionId)
        {
            ValidationHelper.ValidateId(roleId);
            ValidationHelper.ValidateId(permissionId);

            await EnsureRolePermissionExistsAsync(roleId, permissionId);

            bool isDeleted = await _rolePermissionData.DeleteRolePermissionAsync(roleId, permissionId);

            if (!isDeleted)
                throw new InvalidOperationException("Failed to remove permission from role.");

            return true;
        }

        #endregion
    }
}
