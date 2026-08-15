using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DAL.Interfaces;
using School.DTO.AuthDTOs;
using System.Data;

namespace School.DAL
{
    public class RefreshTokenData : BaseData, IRefreshTokenData
    {
        public RefreshTokenData(IConfiguration configuration) : base(configuration) { }

        #region Helper Methods
        private static RefreshToken MapRefreshToken(SqlDataReader reader)
        {
            return new RefreshToken
            {
                RefreshTokenID = reader.GetInt32(reader.GetOrdinal("RefreshTokenID")),
                UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                Token = reader.GetString(reader.GetOrdinal("Token")),
                ExpiresAt = reader.GetDateTime(reader.GetOrdinal("ExpiresAt")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                RevokedAt = reader.IsDBNull(reader.GetOrdinal("RevokedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("RevokedAt"))
            };
        }
        #endregion

        #region Public Methods
        public Task<int> AddRefreshTokenAsync(int userId, string token, DateTime expiresAt) =>
            InsertAsync<int>("SP_AddRefreshToken",
                cmd =>
                {
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                    cmd.Parameters.Add("@Token", SqlDbType.NVarChar).Value = token;
                    cmd.Parameters.Add("@ExpiresAt", SqlDbType.DateTime2).Value = expiresAt;
                },
                "@RefreshTokenID", SqlDbType.Int);

        public Task<RefreshToken?> GetRefreshTokenByTokenAsync(string token) =>
            QuerySingleAsync("SP_GetRefreshTokenByToken",
                cmd => cmd.Parameters.Add("@Token", SqlDbType.NVarChar).Value = token,
                MapRefreshToken);

        public Task<bool> RevokeRefreshTokenAsync(string token) =>
            ExecuteNonQueryAsync("SP_RevokeRefreshToken",
                cmd => cmd.Parameters.Add("@Token", SqlDbType.NVarChar).Value = token);

        public Task<bool> RevokeAllRefreshTokensByUserIdAsync(int userId) =>
            ExecuteNonQueryAsync("SP_RevokeAllRefreshTokensByUserID",
                cmd => cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId);

        #endregion
    }
}