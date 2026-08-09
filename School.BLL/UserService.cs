using School.BLL.Common;
using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.UserDTOs;

namespace School.BLL
{
    public class UserService : IUserService
    {
        private readonly IUserData _userData;
        private readonly IPersonData _personData;
        private static int MinPasswordLength => 8;
        private static int MaxPasswordLength => 500;
        private static int MinUsernameLength => 6;
        private static int MaxUsernameLength => 100;
        public UserService(IUserData userData, IPersonData personData)
        {
            _userData = userData;
            _personData = personData;
        }
        #region Validation
        private static void ValidateUser(UserDTO user)
        {
            ArgumentNullException.ThrowIfNull(user);

            ValidationHelper.ValidateId(user.PersonID);

            user.Username = ValidationHelper.ValidateString(user.Username, nameof(user.Username), MinUsernameLength, MaxUsernameLength);
            user.Password = ValidationHelper.ValidateString(user.Password, nameof(user.Password), MinPasswordLength, MaxPasswordLength);
        }

        private static void ValidateUpdatePassword(UpdatePasswordDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidationHelper.ValidateId(dto.UserID);

            dto.CurrentPassword = ValidationHelper.ValidateString(dto.CurrentPassword, nameof(dto.CurrentPassword), MinPasswordLength, MaxPasswordLength);
            dto.NewPassword = ValidationHelper.ValidateString(dto.NewPassword, nameof(dto.NewPassword), MinPasswordLength, MaxPasswordLength);
            dto.ConfirmPassword = ValidationHelper.ValidateString(dto.ConfirmPassword, nameof(dto.ConfirmPassword), MinPasswordLength, MaxPasswordLength);

            if (dto.CurrentPassword == dto.NewPassword)
                throw new ArgumentException("New password must be different from current password.", nameof(dto.NewPassword));

            if (dto.NewPassword != dto.ConfirmPassword)
                throw new ArgumentException("Password confirmation does not match.", nameof(dto.ConfirmPassword));
        }
        #endregion

        #region Ensure
        private async Task EnsureUserExistsAsync(int userId)
        {
            if (!await _userData.IsUserExistAsync(userId))
                throw new KeyNotFoundException($"User with ID {userId} does not exist.");
        }

        private async Task EnsurePersonExistsAsync(int personId)
        {
            if (!await _personData.IsPersonExistAsync(personId))
                throw new KeyNotFoundException($"Person with ID {personId} does not exist.");
        }

        private async Task EnsurePersonHasNoUserAsync(int personId, int? currentUserId = null)
        {
            UserDetailsDTO? user = await _userData.GetUserByPersonIdAsync(personId);

            if (user == null)
                return;

            if (currentUserId.HasValue && user.UserID == currentUserId)
                return;

            throw new InvalidOperationException("This person already has a user account.");
        }

        private async Task EnsureUsernameUniqueAsync(string username, int? currentUserId = null)
        {
            UserDetailsDTO? user = await _userData.GetUserByUsernameAsync(username);

            if (user == null)
                return;

            if (currentUserId.HasValue && user.UserID == currentUserId)
                return;

            throw new InvalidOperationException("Username already exists.");
        }
        #endregion

        #region Public

        public Task<List<UserDetailsDTO>> GetAllUsersAsync()
        {
            return _userData.GetAllUsersAsync();
        }

        public async Task<UserDetailsDTO?> GetUserByIdAsync(int userId)
        {
            ValidationHelper.ValidateId(userId);

            UserDetailsDTO? user = await _userData.GetUserByIdAsync(userId);

            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} does not exist.");

            return user;
        }

        public async Task<UserDetailsDTO?> GetUserByUsernameAsync(string username)
        {
            username = ValidationHelper.ValidateString(username, nameof(username), MinUsernameLength, MaxUsernameLength);

            UserDetailsDTO? user = await _userData.GetUserByUsernameAsync(username);

            if (user == null)
                throw new KeyNotFoundException($"User with username '{username}' does not exist.");

            return user;
        }

        public async Task<UserDetailsDTO?> GetUserByPersonIdAsync(int personId)
        {
            ValidationHelper.ValidateId(personId);

            await EnsurePersonExistsAsync(personId);

            UserDetailsDTO? user = await _userData.GetUserByPersonIdAsync(personId);

            if (user == null)
                throw new KeyNotFoundException($"User for person ID {personId} does not exist.");

            return user;
        }

        public async Task<int> AddUserAsync(UserDTO user)
        {
            ValidateUser(user);

            await EnsurePersonExistsAsync(user.PersonID);
            await EnsurePersonHasNoUserAsync(user.PersonID);
            await EnsureUsernameUniqueAsync(user.Username);

            user.Password = PasswordHasher.Hash(user.Password);

            int newUserId = await _userData.AddUserAsync(user);

            if (newUserId <= 0)
                throw new InvalidOperationException("Failed to add user.");

            return newUserId;
        }

        public async Task<bool> UpdateUserAsync(UpdateUserDTO user)
        {
            ArgumentNullException.ThrowIfNull(user);

            ValidationHelper.ValidateId(user.UserID);
            ValidationHelper.ValidateId(user.PersonID);

            user.Username = ValidationHelper.ValidateString(user.Username, nameof(user.Username), MinUsernameLength, MaxUsernameLength);

            await EnsureUserExistsAsync(user.UserID);
            await EnsurePersonExistsAsync(user.PersonID);
            await EnsurePersonHasNoUserAsync(user.PersonID, user.UserID);
            await EnsureUsernameUniqueAsync(user.Username, user.UserID);

            bool isUpdated = await _userData.UpdateUserAsync(user);

            if (!isUpdated)
                throw new InvalidOperationException($"Failed to update user with ID {user.UserID}.");

            return isUpdated;
        }

        public async Task<bool> ChangePasswordAsync(UpdatePasswordDTO dto)
        {
            ValidateUpdatePassword(dto);

            await EnsureUserExistsAsync(dto.UserID);

            string? hash = await _userData.GetPasswordHashByUserIdAsync(dto.UserID);

            if (hash is null)
                throw new InvalidOperationException("Password hash was not found.");

            if (!PasswordHasher.Verify(dto.CurrentPassword, hash))
                throw new UnauthorizedAccessException("Current password is incorrect.");

            string newHash = PasswordHasher.Hash(dto.NewPassword);

            bool isUpdated = await _userData.UpdatePasswordAsync(dto.UserID, newHash);

            if (!isUpdated)
                throw new InvalidOperationException("Failed to change the password.");

            return isUpdated;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            ValidationHelper.ValidateId(userId);

            await EnsureUserExistsAsync(userId);

            bool isDeleted = await _userData.DeleteUserAsync(userId);

            if (!isDeleted)
                throw new InvalidOperationException($"Failed to delete user with ID {userId}.");

            return isDeleted;
        }

        #endregion
    }
}