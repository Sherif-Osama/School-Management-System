using School.DTO.AuthDTOs;

namespace School.DAL.Interfaces
{
    public interface IRefreshTokenData
    {
        Task<int> AddRefreshTokenAsync(int userId, string token, DateTime expiresAt);
        Task<RefreshToken?> GetRefreshTokenByTokenAsync(string token);
        Task<bool> RevokeRefreshTokenAsync(string token);
        Task<bool> RevokeAllRefreshTokensByUserIdAsync(int userId);
    }
}
