namespace School.DTO.StudentStatusDTOs.Responses
{
    public class StudentStatusResponse
    {
        public int StatusID { get; set; }

        public required string StatusName { get; set; }

        public bool IsActive { get; set; }
    }
}