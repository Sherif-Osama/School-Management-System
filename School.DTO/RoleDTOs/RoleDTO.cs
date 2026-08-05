namespace School.DTO.RoleDTOs
{
    public class RoleDTO
    {
        public int RoleID { get; set; }

        public required string RoleName { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; }
    }
}