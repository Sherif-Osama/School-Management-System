using School.DTO.AssociationsDTOs.UserRoleDTOs;

namespace School.DAL.Interfaces
{
    public interface IUserRoleData
    {
        Task<List<UserRoleDetailsDTO>> GetAllUserRolesAsync();

        Task<UserRoleDetailsDTO?> GetUserRoleAsync(int userId, int roleId);

        Task<List<UserRoleDetailsDTO>> GetRolesByUserIdAsync(int userId);

        Task<bool> AddUserRoleAsync(UserRoleDTO userRole);

        Task<bool> DeleteUserRoleAsync(int userId, int roleId);

        Task<bool> IsUserRoleExistAsync(int userId, int roleId);

        Task<List<string>> GetRoleNamesByUserIdAsync(int userId);
    }
}