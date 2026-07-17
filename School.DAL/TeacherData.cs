using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DAL.Interfaces;
using School.DTO.TeachersDTOs;
using System.Data;
namespace School.DAL
{
    public class TeacherData : BaseData, ITeacherData
    {
        public TeacherData(IConfiguration configuration) : base(configuration) { }

        #region Helper Methods
        private static TeacherDetailsDTO MapTeacherDetails(SqlDataReader reader)
        {
            return new TeacherDetailsDTO
            {
                TeacherID = reader.GetInt32(reader.GetOrdinal("TeacherID")),
                PersonID = reader.GetInt32(reader.GetOrdinal("PersonID")),
                NationalID = reader.GetString(reader.GetOrdinal("NationalID")),
                FullName = reader.GetString(reader.GetOrdinal("FullName")),
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
                HireDate = reader.GetDateTime(reader.GetOrdinal("HireDate")),
                Salary = reader.GetDecimal(reader.GetOrdinal("Salary")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
            };
        }

        private static void AddParameters(SqlCommand command, TeacherDTO teacher)
        {
            command.Parameters.Add("@PersonID", SqlDbType.Int).Value = teacher.PersonID;
            command.Parameters.Add("@HireDate", SqlDbType.Date).Value = teacher.HireDate;
            command.Parameters.Add("@Salary", SqlDbType.Decimal).Value = teacher.Salary;
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = teacher.IsActive;
        }

        private static async Task<List<TeacherDetailsDTO>> ReadTeacherDetailsAsync(SqlCommand command)
        {
            List<TeacherDetailsDTO> teachers = [];
            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                teachers.Add(MapTeacherDetails(reader));
            }
            return teachers;
        }
        #endregion

        #region Public Methods
        public async Task<List<TeacherDetailsDTO>> GetAllTeachersAsync()
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetAllTeachers");
            return await ReadTeacherDetailsAsync(command);
        }

        public async Task<TeacherDetailsDTO?> GetTeacherByIdAsync(int teacherId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetTeacherByID");
            command.Parameters.Add("@TeacherID", SqlDbType.Int).Value = teacherId;
            return (await ReadTeacherDetailsAsync(command)).FirstOrDefault();
        }

        public async Task<TeacherDetailsDTO?> GetTeacherByPersonIdAsync(int personId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetTeacherByPersonID");
            command.Parameters.Add("@PersonID", SqlDbType.Int).Value = personId;
            return (await ReadTeacherDetailsAsync(command)).FirstOrDefault();
        }

        public async Task<TeacherDetailsDTO?> GetTeacherByNationalIdAsync(string nationalId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetTeacherByNationalID");
            command.Parameters.Add("@NationalID", SqlDbType.NVarChar).Value = nationalId;

            return (await ReadTeacherDetailsAsync(command)).FirstOrDefault();
        }

        public async Task<int> AddTeacherAsync(TeacherDTO teacher)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = CreateStoredProcedure(connection, "SP_AddTeacher");

            AddParameters(command, teacher);

            var outpuTeacherId = new SqlParameter("@TeacherID", SqlDbType.Int) { Direction = ParameterDirection.Output };

            command.Parameters.Add(outpuTeacherId);

            await command.ExecuteNonQueryAsync();

            return (int)outpuTeacherId.Value;
        }

        public async Task<bool> UpdateTeacherAsync(TeacherDTO teacher)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_UpdateTeacher");
            command.Parameters.Add("@TeacherID", SqlDbType.Int).Value = teacher.TeacherID;
            AddParameters(command, teacher);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteTeacherAsync(int teacherId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_DeleteTeacher");

            command.Parameters.Add("@TeacherID", SqlDbType.Int).Value = teacherId;

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> IsTeacherExistAsync(int teacherId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_IsTeacherExists");

            command.Parameters.Add("@TeacherID", SqlDbType.Int).Value = teacherId;

            return Convert.ToBoolean(await command.ExecuteScalarAsync());
        }

        #endregion
    }
}
