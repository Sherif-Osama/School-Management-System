namespace School.DTO.AttendanceDTOs
{
    public class AttendanceDetailsDTO
    {
        public int AttendanceID { get; set; }

        public int StudentID { get; set; }

        public int PersonID { get; set; }

        public required string FirstName { get; set; }

        public required string SecondName { get; set; }

        public required string ThirdName { get; set; }

        public string? LastName { get; set; }

        public string FullName => $"{FirstName} {SecondName} {ThirdName} {LastName}".Trim();


        public byte GradeID { get; set; }

        public required string GradeName { get; set; }

        public int ClassID { get; set; }

        public required string ClassName { get; set; }

        public required string AcademicYear { get; set; }

        public DateOnly AttendanceDate { get; set; }

        public int StatusID { get; set; }

        public required string StatusName { get; set; }
    }
}
