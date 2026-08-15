namespace School.DTO.SubjectDTOs.Requests
{
    public class CreateSubjectRequest
    {
        public required string SubjectName { get; set; }
        public bool IsActive { get; set; }
    }
}