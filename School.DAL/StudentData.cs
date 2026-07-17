using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DTO.StudentsDTOs;
using System.Data;

namespace School.DAL
{
    public class StudentData : BaseData
    {
        public StudentData(IConfiguration configuration) : base(configuration) { }
        #region Helper Methods
        private static StudentDetailsDTO MapStudentDetails(SqlDataReader reader)
        {
            return new StudentDetailsDTO
            {
                StudentID = reader.GetInt32(reader.GetOrdinal("StudentID")),

                PersonID = reader.GetInt32(reader.GetOrdinal("PersonID")),

                ClassID = reader.GetInt32(reader.GetOrdinal("ClassID")),

                GradeID = reader.GetByte(reader.GetOrdinal("GradeID")),

                GradeName = reader.GetString(reader.GetOrdinal("GradeName")),

                ClassName = reader.GetString(reader.GetOrdinal("ClassName")),

                AcademicYear = reader.GetString(reader.GetOrdinal("AcademicYear")),

                EnrollmentDate = reader.GetDateTime(reader.GetOrdinal("EnrollmentDate")),

                StatusID = reader.GetInt32(reader.GetOrdinal("StatusID")),

                StatusName = reader.GetString(reader.GetOrdinal("StatusName")),

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

                CityID = reader.GetInt32(reader.GetOrdinal("CityID"))

            };
        }

        private static void AddParameters(SqlCommand command, StudentDTO student)
        {
            command.Parameters.Add("@PersonID", SqlDbType.Int).Value = student.PersonID;
            command.Parameters.Add("@ClassID", SqlDbType.Int).Value = student.ClassID;
            command.Parameters.Add("@EnrollmentDate", SqlDbType.Date).Value = student.EnrollmentDate;
            command.Parameters.Add("@StatusID", SqlDbType.Int).Value = student.StatusID;
        }

        private static async Task<List<StudentDetailsDTO>> ReadStudentDetailsAsync(SqlCommand command)
        {
            List<StudentDetailsDTO> studentsDetails = [];
            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                studentsDetails.Add(MapStudentDetails(reader));
            }

            return studentsDetails;
        }
        #endregion

        #region Public Methods
        public async Task<List<StudentDetailsDTO>> GetAllStudentsAsync()
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetAllStudents");

            return await ReadStudentDetailsAsync(command);
        }

        public async Task<StudentDetailsDTO?> GetStudentByIdAsync(int studentId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetStudentByID");

            command.Parameters.Add("@StudentID", SqlDbType.Int).Value = studentId;

            return (await ReadStudentDetailsAsync(command)).FirstOrDefault();
        }

        public async Task<StudentDetailsDTO?> GetStudentByPersonIdAsync(int personId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetStudentByPersonID");

            command.Parameters.Add("@PersonID", SqlDbType.Int).Value = personId;

            return (await ReadStudentDetailsAsync(command)).FirstOrDefault();
        }

        public async Task<int> AddStudentAsync(StudentDTO student)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = CreateStoredProcedure(connection, "SP_AddStudent");

            AddParameters(command, student);

            var outputStudentId = new SqlParameter("@StudentID", SqlDbType.Int) { Direction = ParameterDirection.Output };

            command.Parameters.Add(outputStudentId);

            await command.ExecuteNonQueryAsync();

            return (int)outputStudentId.Value;
        }

        public async Task<bool> UpdateStudentAsync(StudentDTO student)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_UpdateStudent");
            command.Parameters.Add("@StudentID", SqlDbType.Int).Value = student.StudentID;
            AddParameters(command, student);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteStudentAsync(int studentId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_DeleteStudent");
            command.Parameters.Add("@StudentID", SqlDbType.Int).Value = studentId;
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> IsStudentExistAsync(int studentId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_IsStudentExist");

            command.Parameters.Add("@StudentID", SqlDbType.Int).Value = studentId;

            return Convert.ToBoolean(await command.ExecuteScalarAsync());
        }
        #endregion
    }
}