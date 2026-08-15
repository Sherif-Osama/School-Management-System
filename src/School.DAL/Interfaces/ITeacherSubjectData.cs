using School.DTO.AssociationsDTOs.TeacherSubjectDTOs.Requests;
using School.DTO.AssociationsDTOs.TeacherSubjectDTOs.Responses;

namespace School.DAL.Interfaces
{
    public interface ITeacherSubjectData
    {
        Task<bool> AssignSubjectToTeacherAsync(TeacherSubjectRequest relation);
        Task<List<TeacherSubjectResponse>> GetAllTeacherSubjectsAsync();
        Task<List<TeacherSubjectResponse>> GetSubjectsByTeacherIdAsync(int teacherId);
        Task<List<TeacherSubjectResponse>> GetTeachersBySubjectIdAsync(int subjectId);
        Task<bool> IsTeacherSubjectExistAsync(TeacherSubjectRequest relation);
        Task<bool> RemoveSubjectFromTeacherAsync(TeacherSubjectRequest relation);
    }
}