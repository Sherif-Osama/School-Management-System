namespace School.DTO.ClassroomDTOs.Requests
{
    public class UpdateClassroomRequest
    {
        public required string RoomName { get; set; }

        public int Capacity { get; set; }
    }
}
