using School.DTO.ClassesDTOs.Requests;
using School.DTO.ClassesDTOs.Responses;

namespace School.BLL.Interfaces
{
    public interface IClassService
    {
        Task<int> AddClassAsync(CreateClassRequest schoolClass);
        Task<bool> DeleteClassAsync(int classId);
        Task<List<ClassResponse>> GetAllClassesAsync();
        Task<ClassResponse?> GetClassByDetailsAsync(byte gradeId, string className, string academicYear);
        Task<ClassResponse?> GetClassByIdAsync(int classId);
        Task<bool> IsClassExistAsync(int classId);
        Task<bool> UpdateClassAsync(int classId, UpdateClassRequest schoolClass);
    }
}