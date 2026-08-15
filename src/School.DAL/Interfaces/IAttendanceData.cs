using School.DTO.AttendanceDTOs.Requests;
using School.DTO.AttendanceDTOs.Responses;

namespace School.DAL.Interfaces
{
    public interface IAttendanceData
    {
        Task<int> AddAttendanceAsync(CreateAttendanceRequest attendance);
        Task<bool> DeleteAttendanceAsync(int attendanceId);
        Task<List<AttendanceResponse>> GetAllAttendancesAsync();
        Task<AttendanceResponse?> GetAttendanceByIdAsync(int attendanceId);
        Task<List<AttendanceResponse>> GetAttendancesByClassIdAsync(int classId);
        Task<List<AttendanceResponse>> GetAttendancesByDateAsync(DateOnly attendanceDate);
        Task<List<AttendanceResponse>> GetAttendancesByStatusIdAsync(int statusId);
        Task<List<AttendanceResponse>> GetAttendancesByStudentIdAsync(int studentId);
        Task<bool> IsAttendanceExistAsync(int attendanceId);
        Task<bool> UpdateAttendanceAsync(int studentId, int attendanceID, UpdateAttendanceRequest attendance);
        Task<bool> IsStudentAttendanceExistsAsync(int studentId, DateOnly attendanceDate, int? attendanceId = null);
    }
}