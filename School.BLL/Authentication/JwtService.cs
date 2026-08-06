using Microsoft.IdentityModel.Tokens;
using School.BLL.Interfaces;
using School.DTO.AuthDTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

        public LoginResponseDTO GenerateToken(UserAuthDTO user)
        {
            Claim[] claims = [
                new(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new(ClaimTypes.Name, user.Username),
                new("PersonID", user.PersonID.ToString())];

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

            return new LoginResponseDTO
            {
                AccessToken = tokenHandler.WriteToken(securityToken),
                ExpiresAt = securityToken.ValidTo
            };
        }
    }
}