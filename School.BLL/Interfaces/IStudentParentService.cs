using School.DTO.AssociationsDTOs.StudentParentDTOs;

namespace School.BLL.Interfaces
{
    public interface IStudentParentService
    {
        Task<bool> AddStudentParentAsync(StudentParentDTO relation);
        Task<bool> DeleteStudentParentAsync(StudentParentDTO relation);
        Task<List<StudentParentDetailsDTO>> GetAllStudentParentsAsync();
        Task<List<StudentParentDetailsDTO>> GetParentsByStudentIdAsync(int studentId);
        Task<List<StudentParentDetailsDTO>> GetStudentsByParentIdAsync(int parentId);
    }
}