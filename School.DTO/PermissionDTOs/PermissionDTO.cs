namespace School.DTO.PermissionDTOs
{
    public class PermissionDTO
    {
        public int PermissionID { get; set; }

        public required string PermissionName { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}