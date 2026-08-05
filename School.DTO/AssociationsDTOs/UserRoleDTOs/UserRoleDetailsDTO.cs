namespace School.DTO.AssociationsDTOs.UserRoleDTOs
{
    public class UserRoleDetailsDTO
    {
        public int UserID { get; set; }

        public required string Username { get; set; }

        public bool IsUserActive { get; set; }

        public int RoleID { get; set; }

        public required string RoleName { get; set; }

        public string? RoleDescription { get; set; }

        public bool IsRoleActive { get; set; }
    }
}