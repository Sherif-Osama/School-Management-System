namespace School.DTO.SubjectDTOs.Responses
{
    public class SubjectResponse
    {
        public int SubjectID { get; set; }
        public required string SubjectName { get; set; }
        public bool IsActive { get; set; }
    }
}