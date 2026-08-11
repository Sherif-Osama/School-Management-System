namespace School.DTO.PermissionDTOs.Responses
{
    public class PermissionResponse
    {
        public int PermissionID { get; set; }

        public required string PermissionName { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; }
    }
}
