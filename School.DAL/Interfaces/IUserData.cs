using School.DTO.UserDTOs;

namespace School.DAL.Interfaces
{
    public interface IUserData
    {
        Task<int> AddUserAsync(UserDTO user);
        Task<bool> DeleteUserAsync(int userId);
        Task<List<UserDetailsDTO>> GetAllUsersAsync();
        Task<string?> GetPasswordHashByUserIdAsync(int userId);
        Task<UserDetailsDTO?> GetUserByIdAsync(int userId);
        Task<UserDetailsDTO?> GetUserByPersonIdAsync(int personId);
        Task<UserDetailsDTO?> GetUserByUsernameAsync(string username);
        Task<bool> IsUserExistAsync(int userId);
        Task<bool> UpdatePasswordAsync(int userId, string passwordHash);
        Task<bool> UpdateUserAsync(UpdateUserDTO user);
    }
}