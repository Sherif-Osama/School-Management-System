namespace School.DTO.AttendanceDTOs.Requests
{
    public class CreateAttendanceRequest
    {
        public int StudentID { get; set; }

        public DateOnly AttendanceDate { get; set; }

        public int StatusID { get; set; }
    }
}