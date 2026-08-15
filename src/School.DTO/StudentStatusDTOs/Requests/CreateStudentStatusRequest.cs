namespace School.DTO.StudentStatusDTOs.Requests
{
    public class CreateStudentStatusRequest
    {
        public required string StatusName { get; set; }

        public bool IsActive { get; set; }
    }
}
