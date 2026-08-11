using School.DTO.StudentStatusDTOs.Requests;
using School.DTO.StudentStatusDTOs.Responses;

namespace School.DAL.Interfaces
{
    public interface IStudentStatusData
    {
        Task<int> AddStudentStatusAsync(CreateStudentStatusRequest status);
        Task<bool> DeleteStudentStatusAsync(int statusId);
        Task<List<StudentStatusResponse>> GetAllStudentStatusesAsync();
        Task<StudentStatusResponse?> GetStudentStatusByIdAsync(int statusId);
        Task<StudentStatusResponse?> GetStudentStatusByNameAsync(string statusName);
        Task<bool> IsStudentStatusExistAsync(int statusId);
        Task<bool> UpdateStudentStatusAsync(int statusId, UpdateStudentStatusRequest status);
    }
}