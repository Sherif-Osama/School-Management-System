namespace School.DTO.ClassesDTOs
{
    public class ClassDTO
    {
        public int ClassID { get; set; }

        public byte GradeID { get; set; }

        public required string ClassName { get; set; }

        public required string AcademicYear { get; set; }

        public int Capacity { get; set; }

        public bool IsActive { get; set; }
    }
}