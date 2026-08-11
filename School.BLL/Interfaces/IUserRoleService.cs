using School.DTO.AssociationsDTOs.UserRoleDTOs.Requests;
using School.DTO.AssociationsDTOs.UserRoleDTOs.Responses;

namespace School.BLL.Interfaces
{
    public interface IUserRoleService
    {
        Task<List<UserRoleResponse>> GetAllUserRolesAsync();

        Task<UserRoleResponse?> GetUserRoleAsync(int userId, int roleId);

        Task<List<UserRoleResponse>> GetRolesByUserIdAsync(int userId);

        Task<bool> AddUserRoleAsync(UserRoleRequest userRole);

        Task<bool> DeleteUserRoleAsync(int userId, int roleId);
    }
}