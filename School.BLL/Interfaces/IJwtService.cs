using School.DTO.AuthDTOs;

namespace School.BLL.Interfaces
{
    public interface IJwtService
    {
        LoginResponseDTO GenerateToken(UserAuthDTO user);
    }
}