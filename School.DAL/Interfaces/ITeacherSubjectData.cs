using School.DTO.AssociationsDTOs.TeacherSubjectDTOs;

namespace School.DAL.Interfaces
{
    public interface ITeacherSubjectData
    {
        Task<bool> AssignSubjectToTeacherAsync(TeacherSubjectDTO relation);
        Task<List<TeacherSubjectDetailsDTO>> GetAllTeacherSubjectsAsync();
        Task<List<TeacherSubjectDetailsDTO>> GetSubjectsByTeacherIdAsync(int teacherId);
        Task<List<TeacherSubjectDetailsDTO>> GetTeachersBySubjectIdAsync(int subjectId);
        Task<bool> IsTeacherSubjectExistAsync(TeacherSubjectDTO relation);
        Task<bool> RemoveSubjectFromTeacherAsync(TeacherSubjectDTO relation);
    }
}