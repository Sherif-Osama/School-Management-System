namespace School.DTO.UserDTOs.Requests
{
    public class CreateUserRequest
    {
        public int PersonID { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public bool IsActive { get; set; }
    }
}