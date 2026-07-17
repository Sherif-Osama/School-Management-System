using School.DTO.ScheduleDTOs;

namespace School.BLL.Interfaces
{
    public interface IScheduleService
    {
        Task<int> AddScheduleAsync(ScheduleDTO schedule);
        Task<bool> DeleteScheduleAsync(int scheduleId);
        Task<List<ScheduleDetailsDTO>> GetAllSchedulesAsync();
        Task<ScheduleDetailsDTO?> GetScheduleByIdAsync(int scheduleId);
        Task<List<ScheduleDetailsDTO>> GetSchedulesByClassIdAsync(int classId);
        Task<List<ScheduleDetailsDTO>> GetSchedulesByClassroomIdAsync(int classroomId);
        Task<List<ScheduleDetailsDTO>> GetSchedulesByClassSubjectIdAsync(int classSubjectId);
        Task<List<ScheduleDetailsDTO>> GetSchedulesByTeacherIdAsync(int teacherId);
        Task<bool> UpdateScheduleAsync(ScheduleDTO schedule);
    }
}