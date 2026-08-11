using School.DTO.ScheduleDTOs.Requests;
using School.DTO.ScheduleDTOs.Responses;

namespace School.BLL.Interfaces
{
    public interface IScheduleService
    {
        Task<int> AddScheduleAsync(CreateScheduleRequest schedule);
        Task<bool> DeleteScheduleAsync(int scheduleId);
        Task<List<ScheduleResponse>> GetAllSchedulesAsync();
        Task<ScheduleResponse?> GetScheduleByIdAsync(int scheduleId);
        Task<List<ScheduleResponse>> GetSchedulesByClassIdAsync(int classId);
        Task<List<ScheduleResponse>> GetSchedulesByClassroomIdAsync(int classroomId);
        Task<List<ScheduleResponse>> GetSchedulesByClassSubjectIdAsync(int classSubjectId);
        Task<List<ScheduleResponse>> GetSchedulesByTeacherIdAsync(int teacherId);
        Task<bool> UpdateScheduleAsync(int scheduleId, UpdateScheduleRequest schedule);
    }
}