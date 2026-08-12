using Microsoft.IdentityModel.Tokens;
using School.BLL.Interfaces;
using School.DTO.AuthDTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace School.BLL.Authentication
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _jwtSettings;

        public JwtService(JwtSettings jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }


        #region Helpers
        private static void AddPermissionClaims(UserAuth user, List<Claim> claims)
        {
            foreach (string permission in user.Permissions)
            {
                claims.Add(new Claim(CustomClaimTypes.Permission, permission));
            }
        }

        private static void AddRoleClaims(UserAuth user, List<Claim> claims)
        {
            foreach (string role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
        }
        #endregion

        #region Public Methods
        public LoginResponse GenerateToken(UserAuth user)
        {
            List<Claim> claims =
            [
                new(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new(ClaimTypes.Name, user.Username),
                new(CustomClaimTypes.PersonId, user.PersonID.ToString())
            ];

            AddPermissionClaims(user, claims);
            AddRoleClaims(user, claims);

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Audience = _jwtSettings.Audience,
                Issuer = _jwtSettings.Issuer,
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key))
                , SecurityAlgorithms.HmacSha256)
            };

            var securityToken = tokenHandler.CreateToken(tokenDescription);

            return new LoginResponse
            {
                AccessToken = tokenHandler.WriteToken(securityToken),
                ExpiresAt = securityToken.ValidTo,
                RefreshToken = string.Empty,
                RefreshTokenExpiresAt = DateTime.MinValue
            };
        }

        public string GenerateRefreshToken()
        {
            byte[] randomBytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(randomBytes);
        }
        #endregion
    }
}