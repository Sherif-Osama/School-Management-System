namespace School.DTO.AuthDTOs
{
    public class LoginResponseDTO
    {
        public string AccessToken { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }
    }
}
