namespace School.DTO.StudentStatusDTOs
{
    public class StudentStatusDTO
    {
        public int StatusID { get; set; }

        public required string StatusName { get; set; }

        public bool IsActive { get; set; }
    }
}