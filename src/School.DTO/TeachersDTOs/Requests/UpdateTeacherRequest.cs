namespace School.DTO.TeachersDTOs.Requests
{
    public class UpdateTeacherRequest
    {
        public DateTime HireDate { get; set; }
        public decimal Salary { get; set; }
        public bool IsActive { get; set; }
    }
}