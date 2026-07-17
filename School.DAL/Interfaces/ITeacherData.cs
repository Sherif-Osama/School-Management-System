using School.DTO.TeachersDTOs;

namespace School.DAL.Interfaces
{
    public interface ITeacherData
    {
        Task<int> AddTeacherAsync(TeacherDTO teacher);
        Task<bool> DeleteTeacherAsync(int teacherId);
        Task<List<TeacherDetailsDTO>> GetAllTeachersAsync();
        Task<TeacherDetailsDTO?> GetTeacherByIdAsync(int teacherId);
        Task<TeacherDetailsDTO?> GetTeacherByNationalIdAsync(string nationalId);
        Task<TeacherDetailsDTO?> GetTeacherByPersonIdAsync(int personId);
        Task<bool> IsTeacherExistAsync(int teacherId);
        Task<bool> UpdateTeacherAsync(TeacherDTO teacher);
    }
}