namespace School.DTO.AssociationsDTOs.ClassSubjectDTOs
{
    public class ClassSubjectDetailsDTO
    {
        public int ClassSubjectID { get; set; }

        public int GradeID { get; set; }
        public required string GradeName { get; set; }

        public int ClassID { get; set; }
        public required string ClassName { get; set; }
        public required string AcademicYear { get; set; }

        public int SubjectID { get; set; }
        public required string SubjectName { get; set; }

        public int TeacherID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string SecondName { get; set; } = string.Empty;
        public string ThirdName { get; set; } = string.Empty;
        public string? LastName { get; set; }
        public string FullName
            => $"{FirstName} {SecondName} {ThirdName} {LastName}";
    }
}