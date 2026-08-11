using School.DTO.AuthDTOs;
using School.DTO.UserDTOs.Requests;
using School.DTO.UserDTOs.Responses;

namespace School.DAL.Interfaces
{
    public interface IUserData
    {
        Task<int> AddUserAsync(CreateUserRequest user);
        Task<bool> DeleteUserAsync(int userId);
        Task<List<UserResponse>> GetAllUsersAsync();
        Task<string?> GetPasswordHashByUserIdAsync(int userId);
        Task<UserResponse?> GetUserByIdAsync(int userId);
        Task<UserResponse?> GetUserByPersonIdAsync(int personId);
        Task<UserResponse?> GetUserByUsernameAsync(string username);
        Task<bool> IsUserExistAsync(int userId);
        Task<bool> UpdatePasswordAsync(int userId, string passwordHash);
        Task<UserAuthDTO?> GetUserForAuthenticationAsync(string username);
        Task<bool> UpdateUserAsync(int userId, UpdateUserRequest user);
    }
}