namespace School.DTO.AuthDTOs
{
    public class LoginRequestDTO
    {
        public required string Username { get; set; }

        public required string Password { get; set; }
    }
}