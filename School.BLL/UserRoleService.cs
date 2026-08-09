using School.BLL.Common;
using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.AssociationsDTOs.UserRoleDTOs;

namespace School.BLL
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IUserRoleData _userRoleData;
        private readonly IUserData _userData;
        private readonly IRoleData _roleData;

        public UserRoleService(IUserRoleData userRoleData, IUserData userData, IRoleData roleData)
        {
            _userRoleData = userRoleData;
            _userData = userData;
            _roleData = roleData;
        }

        #region Validation
        private static void ValidateUserRole(UserRoleDTO userRole)
        {
            ArgumentNullException.ThrowIfNull(userRole);

            ValidationHelper.ValidateId(userRole.UserID);
            ValidationHelper.ValidateId(userRole.RoleID);
        }
        #endregion

        #region Ensure
        private async Task EnsureUserExistsAsync(int userId)
        {
            if (!await _userData.IsUserExistAsync(userId))
                throw new KeyNotFoundException($"User with ID {userId} does not exist.");
        }

        private async Task EnsureRoleExistsAsync(int roleId)
        {
            if (!await _roleData.IsRoleExistAsync(roleId))
                throw new KeyNotFoundException($"Role with ID {roleId} does not exist.");
        }

        private async Task EnsureUserRoleExistsAsync(int userId, int roleId)
        {
            if (!await _userRoleData.IsUserRoleExistAsync(userId, roleId))
                throw new KeyNotFoundException("The user-role relationship does not exist.");
        }

        private async Task EnsureUserRoleUniqueAsync(int userId, int roleId)
        {
            if (await _userRoleData.IsUserRoleExistAsync(userId, roleId))
                throw new InvalidOperationException("This role is already assigned to the user.");
        }
        #endregion

        #region Public
        public Task<List<UserRoleDetailsDTO>> GetAllUserRolesAsync() => _userRoleData.GetAllUserRolesAsync();

        public async Task<UserRoleDetailsDTO?> GetUserRoleAsync(int userId, int roleId)
        {
            ValidationHelper.ValidateId(userId);
            ValidationHelper.ValidateId(roleId);

            UserRoleDetailsDTO? userRole =
                await _userRoleData.GetUserRoleAsync(userId, roleId);

            if (userRole == null)
                throw new KeyNotFoundException("The user-role relationship does not exist.");

            return userRole;
        }

        public async Task<List<UserRoleDetailsDTO>> GetRolesByUserIdAsync(int userId)
        {
            ValidationHelper.ValidateId(userId);

            await EnsureUserExistsAsync(userId);

            return await _userRoleData.GetRolesByUserIdAsync(userId);
        }

        public async Task<bool> AddUserRoleAsync(UserRoleDTO userRole)
        {
            ValidateUserRole(userRole);

            await EnsureUserExistsAsync(userRole.UserID);
            await EnsureRoleExistsAsync(userRole.RoleID);
            await EnsureUserRoleUniqueAsync(userRole.UserID, userRole.RoleID);

            if (!await _userRoleData.AddUserRoleAsync(userRole))
                throw new InvalidOperationException("Failed to assign role to user.");

            return true;
        }

        public async Task<bool> DeleteUserRoleAsync(int userId, int roleId)
        {
            ValidationHelper.ValidateId(userId);
            ValidationHelper.ValidateId(roleId);

            await EnsureUserRoleExistsAsync(userId, roleId);

            if (!await _userRoleData.DeleteUserRoleAsync(userId, roleId))
                throw new InvalidOperationException("Failed to remove role from user.");

            return true;
        }

        #endregion
    }
}