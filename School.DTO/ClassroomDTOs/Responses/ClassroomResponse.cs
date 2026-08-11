namespace School.DTO.ClassroomDTOs.Responses
{
    public class ClassroomResponse
    {
        public int ClassroomID { get; set; }
        public required string RoomName { get; set; }
        public int Capacity { get; set; }
    }
}