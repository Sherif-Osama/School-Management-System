namespace School.DTO.AuthDTOs
{
    public class UserAuth
    {
        public int UserID { get; set; }

        public int PersonID { get; set; }

        public required string Username { get; set; }

        public required string PasswordHash { get; set; }

        public bool IsActive { get; set; }

        public List<string> Roles { get; set; } = [];

        public List<string> Permissions { get; set; } = [];
    }
}