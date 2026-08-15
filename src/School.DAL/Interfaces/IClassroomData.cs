using School.DTO.ClassroomDTOs.Requests;
using School.DTO.ClassroomDTOs.Responses;

namespace School.DAL.Interfaces
{
    public interface IClassroomData
    {
        Task<int> AddClassroomAsync(CreateClassroomRequest classroom);
        Task<bool> DeleteClassroomAsync(int classroomId);
        Task<List<ClassroomResponse>> GetAllClassroomsAsync();
        Task<ClassroomResponse?> GetClassroomByIdAsync(int classroomId);
        Task<ClassroomResponse?> GetClassroomByRoomNameAsync(string roomName);
        Task<bool> IsClassroomExistAsync(int classroomId);
        Task<bool> UpdateClassroomAsync(int classroomId, UpdateClassroomRequest classroom);
    }
}