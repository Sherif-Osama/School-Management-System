using School.DTO.StudentsDTOs.Requests;
using School.DTO.StudentsDTOs.Responses;

namespace School.DAL.Interfaces
{
    public interface IStudentData
    {
        Task<int> AddStudentAsync(CreateStudentRequest student);
        Task<bool> DeleteStudentAsync(int studentId);
        Task<List<StudentResponse>> GetAllStudentsAsync();
        Task<StudentResponse?> GetStudentByIdAsync(int studentId);
        Task<StudentResponse?> GetStudentByPersonIdAsync(int personId);
        Task<bool> IsStudentExistAsync(int studentId);
        Task<bool> UpdateStudentAsync(int studentId, UpdateStudentRequest student);
    }
}