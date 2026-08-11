using School.DTO.TeachersDTOs.Requests;
using School.DTO.TeachersDTOs.Responses;

namespace School.DAL.Interfaces
{
    public interface ITeacherData
    {
        Task<int> AddTeacherAsync(CreateTeacherRequest teacher);
        Task<bool> DeleteTeacherAsync(int teacherId);
        Task<List<TeacherResponse>> GetAllTeachersAsync();
        Task<TeacherResponse?> GetTeacherByIdAsync(int teacherId);
        Task<TeacherResponse?> GetTeacherByNationalIdAsync(string nationalId);
        Task<TeacherResponse?> GetTeacherByPersonIdAsync(int personId);
        Task<bool> IsTeacherExistAsync(int teacherId);
        Task<bool> UpdateTeacherAsync(int teacherId, UpdateTeacherRequest teacher);
    }
}