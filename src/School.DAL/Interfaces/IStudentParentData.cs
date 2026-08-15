using School.DTO.AssociationsDTOs.StudentParentDTOs;
using School.DTO.AssociationsDTOs.StudentParentDTOs.Requests;

namespace School.DAL.Interfaces
{
    public interface IStudentParentData
    {
        Task<bool> AddStudentParentAsync(StudentParentRequest relation);
        Task<bool> DeleteStudentParentAsync(StudentParentRequest relation);
        Task<List<StudentParentResponse>> GetAllStudentParentsAsync();
        Task<List<StudentParentResponse>> GetParentsByStudentIdAsync(int studentId);
        Task<List<StudentParentResponse>> GetStudentsByParentIdAsync(int parentId);
        Task<bool> IsStudentParentExistAsync(StudentParentRequest relation);
    }
}