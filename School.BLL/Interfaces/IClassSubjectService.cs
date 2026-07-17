using School.DTO.AssociationsDTOs.ClassSubjectDTOs;

namespace School.BLL.Interfaces
{
    public interface IClassSubjectService
    {
        Task<int> AddClassSubjectAsync(ClassSubjectDTO classSubject);
        Task<bool> DeleteClassSubjectAsync(int classSubjectId);
        Task<List<ClassSubjectDetailsDTO>> GetAllClassSubjectsAsync();
        Task<ClassSubjectDetailsDTO?> GetClassSubjectByIdAsync(int classSubjectId);
        Task<List<ClassSubjectDetailsDTO>> GetClassSubjectsByClassIdAsync(int classId);
        Task<List<ClassSubjectDetailsDTO>> GetClassSubjectsBySubjectIdAsync(byte subjectId);
        Task<List<ClassSubjectDetailsDTO>> GetClassSubjectsByTeacherIdAsync(int teacherId);
        Task<bool> IsClassSubjectExistAsync(int classSubjectId);
        Task<bool> UpdateClassSubjectAsync(ClassSubjectDTO classSubject);
    }
}