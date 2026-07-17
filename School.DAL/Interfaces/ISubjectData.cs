using School.DTO.SubjectDTO;

namespace School.DAL.Interfaces
{
    public interface ISubjectData
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