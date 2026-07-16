namespace School.DTO.UserDTOs
{
    public class UpdateUserDTO
    {
        public int UserID { get; set; }

        public int PersonID { get; set; }

        public required string Username { get; set; }

        public bool IsActive { get; set; }
    }
}
