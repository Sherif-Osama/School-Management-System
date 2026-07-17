namespace School.DTO.StudentGradeDetailsDTOs
{
    public class StudentGradeDTO
    {
        public int StudentGradeID { get; set; }

        public int StudentID { get; set; }

        public int ExamID { get; set; }

        public decimal Grade { get; set; }

        public bool IsAbsent { get; set; }
    }
}
