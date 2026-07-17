using School.DTO.AssociationsDTOs.StudentParentDTOs;

namespace School.DAL.Interfaces
{
    public interface IStudentParentData
    {
        Task<bool> AddStudentParentAsync(StudentParentDTO relation);
        Task<bool> DeleteStudentParentAsync(StudentParentDTO relation);
        Task<List<StudentParentDetailsDTO>> GetAllStudentParentsAsync();
        Task<List<StudentParentDetailsDTO>> GetParentsByStudentIdAsync(int studentId);
        Task<List<StudentParentDetailsDTO>> GetStudentsByParentIdAsync(int parentId);
        Task<bool> IsStudentParentExistAsync(StudentParentDTO relation);
    }
}