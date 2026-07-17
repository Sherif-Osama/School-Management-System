using School.DTO.ExamTypeDTOs;

namespace School.DAL.Interfaces
{
    public interface IExamTypeData
    {
        Task<int> AddExamTypeAsync(ExamTypeDTO examType);
        Task<bool> DeleteExamTypeAsync(int examTypeId);
        Task<List<ExamTypeDTO>> GetAllExamTypesAsync();
        Task<ExamTypeDTO?> GetExamTypeByIdAsync(int examTypeId);
        Task<ExamTypeDTO?> GetExamTypeByNameAsync(string examName);
        Task<bool> IsExamTypeExistAsync(int examTypeId);
        Task<bool> UpdateExamTypeAsync(ExamTypeDTO examType);
    }
}