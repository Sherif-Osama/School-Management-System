using School.DTO.UserDTOs;

namespace School.BLL.Interfaces
{
    public interface IUserService
    {
        Task<int> AddUserAsync(UserDTO user);
        Task<bool> ChangePasswordAsync(UpdatePasswordDTO dto);
        Task<bool> DeleteUserAsync(int userId);
        Task<List<UserDetailsDTO>> GetAllUsersAsync();
        Task<UserDetailsDTO?> GetUserByIdAsync(int userId);
        Task<UserDetailsDTO?> GetUserByPersonIdAsync(int personId);
        Task<UserDetailsDTO?> GetUserByUsernameAsync(string username);
        Task<bool> UpdateUserAsync(UpdateUserDTO user);
    }
}