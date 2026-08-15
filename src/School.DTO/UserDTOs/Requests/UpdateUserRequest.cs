namespace School.DTO.UserDTOs.Requests
{
    public class UpdateUserRequest
    {
        public required string Username { get; set; }

        public bool IsActive { get; set; }
    }
}