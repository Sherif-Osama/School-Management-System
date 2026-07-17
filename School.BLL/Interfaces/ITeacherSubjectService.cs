using School.DTO.AssociationsDTOs.TeacherSubjectDTOs;

namespace School.BLL.Interfaces
{
    public interface ITeacherSubjectService
    {
        Task<bool> AssignSubjectToTeacherAsync(TeacherSubjectDTO relation);
        Task<List<TeacherSubjectDetailsDTO>> GetAllTeacherSubjectsAsync();
        Task<List<TeacherSubjectDetailsDTO>> GetSubjectsByTeacherIdAsync(int teacherId);
        Task<List<TeacherSubjectDetailsDTO>> GetTeachersBySubjectIdAsync(int subjectId);
        Task<bool> RemoveSubjectFromTeacherAsync(TeacherSubjectDTO relation);
    }
}