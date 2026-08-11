namespace School.DTO.StudentGradeDTOs.Requests
{
    public class CreateStudentGradeRequest
    {
        public int StudentID { get; set; }

        public int ExamID { get; set; }

        public decimal Grade { get; set; }

        public bool IsAbsent { get; set; }
    }
}
