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

        public UserService(IUserData userData, IPersonData personData)
        {
            _userData = userData;
            _personData = personData;
        }

        #region Validation
        private static void ValidateUser(UserDTO user)
        {
            ArgumentNullException.ThrowIfNull(user);

            ValidatePersonId(user.PersonID);

            user.Username = ValidateUsername(user.Username);
            user.Password = ValidatePassword(user.Password);
        }

        private static void ValidateUserId(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("User ID must be greater than zero.", nameof(userId));
        }

        private static void ValidatePersonId(int personId)
        {
            if (personId <= 0)
                throw new ArgumentException("Person ID must be greater than zero.", nameof(personId));
        }

        private static string ValidateUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username is required.", nameof(username));

            username = username.Trim();

            if (username.Length < 3 || username.Length > 50)
                throw new ArgumentException("Username must be between 3 and 50 characters.", nameof(username));

            return username;
        }

        private static string ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is required.", nameof(password));

            password = password.Trim();

            if (password.Length < 8)
                throw new ArgumentException("Password must contain at least 8 characters.", nameof(password));

            return password;
        }

        private static void ValidateUpdatePassword(UpdatePasswordDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateUserId(dto.UserID);

            dto.CurrentPassword = ValidatePassword(dto.CurrentPassword);
            dto.NewPassword = ValidatePassword(dto.NewPassword);

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
            ValidateUserId(userId);

            UserDetailsDTO? user = await _userData.GetUserByIdAsync(userId);

            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} does not exist.");

            return user;
        }

        public async Task<UserDetailsDTO?> GetUserByUsernameAsync(string username)
        {
            username = ValidateUsername(username);

            UserDetailsDTO? user = await _userData.GetUserByUsernameAsync(username);

            if (user == null)
                throw new KeyNotFoundException($"User with username '{username}' does not exist.");

            return user;
        }

        public async Task<UserDetailsDTO?> GetUserByPersonIdAsync(int personId)
        {
            ValidatePersonId(personId);

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

            ValidateUserId(user.UserID);
            ValidatePersonId(user.PersonID);

            user.Username = ValidateUsername(user.Username);

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
            ValidateUserId(userId);

            await EnsureUserExistsAsync(userId);

            bool isDeleted = await _userData.DeleteUserAsync(userId);

            if (!isDeleted)
                throw new InvalidOperationException($"Failed to delete user with ID {userId}.");

            return isDeleted;
        }

        #endregion
    }
}