namespace School.DTO.RoleDTOs.Requests
{
    public class CreateRoleRequest
    {
        public required string RoleName { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; }
    }
}
