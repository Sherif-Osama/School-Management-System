using School.DTO.ClassesDTOs;

namespace School.DAL.Interfaces
{
    public interface IClassData
    {
        Task<int> AddClassAsync(ClassDTO schoolClass);
        Task<bool> DeleteClassAsync(int classId);
        Task<List<ClassDetailsDTO>> GetAllClassesAsync();
        Task<ClassDetailsDTO?> GetClassByDetailsAsync(byte gradeId, string className, string academicYear);
        Task<ClassDetailsDTO?> GetClassByIdAsync(int classId);
        Task<bool> HasClassAvailableCapacityAsync(int classID);
        Task<bool> IsClassExistAsync(int classId);
        Task<bool> UpdateClassAsync(ClassDTO schoolClass);
    }
}