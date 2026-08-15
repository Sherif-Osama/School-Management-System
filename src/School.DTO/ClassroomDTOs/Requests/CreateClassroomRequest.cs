namespace School.DTO.ClassroomDTOs.Requests
{
    public class CreateClassroomRequest
    {
        public required string RoomName { get; set; }

        public int Capacity { get; set; }
    }
}