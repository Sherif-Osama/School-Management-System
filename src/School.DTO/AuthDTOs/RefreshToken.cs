namespace School.DTO.AuthDTOs
{
    public class RefreshToken
    {
        public int RefreshTokenID { get; set; }

        public int UserID { get; set; }

        public required string Token { get; set; }

        public DateTime ExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? RevokedAt { get; set; }
    }
}