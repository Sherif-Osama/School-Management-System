namespace School.DTO.AuthDTOs
{
    public class LoginResponse
    {
        public required string AccessToken { get; set; }
        public required DateTime ExpiresAt { get; set; }
        public required string RefreshToken { get; set; }
        public required DateTime RefreshTokenExpiresAt { get; set; }
    }
}