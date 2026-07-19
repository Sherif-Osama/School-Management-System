using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DAL.Interfaces;
using School.DTO.StudentGradeDetailsDTOs;
using School.DTO.StudentGradeDTOs;
using System.Data;

namespace School.DAL
{
    public class StudentGradeData : BaseData, IStudentGradeData
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

        #endregion

        #region Public Methods

        public Task<List<StudentGradeDetailsDTO>> GetAllStudentGradesAsync() =>
            QueryListAsync("SP_GetAllStudentGrades", null, MapStudentGradeDetails);

        public Task<StudentGradeDetailsDTO?> GetStudentGradeByIdAsync(int studentGradeId) =>
            QuerySingleAsync("SP_GetStudentGradeById", cmd => cmd.Parameters.Add("@StudentGradeID", SqlDbType.Int).Value = studentGradeId,
                MapStudentGradeDetails);

        public Task<List<StudentGradeDetailsDTO>> GetStudentGradesByStudentIdAsync(int studentId) =>
            QueryListAsync("SP_GetStudentGradesByStudentId", cmd => cmd.Parameters.Add("@StudentID", SqlDbType.Int).Value = studentId,
                MapStudentGradeDetails);

        public Task<List<StudentGradeDetailsDTO>> GetStudentGradesByExamIdAsync(int examId) =>
            QueryListAsync("SP_GetStudentGradesByExamId", cmd => cmd.Parameters.Add("@ExamID", SqlDbType.Int).Value = examId,
                MapStudentGradeDetails);

        public Task<List<StudentGradeDetailsDTO>> GetStudentGradesByClassIdAsync(int classId) =>
            QueryListAsync("SP_GetStudentGradesByClassId", cmd => cmd.Parameters.Add("@ClassID", SqlDbType.Int).Value = classId,
                MapStudentGradeDetails);

        public Task<List<StudentGradeDetailsDTO>> GetStudentGradesBySubjectIdAsync(int subjectId) =>
            QueryListAsync("SP_GetStudentGradesBySubjectId", cmd => cmd.Parameters.Add("@SubjectID", SqlDbType.Int).Value = subjectId,
                MapStudentGradeDetails);

        public Task<int> AddStudentGradeAsync(StudentGradeDTO studentGrade) =>
            InsertAsync<int>("SP_AddStudentGrade", cmd => AddParameters(cmd, studentGrade), "@StudentGradeID", SqlDbType.Int);

        public Task<bool> UpdateStudentGradeAsync(StudentGradeDTO studentGrade) =>
            ExecuteNonQueryAsync("SP_UpdateStudentGrade",
                cmd =>
                {
                    cmd.Parameters.Add("@StudentGradeID", SqlDbType.Int).Value = studentGrade.StudentGradeID;
                    AddParameters(cmd, studentGrade);
                });

        public Task<bool> DeleteStudentGradeAsync(int studentGradeId) =>
            ExecuteNonQueryAsync("SP_DeleteStudentGrade", cmd => cmd.Parameters.Add("@StudentGradeID", SqlDbType.Int).Value = studentGradeId);

        public Task<bool> IsStudentGradeExistAsync(int studentGradeId) =>
            ExecuteExistsAsync("SP_IsStudentGradeExist", cmd => cmd.Parameters.Add("@StudentGradeID", SqlDbType.Int).Value = studentGradeId);

        #endregion
    }
}