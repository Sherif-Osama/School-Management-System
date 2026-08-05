using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.RoleDTOs;

namespace School.BLL
{
    public class RoleService : IRoleService
    {
        private readonly IRoleData _roleData;

        public RoleService(IRoleData roleData)
        {
            _roleData = roleData;
        }

        #region Validation

        private static void ValidateRole(RoleDTO role)
        {
            ArgumentNullException.ThrowIfNull(role);

            role.RoleName = ValidateRoleName(role.RoleName);

            if (role.Description?.Length > 255)
                throw new ArgumentException("Description cannot exceed 255 characters.", nameof(role.Description));
        }

        private static void ValidateRoleId(int roleId)
        {
            if (roleId <= 0)
                throw new ArgumentException("RoleID must be a positive number.", nameof(roleId));
        }

        private static string ValidateRoleName(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentException("RoleName is required.", nameof(roleName));

            roleName = roleName.Trim();

            if (roleName.Length > 50)
                throw new ArgumentException("RoleName cannot exceed 50 characters.", nameof(roleName));

            return roleName;
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
                throw new InvalidOperationException(
                    $"Role '{roleName}' already exists.");
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
            ValidateRoleId(roleId);

            RoleDTO? role = await _roleData.GetRoleByIdAsync(roleId);

            if (role == null)
                throw new KeyNotFoundException($"Role with ID {roleId} does not exist.");

            return role;
        }

        public async Task<RoleDTO?> GetRoleByNameAsync(string roleName)
        {
            roleName = ValidateRoleName(roleName);

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
            ValidateRoleId(role.RoleID);
            ValidateRole(role);

            await EnsureRoleExistsAsync(role.RoleID);
            await EnsureRoleNameUniqueAsync(role.RoleName, role.RoleID);

            bool isUpdated = await _roleData.UpdateRoleAsync(role);

            if (!isUpdated)
                throw new InvalidOperationException($"Failed to update role with ID {role.RoleID}.");

            return isUpdated;
        }

        public async Task<bool> DeleteRoleAsync(int roleId)
        {
            ValidateRoleId(roleId);

            await EnsureRoleExistsAsync(roleId);

            bool isDeleted = await _roleData.DeleteRoleAsync(roleId);

            if (!isDeleted)
                throw new InvalidOperationException($"Failed to delete role with ID {roleId}.");

            return isDeleted;
        }

        #endregion
    }
}