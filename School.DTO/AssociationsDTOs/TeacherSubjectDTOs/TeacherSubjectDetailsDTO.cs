namespace School.DTO.AssociationsDTOs.TeacherSubjectDTOs
{
    public class TeacherSubjectDetailsDTO
    {
        public int TeacherID { get; set; }

        public required string FirstName { get; set; }

        public required string SecondName { get; set; }

        public required string ThirdName { get; set; }

        public string? LastName { get; set; }

        public string FullName =>
            $"{FirstName} {SecondName} {ThirdName} {LastName}".Trim();

        public int SubjectID { get; set; }

        public required string SubjectName { get; set; }
    }
}
