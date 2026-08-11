namespace School.DTO.SubjectDTOs.Requests
{
    public class UpdateSubjectRequest
    {
        public required string SubjectName { get; set; }
        public bool IsActive { get; set; }
    }
}