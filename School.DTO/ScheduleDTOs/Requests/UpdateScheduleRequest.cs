namespace School.DTO.ScheduleDTOs.Requests
{
    public class UpdateScheduleRequest
    {
        public int ClassSubjectID { get; set; }

        public byte DayOfWeek { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public int ClassroomID { get; set; }
    }
}
