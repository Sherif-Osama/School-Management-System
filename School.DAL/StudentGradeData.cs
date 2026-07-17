using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DTO.StudentGradeDetailsDTOs;
using School.DTO.StudentGradeDTOs;
using System.Data;
namespace School.DAL
{
    public class StudentGradeData : BaseData
    {
        public StudentGradeData(IConfiguration configuration) : base(configuration) { }

        #region Helper Methods
        private static StudentGradeDetailsDTO MapStudentGradeDetails(SqlDataReader reader)
        {
            return new StudentGradeDetailsDTO
            {
                StudentGradeID = reader.GetInt32(reader.GetOrdinal("StudentGradeID")),

                StudentID = reader.GetInt32(reader.GetOrdinal("StudentID")),

                PersonID = reader.GetInt32(reader.GetOrdinal("PersonID")),

                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),

                SecondName = reader.GetString(reader.GetOrdinal("SecondName")),

                ThirdName = reader.GetString(reader.GetOrdinal("ThirdName")),

                LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? null : reader.GetString(reader.GetOrdinal("LastName")),

                GradeID = reader.GetByte(reader.GetOrdinal("GradeID")),

                GradeName = reader.GetString(reader.GetOrdinal("GradeName")),

                ClassID = reader.GetInt32(reader.GetOrdinal("ClassID")),

                ClassName = reader.GetString(reader.GetOrdinal("ClassName")),

                AcademicYear = reader.GetString(reader.GetOrdinal("AcademicYear")),

                SubjectID = reader.GetInt32(reader.GetOrdinal("SubjectID")),

                SubjectName = reader.GetString(reader.GetOrdinal("SubjectName")),

                ExamID = reader.GetInt32(reader.GetOrdinal("ExamID")),

                ExamTypeID = reader.GetInt32(reader.GetOrdinal("ExamTypeID")),

                ExamName = reader.GetString(reader.GetOrdinal("ExamName")),

                ExamDate = reader.GetDateTime(reader.GetOrdinal("ExamDate")),

                TotalMarks = reader.GetDecimal(reader.GetOrdinal("TotalMarks")),

                Grade = reader.GetDecimal(reader.GetOrdinal("Grade")),

                IsAbsent = reader.GetBoolean(reader.GetOrdinal("IsAbsent"))

            };
        }

        private static void AddParameters(SqlCommand command, StudentGradeDTO studentGrade)
        {
            command.Parameters.Add("@StudentID", SqlDbType.Int).Value = studentGrade.StudentID;
            command.Parameters.Add("@ExamID", SqlDbType.Int).Value = studentGrade.ExamID;
            command.Parameters.Add("@Grade", SqlDbType.Decimal).Value = studentGrade.Grade;
            command.Parameters.Add("@IsAbsent", SqlDbType.Bit).Value = studentGrade.IsAbsent;
        }

        private static async Task<List<StudentGradeDetailsDTO>> ReadStudentGradesAsync(SqlCommand command)
        {
            List<StudentGradeDetailsDTO> studentGrades = [];
            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                studentGrades.Add(MapStudentGradeDetails(reader));
            }

            return studentGrades;
        }
        #endregion

        #region Public Methods
        public async Task<List<StudentGradeDetailsDTO>> GetAllStudentGradesAsync()
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetAllStudentGrades");

            return await ReadStudentGradesAsync(command);
        }

        public async Task<StudentGradeDetailsDTO?> GetStudentGradeByIdAsync(int studentGradeId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetStudentGradeById");
            command.Parameters.Add("@StudentGradeID", SqlDbType.Int).Value = studentGradeId;

            return (await ReadStudentGradesAsync(command)).FirstOrDefault();
        }

        public async Task<List<StudentGradeDetailsDTO>> GetStudentGradesByStudentIdAsync(int studentId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetStudentGradesByStudentId");
            command.Parameters.Add("@StudentID", SqlDbType.Int).Value = studentId;
            return await ReadStudentGradesAsync(command);
        }

        public async Task<List<StudentGradeDetailsDTO>> GetStudentGradesByExamIdAsync(int examId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetStudentGradesByExamId");
            command.Parameters.Add("@ExamID", SqlDbType.Int).Value = examId;

            return await ReadStudentGradesAsync(command);
        }

        public async Task<List<StudentGradeDetailsDTO>> GetStudentGradesByClassIdAsync(int classId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetStudentGradesByClassId");
            command.Parameters.Add("@ClassID", SqlDbType.Int).Value = classId;
            return await ReadStudentGradesAsync(command);
        }

        public async Task<List<StudentGradeDetailsDTO>> GetStudentGradesBySubjectIdAsync(int subjectId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetStudentGradesBySubjectId");
            command.Parameters.Add("@SubjectID", SqlDbType.Int).Value = subjectId;

            return await ReadStudentGradesAsync(command);
        }

        public async Task<int> AddStudentGradeAsync(StudentGradeDTO studentGrade)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_AddStudentGrade");
            AddParameters(command, studentGrade);
            var outputStudentGradeId = new SqlParameter("@StudentGradeID", SqlDbType.Int) { Direction = ParameterDirection.Output };
            command.Parameters.Add(outputStudentGradeId);
            await command.ExecuteNonQueryAsync();
            return (int)outputStudentGradeId.Value;
        }

        public async Task<bool> UpdateStudentGradeAsync(StudentGradeDTO studentGrade)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_UpdateStudentGrade");
            command.Parameters.Add("@StudentGradeID", SqlDbType.Int).Value = studentGrade.StudentGradeID;
            AddParameters(command, studentGrade);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteStudentGradeAsync(int studentGradeId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_DeleteStudentGrade");
            command.Parameters.Add("@StudentGradeID", SqlDbType.Int).Value = studentGradeId;
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> IsStudentGradeExistAsync(int studentGradeId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_IsStudentGradeExist");
            command.Parameters.Add("@StudentGradeID", SqlDbType.Int).Value = studentGradeId;
            return Convert.ToBoolean(await command.ExecuteScalarAsync());
        }
        #endregion
    }
}