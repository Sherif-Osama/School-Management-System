using School.DTO.ExamTypeDTOs.Requests;
using School.DTO.ExamTypeDTOs.Responses;

namespace School.BLL.Interfaces
{
    public interface IExamTypeService
    {
        Task<int> AddExamTypeAsync(CreateExamTypeRequest examType);
        Task<bool> DeleteExamTypeAsync(int examTypeId);
        Task<List<ExamTypeResponse>> GetAllExamTypesAsync();
        Task<ExamTypeResponse?> GetExamTypeByIdAsync(int examTypeId);
        Task<ExamTypeResponse?> GetExamTypeByNameAsync(string examName);
        Task<bool> UpdateExamTypeAsync(int examTypeId, UpdateExamTypeRequest examType);
    }
}