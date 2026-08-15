namespace School.DTO.TeachersDTOs.Requests
{
    public class CreateTeacherRequest
    {
        public int PersonID { get; set; }
        public DateTime HireDate { get; set; }

        public decimal Salary { get; set; }

        public bool IsActive { get; set; }
    }
}