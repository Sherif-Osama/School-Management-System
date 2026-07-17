using School.DTO.StudentsDTOs;

namespace School.BLL.Interfaces
{
    public interface IStudentService
    {
        Task<int> AddStudentAsync(StudentDTO student);
        Task<bool> DeleteStudentAsync(int studentId);
        Task<List<StudentDetailsDTO>> GetAllStudentsAsync();
        Task<StudentDetailsDTO?> GetStudentByIdAsync(int studentId);
        Task<StudentDetailsDTO?> GetStudentByPersonIdAsync(int personId);
        Task<bool> UpdateStudentAsync(StudentDTO student);
    }
}