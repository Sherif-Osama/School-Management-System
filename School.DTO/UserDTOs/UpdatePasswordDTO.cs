namespace School.DTO.UserDTOs
{
    namespace School.DTO.UserDTOs
    {
        public class UpdatePasswordDTO
        {
            public int UserID { get; set; }

            public required string CurrentPassword { get; set; }

            public required string NewPassword { get; set; }

            public required string ConfirmPassword { get; set; }
        }
    }
}
