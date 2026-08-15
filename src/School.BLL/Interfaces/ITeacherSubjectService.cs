using School.DTO.AssociationsDTOs.TeacherSubjectDTOs.Requests;
using School.DTO.AssociationsDTOs.TeacherSubjectDTOs.Responses;

namespace School.BLL.Interfaces
{
    public interface ITeacherSubjectService
    {
        Task<bool> AssignSubjectToTeacherAsync(TeacherSubjectRequest relation);
        Task<List<TeacherSubjectResponse>> GetAllTeacherSubjectsAsync();
        Task<List<TeacherSubjectResponse>> GetSubjectsByTeacherIdAsync(int teacherId);
        Task<List<TeacherSubjectResponse>> GetTeachersBySubjectIdAsync(int subjectId);
        Task<bool> RemoveSubjectFromTeacherAsync(TeacherSubjectRequest relation);
    }
}