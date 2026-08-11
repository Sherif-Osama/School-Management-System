namespace School.DTO.StudentStatusDTOs.Requests
{
    public class UpdateStudentStatusRequest
    {
        public required string StatusName { get; set; }

        public bool IsActive { get; set; }
    }
}
