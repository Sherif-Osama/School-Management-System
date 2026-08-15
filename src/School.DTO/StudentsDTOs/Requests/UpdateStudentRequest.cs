namespace School.DTO.StudentsDTOs.Requests
{
    public class UpdateStudentRequest
    {
        public int ClassID { get; set; }

        public DateTime EnrollmentDate { get; set; }

        public int StatusID { get; set; }
    }
}