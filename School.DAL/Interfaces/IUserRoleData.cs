using School.DTO.AssociationsDTOs.UserRoleDTOs.Requests;
using School.DTO.AssociationsDTOs.UserRoleDTOs.Responses;

namespace School.DAL.Interfaces
{
    public interface IUserRoleData
    {
        Task<List<UserRoleResponse>> GetAllUserRolesAsync();

        Task<UserRoleResponse?> GetUserRoleAsync(int userId, int roleId);

        Task<List<UserRoleResponse>> GetRolesByUserIdAsync(int userId);

        Task<bool> AddUserRoleAsync(UserRoleRequest userRole);

        Task<bool> DeleteUserRoleAsync(int userId, int roleId);

        Task<bool> IsUserRoleExistAsync(int userId, int roleId);

        Task<List<string>> GetRoleNamesByUserIdAsync(int userId);
    }
}