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

        #endregion

        #region Public Methods

        public Task<List<TeacherDetailsDTO>> GetAllTeachersAsync() =>
            QueryListAsync("SP_GetAllTeachers", null, MapTeacherDetails);

        public Task<TeacherDetailsDTO?> GetTeacherByIdAsync(int teacherId) =>
            QuerySingleAsync("SP_GetTeacherByID", cmd => cmd.Parameters.Add("@TeacherID", SqlDbType.Int).Value = teacherId,
                MapTeacherDetails);

        public Task<TeacherDetailsDTO?> GetTeacherByPersonIdAsync(int personId) =>
            QuerySingleAsync("SP_GetTeacherByPersonID", cmd => cmd.Parameters.Add("@PersonID", SqlDbType.Int).Value = personId,
                MapTeacherDetails);

        public Task<TeacherDetailsDTO?> GetTeacherByNationalIdAsync(string nationalId) =>
            QuerySingleAsync("SP_GetTeacherByNationalID", cmd => cmd.Parameters.Add("@NationalID", SqlDbType.NVarChar).Value = nationalId,
                MapTeacherDetails);

        public Task<int> AddTeacherAsync(TeacherDTO teacher) =>
            InsertAsync<int>("SP_AddTeacher", cmd => AddParameters(cmd, teacher), "@TeacherID", SqlDbType.Int);

        public Task<bool> UpdateTeacherAsync(TeacherDTO teacher) =>
            ExecuteNonQueryAsync("SP_UpdateTeacher",
                cmd =>
                {
                    cmd.Parameters.Add("@TeacherID", SqlDbType.Int).Value = teacher.TeacherID;
                    AddParameters(cmd, teacher);
                });

        public Task<bool> DeleteTeacherAsync(int teacherId) =>
            ExecuteNonQueryAsync("SP_DeleteTeacher", cmd => cmd.Parameters.Add("@TeacherID", SqlDbType.Int).Value = teacherId);

        public Task<bool> IsTeacherExistAsync(int teacherId) =>
            ExecuteExistsAsync("SP_IsTeacherExists", cmd => cmd.Parameters.Add("@TeacherID", SqlDbType.Int).Value = teacherId);

        #endregion
    }
}