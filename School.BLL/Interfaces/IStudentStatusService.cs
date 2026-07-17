using School.DTO.StudentStatusDTOs;

namespace School.BLL.Interfaces
{
    public interface IStudentStatusService
    {
        Task<int> AddStudentStatusAsync(StudentStatusDTO status);
        Task<bool> DeleteStudentStatusAsync(int statusId);
        Task<List<StudentStatusDTO>> GetAllStudentStatusesAsync();
        Task<StudentStatusDTO?> GetStudentStatusByIdAsync(int statusId);
        Task<StudentStatusDTO?> GetStudentStatusByNameAsync(string statusName);
        Task<bool> UpdateStudentStatusAsync(StudentStatusDTO status);
    }
}