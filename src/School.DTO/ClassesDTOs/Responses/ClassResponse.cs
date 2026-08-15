namespace School.DTO.ClassesDTOs.Responses
{
    public class ClassResponse
    {
        public int ClassID { get; set; }

        public byte GradeID { get; set; }

        public required string GradeName { get; set; }

        public required string ClassName { get; set; }

        public required string AcademicYear { get; set; }

        public int Capacity { get; set; }

        public bool IsActive { get; set; }
    }
}