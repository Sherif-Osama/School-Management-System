using School.DTO.AssociationsDTOs.StudentParentDTOs;
using School.DTO.AssociationsDTOs.StudentParentDTOs.Requests;

namespace School.BLL.Interfaces
{
    public interface IStudentParentService
    {
        Task<bool> AddStudentParentAsync(StudentParentRequest relation);
        Task<bool> DeleteStudentParentAsync(StudentParentRequest relation);
        Task<List<StudentParentResponse>> GetAllStudentParentsAsync();
        Task<List<StudentParentResponse>> GetParentsByStudentIdAsync(int studentId);
        Task<List<StudentParentResponse>> GetStudentsByParentIdAsync(int parentId);
    }
}