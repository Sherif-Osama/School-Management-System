using School.DTO.ScheduleDTOs.Requests;
using School.DTO.ScheduleDTOs.Responses;

namespace School.DAL.Interfaces
{
    public interface IScheduleData
    {
        Task<int> AddScheduleAsync(CreateScheduleRequest schedule);
        Task<bool> DeleteScheduleAsync(int scheduleId);
        Task<List<ScheduleResponse>> GetAllSchedulesAsync();
        Task<ScheduleResponse?> GetScheduleByIdAsync(int scheduleId);
        Task<List<ScheduleResponse>> GetSchedulesByClassIdAsync(int classId);
        Task<List<ScheduleResponse>> GetSchedulesByClassroomIdAsync(int classroomId);
        Task<List<ScheduleResponse>> GetSchedulesByClassSubjectIdAsync(int classSubjectId);
        Task<List<ScheduleResponse>> GetSchedulesByTeacherIdAsync(int teacherId);
        Task<bool> IsClassAvailableAsync(int classId, byte dayOfWeek, TimeOnly startTime, TimeOnly endTime, int? scheduleId = null);
        Task<bool> IsClassroomAvailableAsync(int classroomId, byte dayOfWeek, TimeOnly startTime, TimeOnly endTime, int? scheduleId = null);
        Task<bool> IsScheduleExistAsync(int scheduleId);
        Task<bool> IsTeacherAvailableAsync(int teacherId, byte dayOfWeek, TimeOnly startTime, TimeOnly endTime, int? scheduleId = null);
        Task<bool> UpdateScheduleAsync(int scheduleId, UpdateScheduleRequest schedule);
    }
}