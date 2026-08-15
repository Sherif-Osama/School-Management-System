using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DAL.Interfaces;
using School.DTO.ExamDTOs;
using School.DTO.ExamDTOs.Requests;
using System.Data;

namespace School.DAL
{
    public class ExamData : BaseData, IExamData
    {
        public ExamData(IConfiguration configuration) : base(configuration) { }

        #region Helper Methods
        private static ExamResponse MapExamDetails(SqlDataReader reader)
        {
            return new ExamResponse
            {
                ExamID = reader.GetInt32(reader.GetOrdinal("ExamID")),
                GradeID = reader.GetByte(reader.GetOrdinal("GradeID")),
                GradeName = reader.GetString(reader.GetOrdinal("GradeName")),
                ClassID = reader.GetInt32(reader.GetOrdinal("ClassID")),
                ClassName = reader.GetString(reader.GetOrdinal("ClassName")),
                SubjectID = reader.GetInt32(reader.GetOrdinal("SubjectID")),
                SubjectName = reader.GetString(reader.GetOrdinal("SubjectName")),
                TeacherID = reader.GetInt32(reader.GetOrdinal("TeacherID")),
                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                SecondName = reader.GetString(reader.GetOrdinal("SecondName")),
                ThirdName = reader.GetString(reader.GetOrdinal("ThirdName")),
                LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? null : reader.GetString(reader.GetOrdinal("LastName")),
                ExamTypeID = reader.GetInt32(reader.GetOrdinal("ExamTypeID")),
                ExamTypeName = reader.GetString(reader.GetOrdinal("ExamTypeName")),
                ExamDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("ExamDate"))),
                TotalMarks = reader.GetDecimal(reader.GetOrdinal("TotalMarks"))
            };
        }

        private static void AddParameters(SqlCommand command, CreateExamRequest exam)
        {
            command.Parameters.Add("@ClassSubjectID", SqlDbType.Int).Value = exam.ClassSubjectID;
            command.Parameters.Add("@ExamTypeID", SqlDbType.Int).Value = exam.ExamTypeID;
            command.Parameters.Add("@ExamDate", SqlDbType.Date).Value = exam.ExamDate;
            command.Parameters.Add("@TotalMarks", SqlDbType.Decimal).Value = exam.TotalMarks;
        }

        #endregion

        #region Public Methods

        public Task<List<ExamResponse>> GetAllExamsAsync() =>
            QueryListAsync("SP_GetAll_Exams", null, MapExamDetails);

        public Task<ExamResponse?> GetExamByIdAsync(int examId) =>
            QuerySingleAsync("SP_GetExamByID", cmd => cmd.Parameters.Add("@ExamID", SqlDbType.Int).Value = examId,
                MapExamDetails);

        public Task<List<ExamResponse>> GetExamsByClassIdAsync(int classId) =>
            QueryListAsync("SP_GetExamsByClassID", cmd => cmd.Parameters.Add("@ClassID", SqlDbType.Int).Value = classId,
                MapExamDetails);

        public Task<List<ExamResponse>> GetExamsByTeacherIdAsync(int teacherId) =>
            QueryListAsync("SP_GetExamsByTeacherID", cmd => cmd.Parameters.Add("@TeacherID", SqlDbType.Int).Value = teacherId,
                MapExamDetails);

        public Task<List<ExamResponse>> GetExamsBySubjectIdAsync(int subjectId) =>
            QueryListAsync("SP_GetExamsBySubjectID", cmd => cmd.Parameters.Add("@SubjectID", SqlDbType.Int).Value = subjectId, MapExamDetails);

        public Task<int> AddExamAsync(CreateExamRequest exam) =>
            InsertAsync<int>("SP_AddExam", cmd => AddParameters(cmd, exam), "@ExamID", SqlDbType.Int);

        public Task<bool> UpdateExamAsync(int examId, UpdateExamRequest exam) =>
            ExecuteNonQueryAsync("SP_UpdateExam",
                cmd =>
                {
                    cmd.Parameters.Add("@ExamID", SqlDbType.Int).Value = examId;
                    cmd.Parameters.Add("@ClassSubjectID", SqlDbType.Int).Value = exam.ClassSubjectID;
                    cmd.Parameters.Add("@ExamTypeID", SqlDbType.Int).Value = exam.ExamTypeID;
                    cmd.Parameters.Add("@ExamDate", SqlDbType.Date).Value = exam.ExamDate;
                    cmd.Parameters.Add("@TotalMarks", SqlDbType.Decimal).Value = exam.TotalMarks;
                });

        public Task<bool> DeleteExamAsync(int examId) =>
            ExecuteNonQueryAsync("SP_DeleteExam", cmd => cmd.Parameters.Add("@ExamID", SqlDbType.Int).Value = examId);

        public Task<bool> IsExamExistAsync(int examId) =>
            ExecuteExistsAsync("SP_IsExamExists", cmd => cmd.Parameters.Add("@ExamID", SqlDbType.Int).Value = examId);

        public Task<bool> IsExamDuplicate(int classSubjectId, int examTypeId, int? examId = null) =>
            ExecuteExistsAsync("SP_IsExamDuplicate",
                cmd =>
                {
                    cmd.Parameters.Add("@ClassSubjectID", SqlDbType.Int).Value = classSubjectId;
                    cmd.Parameters.Add("@ExamTypeID", SqlDbType.Int).Value = examTypeId;
                    cmd.Parameters.Add("@ExamID", SqlDbType.Int).Value = (object?)examId ?? DBNull.Value;
                });

        #endregion
    }
}