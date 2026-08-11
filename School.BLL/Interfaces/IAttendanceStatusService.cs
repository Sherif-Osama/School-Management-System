using School.DTO.AttendanceStatusDTOs.Requests;
using School.DTO.AttendanceStatusDTOs.Responses;

namespace School.BLL.Interfaces
{
    public interface IAttendanceStatusService
    {
        Task<int> AddAttendanceStatusAsync(AttendanceStatusRequest status);
        Task<bool> DeleteAttendanceStatusAsync(int statusId);
        Task<List<AttendanceStatusResponse>> GetAllAttendanceStatusesAsync();
        Task<AttendanceStatusResponse?> GetAttendanceStatusByIdAsync(int statusId);
        Task<AttendanceStatusResponse?> GetAttendanceStatusByNameAsync(string statusName);
        Task<bool> UpdateAttendanceStatusAsync(int statusId, AttendanceStatusRequest status);
    }
}