using School.DTO.AuthDTOs;

namespace School.BLL.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request);
    }
}