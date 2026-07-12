namespace School.DTO.TeachersDTOs
{
    public class TeacherDTO
    {
        public int TeacherID { get; set; }

        public int PersonID { get; set; }

        public DateTime HireDate { get; set; }

        public decimal Salary { get; set; }

        public bool IsActive { get; set; }
    }
}