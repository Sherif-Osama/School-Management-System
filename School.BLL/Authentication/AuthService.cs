using School.BLL.Common;
using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.AuthDTOs;

namespace School.BLL.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly IUserData _userData;
        private readonly IJwtService _jwtService;

        public AuthService(IUserData userData, IJwtService jwtService)
        {
            _userData = userData;
            _jwtService = jwtService;
        }
        #region Helper Methods
        private static void ValidateLoginRequest(LoginRequestDTO request)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (string.IsNullOrWhiteSpace(request.Username))
                throw new ArgumentException("Username is required.", nameof(request.Username));

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Password is required.", nameof(request.Password));

            request.Username = request.Username.Trim();
        }
        #endregion

        #region public Methods
        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request)
        {
            ValidateLoginRequest(request);

            UserAuthDTO? user = await _userData.GetUserForAuthenticationAsync(request.Username);

            if (user == null)
                throw new UnauthorizedAccessException("Invalid username or password.");

            if (!user.IsActive)
                throw new UnauthorizedAccessException("Invalid username or password.");

            if (!PasswordHasher.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid username or password.");

            return _jwtService.GenerateToken(user);
        }
        #endregion
    }
}