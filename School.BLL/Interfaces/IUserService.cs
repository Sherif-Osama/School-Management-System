using School.DTO.UserDTOs.Requests;
using School.DTO.UserDTOs.Responses;

namespace School.BLL.Interfaces
{
    public interface IUserService
    {
        Task<int> AddUserAsync(CreateUserRequest user);
        Task<bool> ChangePasswordAsync(int userId, UpdatePasswordRequest dto);
        Task<bool> DeleteUserAsync(int userId);
        Task<List<UserResponse>> GetAllUsersAsync();
        Task<UserResponse> GetUserByIdAsync(int userId);
        Task<UserResponse> GetUserByPersonIdAsync(int personId);
        Task<UserResponse> GetUserByUsernameAsync(string username);
        Task<bool> UpdateUserAsync(int userId, UpdateUserRequest user);
    }
}