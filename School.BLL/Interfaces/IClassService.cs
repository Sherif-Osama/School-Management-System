using School.DTO.ClassesDTOs;

namespace School.BLL.Interfaces
{
    public interface IClassService
    {
        Task<int> AddClassAsync(ClassDTO schoolClass);
        Task<bool> DeleteClassAsync(int classId);
        Task<List<ClassDetailsDTO>> GetAllClassesAsync();
        Task<ClassDetailsDTO?> GetClassByDetailsAsync(byte gradeId, string className, string academicYear);
        Task<ClassDetailsDTO?> GetClassByIdAsync(int classId);
        Task<bool> IsClassExistAsync(int classId);
        Task<bool> UpdateClassAsync(ClassDTO schoolClass);
    }
}