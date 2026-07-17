using School.DTO.StudentStatusDTOs;

namespace School.DAL.Interfaces
{
    public interface IStudentStatusData
    {
        Task<int> AddStudentStatusAsync(StudentStatusDTO status);
        Task<bool> DeleteStudentStatusAsync(int statusId);
        Task<List<StudentStatusDTO>> GetAllStudentStatusesAsync();
        Task<StudentStatusDTO?> GetStudentStatusByIdAsync(int statusId);
        Task<StudentStatusDTO?> GetStudentStatusByNameAsync(string statusName);
        Task<bool> IsStudentStatusExistAsync(int statusId);
        Task<bool> UpdateStudentStatusAsync(StudentStatusDTO status);
    }
}