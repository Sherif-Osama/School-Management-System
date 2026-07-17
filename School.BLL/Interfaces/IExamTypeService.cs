using School.DTO.ExamTypeDTOs;

namespace School.BLL.Interfaces
{
    public interface IExamTypeService
    {
        Task<int> AddExamTypeAsync(ExamTypeDTO examType);
        Task<bool> DeleteExamTypeAsync(int examTypeId);
        Task<List<ExamTypeDTO>> GetAllExamTypesAsync();
        Task<ExamTypeDTO?> GetExamTypeByIdAsync(int examTypeId);
        Task<ExamTypeDTO?> GetExamTypeByNameAsync(string examName);
        Task<bool> UpdateExamTypeAsync(ExamTypeDTO examType);
    }
}