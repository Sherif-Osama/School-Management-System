using School.DTO.ClassroomDTOs;

namespace School.BLL.Interfaces
{
    public interface IClassroomService
    {
        Task<int> AddClassroomAsync(ClassroomDTO classroom);
        Task<bool> DeleteClassroomAsync(int classroomId);
        Task<List<ClassroomDTO>> GetAllClassroomsAsync();
        Task<ClassroomDTO?> GetClassroomByIdAsync(int classroomId);
        Task<ClassroomDTO?> GetClassroomByRoomNameAsync(string roomName);
        Task<bool> IsClassroomExistAsync(int classroomId);
        Task<bool> UpdateClassroomAsync(ClassroomDTO classroom);
    }
}