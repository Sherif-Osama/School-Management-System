namespace School.DTO.AuthDTOs
{
    public class UserAuthDTO
    {
        public required int UserID { get; set; }

        public required int PersonID { get; set; }

        public required string UserName { get; set; }

        public string Email { get; set; } = string.Empty;

        public required string PasswordHash { get; set; }

        public required bool IsActive { get; set; }
    }
}