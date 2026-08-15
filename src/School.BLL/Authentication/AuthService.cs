using School.BLL.Common;
using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.AuthDTOs;
using School.DTO.UserDTOs.Responses;

namespace School.BLL.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly IUserData _userData;
        private readonly IJwtService _jwtService;
        private readonly IUserRoleData _userRoleData;
        private readonly IRolePermissionData _rolePermissionData;
        private readonly IRefreshTokenData _refreshTokenData;
        private readonly JwtSettings _jwtSettings;

        public AuthService(IUserData userData, IJwtService jwtService, IUserRoleData userRoleData,
            IRolePermissionData rolePermissionData, IRefreshTokenData refreshTokenData, JwtSettings jwtSettings)
        {
            _userData = userData;
            _jwtService = jwtService;
            _userRoleData = userRoleData;
            _rolePermissionData = rolePermissionData;
            _refreshTokenData = refreshTokenData;
            _jwtSettings = jwtSettings;
        }
        #region Helper Methods
        private static void ValidateLoginRequest(LoginRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (string.IsNullOrWhiteSpace(request.Username))
                throw new ArgumentException("Username is required.", nameof(request.Username));

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Password is required.", nameof(request.Password));

            request.Username = request.Username.Trim();
        }

        private static string ValidateRefreshTokenRequest(RefreshTokenRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            return ValidationHelper.ValidateString(request.RefreshToken, nameof(request.RefreshToken), 1, 500);
        }

        private async Task<UserAuth> BuildUserAuthAsync(int userId)
        {
            UserResponse? user = await _userData.GetUserByIdAsync(userId);

            if (user == null || !user.IsActive)
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");

            return new UserAuth
            {
                UserID = user.UserID,
                PersonID = user.PersonID,
                Username = user.Username,
                PasswordHash = string.Empty,
                IsActive = user.IsActive,
                Roles = await _userRoleData.GetRoleNamesByUserIdAsync(user.UserID),
                Permissions = await _rolePermissionData.GetPermissionNamesByUserIdAsync(user.UserID)
            };
        }

        private async Task<LoginResponse> IssueTokensAsync(UserAuth user)
        {
            LoginResponse response = _jwtService.GenerateToken(user);

            string refreshToken = _jwtService.GenerateRefreshToken();
            DateTime refreshTokenExpiresAt = DateTime.Now.AddDays(_jwtSettings.RefreshTokenExpireDays);

            await _refreshTokenData.AddRefreshTokenAsync(user.UserID, refreshToken, refreshTokenExpiresAt);

            response.RefreshToken = refreshToken;
            response.RefreshTokenExpiresAt = refreshTokenExpiresAt;

            return response;
        }
        #endregion

        #region public Methods
        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            ValidateLoginRequest(request);

            UserAuth? user = await _userData.GetUserForAuthenticationAsync(request.Username);

            if (user == null)
                throw new UnauthorizedAccessException("Invalid username or password.");

            if (!user.IsActive)
                throw new UnauthorizedAccessException("Invalid username or password.");

            if (!PasswordHasher.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid username or password.");

            user.Roles = await _userRoleData.GetRoleNamesByUserIdAsync(user.UserID);

            user.Permissions = await _rolePermissionData.GetPermissionNamesByUserIdAsync(user.UserID);

            return await IssueTokensAsync(user);
        }

        public async Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            string token = ValidateRefreshTokenRequest(request);

            RefreshToken? storedToken = await _refreshTokenData.GetRefreshTokenByTokenAsync(token);

            if (storedToken == null || storedToken.RevokedAt != null || storedToken.ExpiresAt <= DateTime.Now)
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");

            await _refreshTokenData.RevokeRefreshTokenAsync(token);

            UserAuth user = await BuildUserAuthAsync(storedToken.UserID);

            return await IssueTokensAsync(user);
        }

        public async Task RevokeTokenAsync(RefreshTokenRequest request)
        {
            string token = ValidateRefreshTokenRequest(request);

            await _refreshTokenData.RevokeRefreshTokenAsync(token);
        }
        #endregion
    }
}