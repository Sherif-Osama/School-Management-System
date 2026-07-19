using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DAL.Interfaces;
using School.DTO.UserDTOs;
using System.Data;

namespace School.DAL
{
    public class UserData : BaseData, IUserData
    {
        public UserData(IConfiguration configuration) : base(configuration) { }

        #region Helper Methods

        private static UserDetailsDTO MapUserDetails(SqlDataReader reader)
        {
            return new UserDetailsDTO
            {
                UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                PersonID = reader.GetInt32(reader.GetOrdinal("PersonID")),
                NationalID = reader.GetString(reader.GetOrdinal("NationalID")),
                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                SecondName = reader.GetString(reader.GetOrdinal("SecondName")),
                ThirdName = reader.GetString(reader.GetOrdinal("ThirdName")),
                LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? null : reader.GetString(reader.GetOrdinal("LastName")),
                DateOfBirth = reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                Gender = reader.GetByte(reader.GetOrdinal("Gender")),
                Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader.GetString(reader.GetOrdinal("Address")),
                Phone = reader.GetString(reader.GetOrdinal("Phone")),
                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                ImagePath = reader.IsDBNull(reader.GetOrdinal("ImagePath")) ? null : reader.GetString(reader.GetOrdinal("ImagePath")),
                CityID = reader.GetInt32(reader.GetOrdinal("CityID")),
                Username = reader.GetString(reader.GetOrdinal("Username")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            };
        }

        private static void AddParameters(SqlCommand command, UserDTO user)
        {
            command.Parameters.Add("@PersonID", SqlDbType.Int).Value = user.PersonID;
            command.Parameters.Add("@Username", SqlDbType.NVarChar).Value = user.Username.Trim();
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = user.IsActive;
        }

        #endregion

        #region Public Methods

        public Task<List<UserDetailsDTO>> GetAllUsersAsync() =>
            QueryListAsync("SP_GetAllUsers", null, MapUserDetails);

        public Task<UserDetailsDTO?> GetUserByIdAsync(int userId) =>
            QuerySingleAsync("SP_GetUserByID", cmd => cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId,
                MapUserDetails);

        public Task<UserDetailsDTO?> GetUserByPersonIdAsync(int personId) =>
            QuerySingleAsync("SP_GetUserByPersonID", cmd => cmd.Parameters.Add("@PersonID", SqlDbType.Int).Value = personId,
                MapUserDetails);

        public Task<UserDetailsDTO?> GetUserByUsernameAsync(string username) =>
            QuerySingleAsync("SP_GetUserByUsername", cmd => cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = username.Trim(),
                MapUserDetails);

        public Task<int> AddUserAsync(UserDTO user) =>
            InsertAsync<int>("SP_Add_User",
                cmd =>
                {
                    AddParameters(cmd, user);
                    cmd.Parameters.Add("@PasswordHash", SqlDbType.NVarChar).Value = user.Password;
                },
                "@UserID", SqlDbType.Int);

        public Task<bool> UpdateUserAsync(UpdateUserDTO user) =>
            ExecuteNonQueryAsync("SP_UpdateUser",
                cmd =>
                {
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = user.UserID;
                    cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = user.Username;
                    cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = user.IsActive;
                });

        public Task<string?> GetPasswordHashByUserIdAsync(int userId) =>
            ExecuteScalarStringAsync("SP_GetPasswordHashByUserID", cmd => cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId);

        public Task<bool> UpdatePasswordAsync(int userId, string passwordHash) =>
            ExecuteNonQueryAsync(
                "SP_UpdatePassword",
                cmd =>
                {
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                    cmd.Parameters.Add("@PasswordHash", SqlDbType.NVarChar).Value = passwordHash;
                });

        public Task<bool> DeleteUserAsync(int userId) =>
            ExecuteNonQueryAsync("SP_DeleteUser", cmd => cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId);

        public Task<bool> IsUserExistAsync(int userId) =>
            ExecuteExistsAsync("SP_IsUserExists", cmd => cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId);

        #endregion
    }
}