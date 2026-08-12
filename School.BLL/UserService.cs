using School.BLL.Common;
using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.UserDTOs.Requests;
using School.DTO.UserDTOs.Responses;

namespace School.BLL
{
    public class UserService : IUserService
    {
        private readonly IRefreshTokenData _refreshTokenData;
        private readonly IUserData _userData;
        private readonly IPersonData _personData;
        private static int MinPasswordLength => 8;
        private static int MaxPasswordLength => 500;
        private static int MinUsernameLength => 6;
        private static int MaxUsernameLength => 100;
        public UserService(IUserData userData, IPersonData personData, IRefreshTokenData refreshTokenData)
        {
            _userData = userData;
            _personData = personData;
            _refreshTokenData = refreshTokenData;
        }
        #region Validation
        private static void ValidateUser(CreateUserRequest user)
        {
            ArgumentNullException.ThrowIfNull(user);

            ValidationHelper.ValidateId(user.PersonID);

            user.Username = ValidationHelper.ValidateString(user.Username, nameof(user.Username), MinUsernameLength, MaxUsernameLength);
            user.Password = ValidationHelper.ValidateString(user.Password, nameof(user.Password), MinPasswordLength, MaxPasswordLength);
        }

        private static void ValidateUpdatePassword(int UserID, UpdatePasswordRequest dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidationHelper.ValidateId(UserID);

            dto.CurrentPassword = ValidationHelper.ValidateString(dto.CurrentPassword, nameof(dto.CurrentPassword), MinPasswordLength, MaxPasswordLength);
            dto.NewPassword = ValidationHelper.ValidateString(dto.NewPassword, nameof(dto.NewPassword), MinPasswordLength, MaxPasswordLength);
            dto.ConfirmPassword = ValidationHelper.ValidateString(dto.ConfirmPassword, nameof(dto.ConfirmPassword), MinPasswordLength, MaxPasswordLength);

            if (dto.CurrentPassword == dto.NewPassword)
                throw new ArgumentException("New password must be different from current password.", nameof(dto.NewPassword));

            if (dto.NewPassword != dto.ConfirmPassword)
                throw new ArgumentException("Password confirmation does not match.", nameof(dto.ConfirmPassword));
        }
        #endregion

        #region Public
        public Task<List<UserResponse>> GetAllUsersAsync()
        {
            return _userData.GetAllUsersAsync();
        }

        public async Task<UserResponse?> GetUserByIdAsync(int userId)
        {
            ValidationHelper.ValidateId(userId);

            UserResponse? user = await _userData.GetUserByIdAsync(userId);

            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} does not exist.");

            return user;
        }

        public async Task<UserResponse?> GetUserByUsernameAsync(string username)
        {
            username = ValidationHelper.ValidateString(username, nameof(username), MinUsernameLength, MaxUsernameLength);

            UserResponse? user = await _userData.GetUserByUsernameAsync(username);

            if (user == null)
                throw new KeyNotFoundException($"User with username '{username}' does not exist.");

            return user;
        }

        public async Task<UserResponse?> GetUserByPersonIdAsync(int personId)
        {
            ValidationHelper.ValidateId(personId);

            await EnsureHelper.EnsureExistsAsync(_personData.IsPersonExistAsync, personId, "Person");

            UserResponse? user = await _userData.GetUserByPersonIdAsync(personId);

            if (user == null)
                throw new KeyNotFoundException($"User for person ID {personId} does not exist.");

            return user;
        }

        public async Task<int> AddUserAsync(CreateUserRequest user)
        {
            ValidateUser(user);

            await EnsureHelper.EnsureExistsAsync(_personData.IsPersonExistAsync, user.PersonID, "Person");
            await EnsureHelper.EnsureUniqueAsync(_userData.GetUserByPersonIdAsync, user.PersonID);
            await EnsureHelper.EnsureUniqueAsync(_userData.GetUserByUsernameAsync, user.Username);

            user.Password = PasswordHasher.Hash(user.Password);

            int newUserId = await _userData.AddUserAsync(user);

            if (newUserId <= 0)
                throw new InvalidOperationException("Failed to add user.");

            return newUserId;
        }

        public async Task<bool> UpdateUserAsync(int userId, UpdateUserRequest user)
        {
            ArgumentNullException.ThrowIfNull(user);

            ValidationHelper.ValidateId(userId);

            user.Username = ValidationHelper.ValidateString(user.Username, nameof(user.Username), MinUsernameLength, MaxUsernameLength);

            await EnsureHelper.EnsureExistsAsync(_userData.IsUserExistAsync, userId, "User");
            await EnsureHelper.EnsureUniqueAsync(_userData.GetUserByUsernameAsync, user.Username, u => u.UserID, userId);

            bool isUpdated = await _userData.UpdateUserAsync(userId, user);

            if (!isUpdated)
                throw new InvalidOperationException($"Failed to update user with ID {userId}.");

            return isUpdated;
        }

        public async Task<bool> ChangePasswordAsync(int userId, UpdatePasswordRequest dto)
        {
            ValidateUpdatePassword(userId, dto);

            await EnsureHelper.EnsureExistsAsync(_userData.IsUserExistAsync, userId, "User");

            string? hash = await _userData.GetPasswordHashByUserIdAsync(userId);

            if (hash is null)
                throw new InvalidOperationException("Password hash was not found.");

            if (!PasswordHasher.Verify(dto.CurrentPassword, hash))
                throw new UnauthorizedAccessException("Current password is incorrect.");

            string newHash = PasswordHasher.Hash(dto.NewPassword);

            bool isUpdated = await _userData.UpdatePasswordAsync(userId, newHash);

            if (!isUpdated)
                throw new InvalidOperationException("Failed to change the password.");

            await _refreshTokenData.RevokeAllRefreshTokensByUserIdAsync(userId);

            return isUpdated;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            ValidationHelper.ValidateId(userId);

            await EnsureHelper.EnsureExistsAsync(_userData.IsUserExistAsync, userId, "User");

            bool isDeleted = await _userData.DeleteUserAsync(userId);

            if (!isDeleted)
                throw new InvalidOperationException($"Failed to delete user with ID {userId}.");

            return isDeleted;
        }

        #endregion
    }
}