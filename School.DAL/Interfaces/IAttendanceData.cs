using School.DTO.AttendanceDTOs;

namespace School.DAL.Interfaces
{
    public interface IAttendanceData
    {
        Task<int> AddAttendanceAsync(AttendanceDTO attendance);
        Task<bool> DeleteAttendanceAsync(int attendanceId);
        Task<List<AttendanceDetailsDTO>> GetAllAttendancesAsync();
        Task<AttendanceDetailsDTO?> GetAttendanceByIdAsync(int attendanceId);
        Task<List<AttendanceDetailsDTO>> GetAttendancesByClassIdAsync(int classId);
        Task<List<AttendanceDetailsDTO>> GetAttendancesByDateAsync(DateOnly attendanceDate);
        Task<List<AttendanceDetailsDTO>> GetAttendancesByStatusIdAsync(int statusId);
        Task<List<AttendanceDetailsDTO>> GetAttendancesByStudentIdAsync(int studentId);
        Task<bool> IsAttendanceExistAsync(int attendanceId);
        Task<bool> UpdateAttendanceAsync(AttendanceDTO attendance);
        Task<bool> IsStudentAttendanceExistsAsync(int studentId, DateOnly attendanceDate, int? attendanceId = null);
    }
}