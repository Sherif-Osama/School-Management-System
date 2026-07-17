using School.DTO.AttendanceDTOs;

namespace School.BLL.Interfaces
{
    public interface IAttendanceService
    {
        Task<int> AddAttendanceAsync(AttendanceDTO attendance);
        Task<bool> DeleteAttendanceAsync(int attendanceId);
        Task<List<AttendanceDetailsDTO>> GetAllAttendancesAsync();
        Task<AttendanceDetailsDTO?> GetAttendanceByIdAsync(int attendanceId);
        Task<List<AttendanceDetailsDTO>> GetAttendancesByClassIdAsync(int classId);
        Task<List<AttendanceDetailsDTO>> GetAttendancesByDateAsync(DateOnly attendanceDate);
        Task<List<AttendanceDetailsDTO>> GetAttendancesByStatusIdAsync(int statusId);
        Task<List<AttendanceDetailsDTO>> GetAttendancesByStudentIdAsync(int studentId);
        Task<bool> UpdateAttendanceAsync(AttendanceDTO attendance);
    }
}