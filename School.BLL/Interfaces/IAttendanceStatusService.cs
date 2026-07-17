using School.DTO.AttendanceStatusDTOs;

namespace School.BLL.Interfaces
{
    public interface IAttendanceStatusService
    {
        Task<int> AddAttendanceStatusAsync(AttendanceStatusDTO status);
        Task<bool> DeleteAttendanceStatusAsync(int statusId);
        Task<List<AttendanceStatusDTO>> GetAllAttendanceStatusesAsync();
        Task<AttendanceStatusDTO?> GetAttendanceStatusByIdAsync(int statusId);
        Task<AttendanceStatusDTO?> GetAttendanceStatusByNameAsync(string statusName);
        Task<bool> UpdateAttendanceStatusAsync(AttendanceStatusDTO status);
    }
}