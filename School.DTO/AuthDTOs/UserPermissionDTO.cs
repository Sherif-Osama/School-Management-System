namespace School.DTO.AuthDTOs
{
    public class UserPermissionDTO
    {
        public required int PermissionID { get; set; }

        public required string PermissionName { get; set; } = string.Empty;
    }
}
