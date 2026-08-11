using School.DTO.AssociationsDTOs.ClassSubjectDTOs.Requests;
using School.DTO.AssociationsDTOs.ClassSubjectDTOs.Responses;

namespace School.DAL.Interfaces
{
    public interface IClassSubjectData
    {
        Task<int> AddClassSubjectAsync(CreateClassSubjectRequest classSubject);
        Task<bool> DeleteClassSubjectAsync(int classSubjectId);
        Task<List<ClassSubjectResponse>> GetAllClassSubjectsAsync();
        Task<ClassSubjectResponse?> GetClassSubjectByDetailsAsync(int classId, int teacherId, int subjectId);
        Task<ClassSubjectResponse?> GetClassSubjectByIdAsync(int classSubjectId);
        Task<List<ClassSubjectResponse>> GetClassSubjectsByClassIdAsync(int classId);
        Task<List<ClassSubjectResponse>> GetClassSubjectsBySubjectIdAsync(int subjectId);
        Task<List<ClassSubjectResponse>> GetClassSubjectsByTeacherIdAsync(int teacherId);
        Task<bool> IsClassSubjectExistAsync(int classSubjectId);
        Task<bool> UpdateClassSubjectAsync(int classsubjectId, UpdateClassSubjectRequest classSubject);
    }
}