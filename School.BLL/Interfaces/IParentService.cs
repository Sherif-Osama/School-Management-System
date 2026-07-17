using School.DTO.ParentsDTOs;

namespace School.BLL.Interfaces
{
    public interface IParentService
    {
        Task<int> AddParentAsync(ParentDTO parent);
        Task<bool> DeleteParentAsync(int parentId);
        Task<List<ParentDetailsDTO>> GetAllParentsAsync();
        Task<ParentDetailsDTO?> GetParentByIdAsync(int parentId);
        Task<ParentDetailsDTO?> GetParentByNationalIdAsync(string nationalId);
        Task<ParentDetailsDTO?> GetParentByPersonIdAsync(int personId);
        Task<bool> IsParentExistAsync(int parentId);
        Task<bool> UpdateParentAsync(ParentDTO parent);
    }
}