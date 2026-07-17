namespace School.DTO.ScheduleDTOs
{
    public class ScheduleDetailsDTO
    {
        public int ScheduleID { get; set; }

        public byte GradeID { get; set; }

        public required string GradeName { get; set; }

        public int ClassID { get; set; }

        public required string ClassName { get; set; }

        public required string AcademicYear { get; set; }

        public int SubjectID { get; set; }

        public required string SubjectName { get; set; }

        public int TeacherID { get; set; }

        public required string FirstName { get; set; }

        public required string SecondName { get; set; }

        public required string ThirdName { get; set; }

        public string? LastName { get; set; }

        public string TeacherFullName =>
            $"{FirstName} {SecondName} {ThirdName} {LastName}".Trim();

        public int ClassroomID { get; set; }
        public required string RoomName { get; set; }

        public byte DayOfWeek { get; set; }

        public string DayName { get; set; } = string.Empty;

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }
    }
}