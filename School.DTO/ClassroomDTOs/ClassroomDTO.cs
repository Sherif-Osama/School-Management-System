namespace School.DTO.ClassroomDTOs
{
    public class ClassroomDTO
    {
        public int ClassroomID { get; set; }
        public required string RoomName { get; set; }
        public int Capacity { get; set; }
    }
}