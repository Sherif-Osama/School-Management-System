namespace School.DTO.ExamDTOs
{
    public class ExamDetailsDTO
    {
        public int ExamID { get; set; }

        public byte GradeID { get; set; }

        public required string GradeName { get; set; }

        public int ClassID { get; set; }

        public required string ClassName { get; set; }

        public int SubjectID { get; set; }

        public required string SubjectName { get; set; }

        public int TeacherID { get; set; }

        public required string FirstName { get; set; }

        public required string SecondName { get; set; }

        public required string ThirdName { get; set; }

        public string? LastName { get; set; }

        public string TeacherFullName => $"{FirstName} {SecondName} {ThirdName} {LastName}".Trim();

        public int ExamTypeID { get; set; }

        public required string ExamTypeName { get; set; }

        public DateOnly ExamDate { get; set; }

        public decimal TotalMarks { get; set; }
    }
}