using School.DTO.AssociationsDTOs.UserRoleDTOs;

namespace School.BLL.Interfaces
{
    public interface IUserRoleService
    {
        Task<List<UserRoleDetailsDTO>> GetAllUserRolesAsync();

        Task<UserRoleDetailsDTO?> GetUserRoleAsync(int userId, int roleId);

        Task<List<UserRoleDetailsDTO>> GetRolesByUserIdAsync(int userId);

        Task<bool> AddUserRoleAsync(UserRoleDTO userRole);

        Task<bool> DeleteUserRoleAsync(int userId, int roleId);
    }
}