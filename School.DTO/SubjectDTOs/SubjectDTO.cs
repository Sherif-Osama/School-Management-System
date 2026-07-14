namespace School.DTO.SubjectDTO
{
    public class SubjectDTO
    {
        public int SubjectID { get; set; }
        public required string SubjectName { get; set; }
        public bool IsActive { get; set; }
    }
}