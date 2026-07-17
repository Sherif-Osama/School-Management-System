using School.DTO.ScheduleDTOs;

namespace School.DAL.Interfaces
{
    public interface IScheduleData
    {
        Task<int> AddScheduleAsync(ScheduleDTO schedule);
        Task<bool> DeleteScheduleAsync(int scheduleId);
        Task<List<ScheduleDetailsDTO>> GetAllSchedulesAsync();
        Task<ScheduleDetailsDTO?> GetScheduleByIdAsync(int scheduleId);
        Task<List<ScheduleDetailsDTO>> GetSchedulesByClassIdAsync(int classId);
        Task<List<ScheduleDetailsDTO>> GetSchedulesByClassroomIdAsync(int classroomId);
        Task<List<ScheduleDetailsDTO>> GetSchedulesByClassSubjectIdAsync(int classSubjectId);
        Task<List<ScheduleDetailsDTO>> GetSchedulesByTeacherIdAsync(int teacherId);
        Task<bool> IsClassAvailableAsync(int classId, byte dayOfWeek, TimeOnly startTime, TimeOnly endTime, int? scheduleId = null);
        Task<bool> IsClassroomAvailableAsync(int classroomId, byte dayOfWeek, TimeOnly startTime, TimeOnly endTime, int? scheduleId = null);
        Task<bool> IsScheduleExistAsync(int scheduleId);
        Task<bool> IsTeacherAvailableAsync(int teacherId, byte dayOfWeek, TimeOnly startTime, TimeOnly endTime, int? scheduleId = null);
        Task<bool> UpdateScheduleAsync(ScheduleDTO schedule);
    }
}