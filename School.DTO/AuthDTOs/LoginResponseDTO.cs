namespace School.DTO.AuthDTOs
{
    public class LoginResponseDTO
    {
        public required string AccessToken { get; set; }

        public required DateTime ExpiresAt { get; set; }
    }
}
