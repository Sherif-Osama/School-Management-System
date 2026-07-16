namespace School.DTO.UserDTOs
{
    public class UserDTO
    {
        public int UserID { get; set; }
        public int PersonID { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public bool IsActive { get; set; }
    }
}