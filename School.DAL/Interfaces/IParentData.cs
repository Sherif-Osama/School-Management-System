using School.DTO.ParentsDTOs.Requests;
using School.DTO.ParentsDTOs.Responses;

namespace School.DAL.Interfaces
{
    public interface IParentData
    {
        Task<int> AddParentAsync(CreateParentRequest parent);
        Task<bool> DeleteParentAsync(int parentId);
        Task<List<ParentResponse>> GetAllParentsAsync();
        Task<ParentResponse?> GetParentByIdAsync(int parentId);
        Task<ParentResponse?> GetParentByNationalIdAsync(string nationalId);
        Task<ParentResponse?> GetParentByPersonIdAsync(int personId);
        Task<bool> IsParentExistAsync(int parentId);
    }
}