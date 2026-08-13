using School.BLL.Common;
using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.AssociationsDTOs.UserRoleDTOs.Requests;
using School.DTO.AssociationsDTOs.UserRoleDTOs.Responses;

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
        private static void ValidateUserRole(UserRoleRequest userRole)
        {
            ArgumentNullException.ThrowIfNull(userRole);

            ValidationHelper.ValidateId(userRole.UserID);
            ValidationHelper.ValidateId(userRole.RoleID);
        }
        #endregion

        #region Ensure
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
        public Task<List<UserRoleResponse>> GetAllUserRolesAsync() => _userRoleData.GetAllUserRolesAsync();

        public async Task<UserRoleResponse> GetUserRoleAsync(int userId, int roleId)
        {
            ValidationHelper.ValidateId(userId);
            ValidationHelper.ValidateId(roleId);

            UserRoleResponse? userRole =
                await _userRoleData.GetUserRoleAsync(userId, roleId);

            if (userRole == null)
                throw new KeyNotFoundException("The user-role relationship does not exist.");

            return userRole;
        }

        public async Task<List<UserRoleResponse>> GetRolesByUserIdAsync(int userId)
        {
            ValidationHelper.ValidateId(userId);

            await EnsureHelper.EnsureExistsAsync(_userData.IsUserExistAsync, userId, "User");

            return await _userRoleData.GetRolesByUserIdAsync(userId);
        }

        public async Task<bool> AddUserRoleAsync(UserRoleRequest userRole)
        {
            ValidateUserRole(userRole);

            await EnsureHelper.EnsureExistsAsync(_userData.IsUserExistAsync, userRole.UserID, "User");
            await EnsureHelper.EnsureExistsAsync(_roleData.IsRoleExistAsync, userRole.RoleID, "Role");
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