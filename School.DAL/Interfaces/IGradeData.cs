using School.DTO.GradesDTOs.Requests;
using School.DTO.GradesDTOs.Responses;

namespace School.DAL.Interfaces
{
    public interface IGradeData
    {
        Task<int> AddGradeAsync(CreateGradeRequest grade);
        Task<bool> DeleteGradeAsync(byte gradeId);
        Task<List<GradeResponse>> GetAllGradesAsync();
        Task<GradeResponse?> GetGradeByIdAsync(byte gradeId);
        Task<GradeResponse?> GetGradeByNameAsync(string gradeName);
        Task<bool> IsGradeExistAsync(byte gradeId);
        Task<bool> UpdateGradeAsync(byte gradeId, UpdateGradeRequest grade);
    }
}