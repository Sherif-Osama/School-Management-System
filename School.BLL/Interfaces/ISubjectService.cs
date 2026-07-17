using School.DTO.SubjectDTO;

namespace School.BLL.Interfaces
{
    public interface ISubjectService
    {
        Task<int> AddSubjectAsync(SubjectDTO subject);
        Task<bool> DeleteSubjectAsync(int subjectId);
        Task<List<SubjectDTO>> GetAllSubjectsAsync();
        Task<SubjectDTO?> GetSubjectByIdAsync(int subjectId);
        Task<SubjectDTO?> GetSubjectByNameAsync(string subjectName);
        Task<bool> IsSubjectExistAsync(int subjectId);
        Task<bool> UpdateSubjectAsync(SubjectDTO subject);
    }
}