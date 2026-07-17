using School.DTO.AssociationsDTOs.ClassSubjectDTOs;

namespace School.DAL.Interfaces
{
    public interface IClassSubjectData
    {
        Task<int> AddClassSubjectAsync(ClassSubjectDTO classSubject);
        Task<bool> DeleteClassSubjectAsync(int classSubjectId);
        Task<List<ClassSubjectDetailsDTO>> GetAllClassSubjectsAsync();
        Task<ClassSubjectDetailsDTO?> GetClassSubjectByDetailsAsync(int classId, int teacherId, int subjectId);
        Task<ClassSubjectDetailsDTO?> GetClassSubjectByIdAsync(int classSubjectId);
        Task<List<ClassSubjectDetailsDTO>> GetClassSubjectsByClassIdAsync(int classId);
        Task<List<ClassSubjectDetailsDTO>> GetClassSubjectsBySubjectIdAsync(byte subjectId);
        Task<List<ClassSubjectDetailsDTO>> GetClassSubjectsByTeacherIdAsync(int teacherId);
        Task<bool> IsClassSubjectExistAsync(int classSubjectId);
        Task<bool> UpdateClassSubjectAsync(ClassSubjectDTO classSubject);
    }
}