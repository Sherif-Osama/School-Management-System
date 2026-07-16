namespace School.DTO.ScheduleDTOs
{
    namespace School.DTO.ScheduleDTOs
    {
        public class ScheduleDTO
        {
            public int ScheduleID { get; set; }

            public int ClassSubjectID { get; set; }

            public byte DayOfWeek { get; set; }

            public TimeOnly StartTime { get; set; }

            public TimeOnly EndTime { get; set; }

            public int ClassroomID { get; set; }
        }
    }
}
