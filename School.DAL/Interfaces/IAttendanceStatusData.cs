using School.DTO.AttendanceStatusDTOs.Requests;
using School.DTO.AttendanceStatusDTOs.Responses;

namespace School.DAL.Interfaces
{
    public interface IAttendanceStatusData
    {
        Task<int> AddAttendanceStatusAsync(AttendanceStatusRequest status);
        Task<bool> DeleteAttendanceStatusAsync(int statusId);
        Task<List<AttendanceStatusResponse>> GetAllAttendanceStatusesAsync();
        Task<AttendanceStatusResponse?> GetAttendanceStatusByIdAsync(int statusId);
        Task<AttendanceStatusResponse?> GetAttendanceStatusByNameAsync(string statusName);
        Task<bool> IsAttendanceStatusExistAsync(int statusId);
        Task<bool> UpdateAttendanceStatusAsync(int statusId, AttendanceStatusRequest status);
    }
}