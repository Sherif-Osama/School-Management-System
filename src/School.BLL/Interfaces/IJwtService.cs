using School.DTO.AuthDTOs;

namespace School.BLL.Interfaces
{
    public interface IJwtService
    {
        LoginResponse GenerateToken(UserAuth user);
        string GenerateRefreshToken();
    }
}