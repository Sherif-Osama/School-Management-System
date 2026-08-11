namespace School.DTO.AttendanceDTOs.Requests
{
    public class UpdateAttendanceRequest
    {
        public DateOnly AttendanceDate { get; set; }

        public int StatusID { get; set; }
    }
}