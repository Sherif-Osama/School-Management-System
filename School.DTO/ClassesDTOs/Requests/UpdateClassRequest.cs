namespace School.DTO.ClassesDTOs.Requests
{
    public class UpdateClassRequest
    {
        public byte GradeID { get; set; }

        public required string ClassName { get; set; }

        public required string AcademicYear { get; set; }

        public int Capacity { get; set; }

        public bool IsActive { get; set; }
    }
}