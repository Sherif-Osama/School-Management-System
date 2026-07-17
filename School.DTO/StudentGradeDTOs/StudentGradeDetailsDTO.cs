namespace School.DTO.StudentGradeDTOs
{
    public class StudentGradeDetailsDTO
    {
        public int StudentGradeID { get; set; }

        public int StudentID { get; set; }

        public int PersonID { get; set; }

        public required string FirstName { get; set; }

        public required string SecondName { get; set; }

        public required string ThirdName { get; set; }

        public string? LastName { get; set; }

        public string FullName => $"{FirstName} {SecondName} {ThirdName} {LastName}".Trim();

        public required byte GradeID { get; set; }

        public required string GradeName { get; set; }

        public int ClassID { get; set; }

        public required string ClassName { get; set; }

        public required string AcademicYear { get; set; }

        public int SubjectID { get; set; }

        public required string SubjectName { get; set; }

        public int ExamID { get; set; }

        public int ExamTypeID { get; set; }

        public required string ExamName { get; set; }

        public DateTime ExamDate { get; set; }

        public decimal TotalMarks { get; set; }

        public decimal Grade { get; set; }

        public bool IsAbsent { get; set; }
    }
}