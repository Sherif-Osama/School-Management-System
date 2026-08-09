using School.BLL.Common;
using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.RoleDTOs;

namespace School.BLL
{
    public class RoleService : IRoleService
    {
        private readonly IRoleData _roleData;
        private static int minRoleNameLenght => 3;
        private static int maxRoleNameLenght => 20;

        public RoleService(IRoleData roleData)
        {
            _roleData = roleData;
        }
        #region Validation

        private static void ValidateRole(RoleDTO role)
        {
            ArgumentNullException.ThrowIfNull(role);

            role.RoleName = ValidationHelper.ValidateString(role.RoleName, nameof(role.RoleName), minRoleNameLenght, maxRoleNameLenght);

            if (role.Description?.Length > 255)
                throw new ArgumentException("Description cannot exceed 255 characters.", nameof(role.Description));
        }

        #endregion

        #region Ensure

        private async Task EnsureRoleExistsAsync(int roleId)
        {
            if (!await _roleData.IsRoleExistAsync(roleId))
                throw new KeyNotFoundException($"Role with ID {roleId} does not exist.");
        }

        private async Task EnsureRoleNameUniqueAsync(string roleName, int? roleId = null)
        {
            RoleDTO? existingRole = await _roleData.GetRoleByNameAsync(roleName);

            if (existingRole != null && (roleId == null || existingRole.RoleID != roleId))
            {
                throw new InvalidOperationException($"Role '{roleName}' already exists.");
            }
        }

        #endregion

        #region Public

        public async Task<List<RoleDTO>> GetAllRolesAsync()
        {
            return await _roleData.GetAllRolesAsync();
        }

        public async Task<RoleDTO?> GetRoleByIdAsync(int roleId)
        {
            ValidationHelper.ValidateId(roleId);

            RoleDTO? role = await _roleData.GetRoleByIdAsync(roleId);

            if (role == null)
                throw new KeyNotFoundException($"Role with ID {roleId} does not exist.");

            return role;
        }

        public async Task<RoleDTO?> GetRoleByNameAsync(string roleName)
        {
            roleName = ValidationHelper.ValidateString(roleName, nameof(roleName), minRoleNameLenght, maxRoleNameLenght);

            RoleDTO? role = await _roleData.GetRoleByNameAsync(roleName);

            if (role == null)
                throw new KeyNotFoundException($"Role '{roleName}' does not exist.");

            return role;
        }

        public async Task<int> AddRoleAsync(RoleDTO role)
        {
            ValidateRole(role);

            await EnsureRoleNameUniqueAsync(role.RoleName);

            int newRoleId = await _roleData.AddRoleAsync(role);

            if (newRoleId <= 0)
                throw new InvalidOperationException("Failed to add role.");

            return newRoleId;
        }

        public async Task<bool> UpdateRoleAsync(RoleDTO role)
        {
            ValidateRole(role);
            ValidationHelper.ValidateId(role.RoleID);

            await EnsureRoleExistsAsync(role.RoleID);
            await EnsureRoleNameUniqueAsync(role.RoleName, role.RoleID);

            bool isUpdated = await _roleData.UpdateRoleAsync(role);

            if (!isUpdated)
                throw new InvalidOperationException($"Failed to update role with ID {role.RoleID}.");

            return isUpdated;
        }

        public async Task<bool> DeleteRoleAsync(int roleId)
        {
            ValidationHelper.ValidateId(roleId);

            await EnsureRoleExistsAsync(roleId);

            bool isDeleted = await _roleData.DeleteRoleAsync(roleId);

            if (!isDeleted)
                throw new InvalidOperationException($"Failed to delete role with ID {roleId}.");

            return isDeleted;
        }

        #endregion
    }
}