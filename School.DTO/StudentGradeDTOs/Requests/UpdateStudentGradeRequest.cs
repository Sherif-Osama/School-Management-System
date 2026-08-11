namespace School.DTO.StudentGradeDTOs.Requests
{
    public class UpdateStudentGradeRequest
    {
        public decimal Grade { get; set; }

        public bool IsAbsent { get; set; }
    }
}
