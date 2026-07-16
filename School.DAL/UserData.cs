
using Microsoft.Data.SqlClient;
using School.DTO.UserDTOs;
using System.Data;

namespace School.DAL
{
    public class UserData
    {
        private readonly string _connectionString;

        public UserData(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region Helper Methods

        private async Task<SqlConnection> GetOpenConnectionAsync()
        {
            SqlConnection connection = new(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

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

        private static async Task<List<UserDetailsDTO>> ReadUsersAsync(SqlCommand command)
        {
            List<UserDetailsDTO> users = [];

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                users.Add(MapUserDetails(reader));

            return users;
        }

        #endregion

        #region Public Methods

        public async Task<List<UserDetailsDTO>> GetAllUsersAsync()
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_GetAllUsers", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            return await ReadUsersAsync(command);
        }

        public async Task<UserDetailsDTO?> GetUserByIdAsync(int userId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_GetUserByID", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;

            return (await ReadUsersAsync(command)).FirstOrDefault();
        }

        public async Task<UserDetailsDTO?> GetUserByPersonIdAsync(int personId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_GetUserByPersonID", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@PersonID", SqlDbType.Int).Value = personId;

            return (await ReadUsersAsync(command)).FirstOrDefault();
        }

        public async Task<UserDetailsDTO?> GetUserByUsernameAsync(string username)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_GetUserByUsername", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@Username", SqlDbType.NVarChar).Value = username.Trim();

            return (await ReadUsersAsync(command)).FirstOrDefault();
        }

        public async Task<int> AddUserAsync(UserDTO user)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_Add_User", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            AddParameters(command, user);

            command.Parameters.Add("@PasswordHash", SqlDbType.NVarChar).Value = user.Password;

            SqlParameter outputUserId = new("@UserID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            command.Parameters.Add(outputUserId);

            await command.ExecuteNonQueryAsync();

            Console.WriteLine(command.Parameters.Count);

            foreach (SqlParameter p in command.Parameters)
            {
                Console.WriteLine($"{p.ParameterName}");
            }
            return (int)outputUserId.Value;
        }

        public async Task<bool> UpdateUserAsync(UpdateUserDTO user)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_UpdateUser", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@UserID", SqlDbType.Int).Value = user.UserID;
            command.Parameters.Add("@Username", SqlDbType.NVarChar).Value = user.Username;
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = user.IsActive;

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<string?> GetPasswordHashByUserIdAsync(int userId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_GetPasswordHashByUserID", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;

            object? result = await command.ExecuteScalarAsync();

            return result == null || result == DBNull.Value
                ? null
                : result.ToString();
        }

        public async Task<bool> UpdatePasswordAsync(int userId, string passwordHash)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_UpdatePassword", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
            command.Parameters.Add("@PasswordHash", SqlDbType.NVarChar).Value = passwordHash;

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_DeleteUser", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> IsUserExistAsync(int userId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_IsUserExists", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;

            return Convert.ToBoolean(await command.ExecuteScalarAsync());
        }

        #endregion
    }
}
