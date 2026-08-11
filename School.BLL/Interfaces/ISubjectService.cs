using School.DTO.SubjectDTOs.Requests;
using School.DTO.SubjectDTOs.Responses;

namespace School.BLL.Interfaces
{
    public interface ISubjectService
    {
        Task<int> AddSubjectAsync(CreateSubjectRequest subject);
        Task<bool> DeleteSubjectAsync(int subjectId);
        Task<List<SubjectResponse>> GetAllSubjectsAsync();
        Task<SubjectResponse?> GetSubjectByIdAsync(int subjectId);
        Task<SubjectResponse?> GetSubjectByNameAsync(string subjectName);
        Task<bool> IsSubjectExistAsync(int subjectId);
        Task<bool> UpdateSubjectAsync(int subjectId, UpdateSubjectRequest subject);
    }
}