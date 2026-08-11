namespace School.DTO.AssociationsDTOs.RolePermissionDTOs
{
    public class RolePermissionResponse
    {
        public int RoleID { get; set; }

        public required string RoleName { get; set; }

        public int PermissionID { get; set; }

        public required string PermissionName { get; set; }

        public string? Description { get; set; }

        public bool IsRoleActive { get; set; }

        public bool IsPermissionActive { get; set; }
    }
}