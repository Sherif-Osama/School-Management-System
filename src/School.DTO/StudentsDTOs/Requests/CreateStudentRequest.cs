namespace School.DTO.StudentsDTOs.Requests
{
    public class CreateStudentRequest
    {
        public int PersonID { get; set; }

        public int ClassID { get; set; }

        public DateTime EnrollmentDate { get; set; }

        public int StatusID { get; set; }
    }
}