using School.DTO.AuthDTOs;

namespace School.BLL.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request);
        Task RevokeTokenAsync(RefreshTokenRequest request);
    }
}