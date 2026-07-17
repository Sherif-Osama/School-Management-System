using School.DTO.AttendanceStatusDTOs;

namespace School.DAL.Interfaces
{
    public interface IAttendanceStatusData
    {
        Task<int> AddAttendanceStatusAsync(AttendanceStatusDTO status);
        Task<bool> DeleteAttendanceStatusAsync(int statusId);
        Task<List<AttendanceStatusDTO>> GetAllAttendanceStatusesAsync();
        Task<AttendanceStatusDTO?> GetAttendanceStatusByIdAsync(int statusId);
        Task<AttendanceStatusDTO?> GetAttendanceStatusByNameAsync(string statusName);
        Task<bool> IsAttendanceStatusExistAsync(int statusId);
        Task<bool> UpdateAttendanceStatusAsync(AttendanceStatusDTO status);
    }
}