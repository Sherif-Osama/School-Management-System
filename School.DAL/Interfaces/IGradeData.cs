using School.DTO.GradesDTOs;

namespace School.DAL.Interfaces
{
    public interface IGradeData
    {
        Task<int> AddGradeAsync(GradeDTO grade);
        Task<bool> DeleteGradeAsync(byte gradeId);
        Task<List<GradeDTO>> GetAllGradesAsync();
        Task<GradeDTO?> GetGradeByIdAsync(byte gradeId);
        Task<GradeDTO?> GetGradeByNameAsync(string gradeName);
        Task<bool> IsGradeExistAsync(byte gradeId);
        Task<bool> UpdateGradeAsync(GradeDTO grade);
    }
}