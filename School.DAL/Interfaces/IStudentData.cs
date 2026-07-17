using School.DTO.StudentsDTOs;

namespace School.DAL.Interfaces
{
    public interface IStudentData
    {
        Task<int> AddStudentAsync(StudentDTO student);
        Task<bool> DeleteStudentAsync(int studentId);
        Task<List<StudentDetailsDTO>> GetAllStudentsAsync();
        Task<StudentDetailsDTO?> GetStudentByIdAsync(int studentId);
        Task<StudentDetailsDTO?> GetStudentByPersonIdAsync(int personId);
        Task<bool> IsStudentExistAsync(int studentId);
        Task<bool> UpdateStudentAsync(StudentDTO student);
    }
}