using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DAL.Interfaces;
using School.DTO.StudentsDTOs;
using System.Data;

namespace School.DAL
{
    public class StudentData : BaseData, IStudentData
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

        #endregion

        #region Public Methods

        public Task<List<StudentDetailsDTO>> GetAllStudentsAsync() =>
            QueryListAsync("SP_GetAllStudents", null, MapStudentDetails);

        public Task<StudentDetailsDTO?> GetStudentByIdAsync(int studentId) =>
            QuerySingleAsync("SP_GetStudentByID", cmd => cmd.Parameters.Add("@StudentID", SqlDbType.Int).Value = studentId,
                MapStudentDetails);

        public Task<StudentDetailsDTO?> GetStudentByPersonIdAsync(int personId) =>
            QuerySingleAsync("SP_GetStudentByPersonID", cmd => cmd.Parameters.Add("@PersonID", SqlDbType.Int).Value = personId,
                MapStudentDetails);

        public Task<int> AddStudentAsync(StudentDTO student) =>
            InsertAsync<int>("SP_AddStudent", cmd => AddParameters(cmd, student), "@StudentID", SqlDbType.Int);

        public Task<bool> UpdateStudentAsync(StudentDTO student) =>
            ExecuteNonQueryAsync("SP_UpdateStudent",
                cmd =>
                {
                    cmd.Parameters.Add("@StudentID", SqlDbType.Int).Value = student.StudentID;
                    AddParameters(cmd, student);
                });

        public Task<bool> DeleteStudentAsync(int studentId) =>
            ExecuteNonQueryAsync("SP_DeleteStudent", cmd => cmd.Parameters.Add("@StudentID", SqlDbType.Int).Value = studentId);

        public Task<bool> IsStudentExistAsync(int studentId) =>
            ExecuteExistsAsync("SP_IsStudentExist", cmd => cmd.Parameters.Add("@StudentID", SqlDbType.Int).Value = studentId);

        #endregion
    }
}