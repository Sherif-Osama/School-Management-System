namespace School.DTO.AttendanceDTOs
{
    public class AttendanceDTO
    {
        public int AttendanceID { get; set; }

        public int StudentID { get; set; }

        public required DateOnly AttendanceDate { get; set; }

        public int StatusID { get; set; }
    }
}
