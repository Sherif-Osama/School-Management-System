namespace School.DTO.PermissionDTOs.Requests
{
    public class UpdatePermissionRequest
    {
        public required string PermissionName { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; }
    }
}