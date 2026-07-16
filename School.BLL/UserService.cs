using School.DAL;
using School.DTO.UserDTOs;
using School.DTO.UserDTOs.School.DTO.UserDTOs;

namespace School.BLL
{
    public class UserService
    {
        private readonly UserData _userData;
        private readonly PersonData _personData;

        public UserService(UserData userData, PersonData personData)
        {
            _userData = userData;
            _personData = personData;
        }

        #region Validation

        private static void ValidateUser(UserDTO user)
        {
            ArgumentNullException.ThrowIfNull(user);

            if (user.PersonID <= 0)
                throw new ArgumentOutOfRangeException(nameof(user.PersonID), "Person ID must be greater than zero.");

            user.Username = ValidateUsername(user.Username);
            user.Password = ValidatePassword(user.Password);
        }

        private static int ValidateUserId(int userId)
        {
            if (userId <= 0)
                throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be greater than zero.");

            return userId;
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
                throw new InvalidOperationException($"User with ID {userId} does not exist.");
        }

        private async Task EnsurePersonExistsAsync(int personId)
        {
            if (!await _personData.IsPersonExistAsync(personId))
                throw new InvalidOperationException($"Person with ID {personId} does not exist.");
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
            => _userData.GetAllUsersAsync();

        public async Task<UserDetailsDTO?> GetUserByIdAsync(int userId)
        {
            ValidateUserId(userId);
            return await _userData.GetUserByIdAsync(userId);
        }

        public async Task<UserDetailsDTO?> GetUserByUsernameAsync(string username)
        {
            username = ValidateUsername(username);
            return await _userData.GetUserByUsernameAsync(username);
        }

        public async Task<UserDetailsDTO?> GetUserByPersonIdAsync(int personId)
        {

            if (personId <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(personId),
                    "Person ID must be greater than zero.");

            await EnsurePersonExistsAsync(personId);

            return await _userData.GetUserByPersonIdAsync(personId);
        }

        public async Task<int> AddUserAsync(UserDTO user)
        {
            ValidateUser(user);

            await EnsurePersonExistsAsync(user.PersonID);
            await EnsurePersonHasNoUserAsync(user.PersonID);
            await EnsureUsernameUniqueAsync(user.Username);

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            return await _userData.AddUserAsync(user);
        }

        public async Task<bool> UpdateUserAsync(UpdateUserDTO user)
        {
            ArgumentNullException.ThrowIfNull(user);

            ValidateUserId(user.UserID);

            user.Username = ValidateUsername(user.Username);

            await EnsureUserExistsAsync(user.UserID);
            await EnsurePersonExistsAsync(user.PersonID);
            await EnsurePersonHasNoUserAsync(user.PersonID, user.UserID);
            await EnsureUsernameUniqueAsync(user.Username, user.UserID);

            return await _userData.UpdateUserAsync(user);
        }

        public async Task<bool> ChangePasswordAsync(UpdatePasswordDTO dto)
        {
            ValidateUpdatePassword(dto);

            await EnsureUserExistsAsync(dto.UserID);

            string? hash = await _userData.GetPasswordHashByUserIdAsync(dto.UserID);

            if (hash is null)
                throw new InvalidOperationException("Password hash was not found.");

            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, hash))
                throw new UnauthorizedAccessException("Current password is incorrect.");

            string newHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            return await _userData.UpdatePasswordAsync(dto.UserID, newHash);
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            ValidateUserId(userId);

            await EnsureUserExistsAsync(userId);

            return await _userData.DeleteUserAsync(userId);
        }

        #endregion
    }
}
