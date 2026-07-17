using Microsoft.Data.SqlClient;
using School.DTO.ExamDTOs;
using System.Data;

namespace School.DAL
{
    public class ExamData
    {
        private readonly string _connectionString;

        public ExamData(string connectionString)
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

        private static ExamDetailsDTO MapExamDetails(SqlDataReader reader)
        {
            return new ExamDetailsDTO
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

        private static void AddParameters(SqlCommand command, ExamDTO exam)
        {
            command.Parameters.Add("@ClassSubjectID", SqlDbType.Int).Value = exam.ClassSubjectID;
            command.Parameters.Add("@ExamTypeID", SqlDbType.Int).Value = exam.ExamTypeID;
            command.Parameters.Add("@ExamDate", SqlDbType.Date).Value = exam.ExamDate;
            command.Parameters.Add("@TotalMarks", SqlDbType.Decimal).Value = exam.TotalMarks;
        }

        private static async Task<List<ExamDetailsDTO>> ReadExamsAsync(SqlCommand command)
        {
            List<ExamDetailsDTO> exams = [];
            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                exams.Add(MapExamDetails(reader));
            }

            return exams;
        }
        #endregion

        #region Public Methods
        public async Task<List<ExamDetailsDTO>> GetAllExamsAsync()
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = new("SP_GetAll_Exams", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            return await ReadExamsAsync(command);
        }

        public async Task<ExamDetailsDTO?> GetExamByIdAsync(int examId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = new("SP_GetExamByID", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@ExamID", SqlDbType.Int).Value = examId;

            return (await ReadExamsAsync(command)).FirstOrDefault();
        }

        public async Task<List<ExamDetailsDTO>> GetExamsByClassIdAsync(int classId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = new("SP_GetExamsByClassID", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@ClassID", SqlDbType.Int).Value = classId;

            return await ReadExamsAsync(command);
        }

        public async Task<List<ExamDetailsDTO>> GetExamsByTeacherIdAsync(int teacherId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = new("SP_GetExamsByTeacherID", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@TeacherID", SqlDbType.Int).Value = teacherId;

            return await ReadExamsAsync(command);
        }

        public async Task<List<ExamDetailsDTO>> GetExamsBySubjectIdAsync(int subjectId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = new("SP_GetExamsBySubjectID", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@SubjectID", SqlDbType.Int).Value = subjectId;

            return await ReadExamsAsync(command);
        }

        public async Task<int> AddExamAsync(ExamDTO exam)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_AddExam", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            AddParameters(command, exam);

            var outputExamId = new SqlParameter("@ExamID", SqlDbType.Int) { Direction = ParameterDirection.Output };

            command.Parameters.Add(outputExamId);

            await command.ExecuteNonQueryAsync();

            return (int)outputExamId.Value;
        }

        public async Task<bool> UpdateExamAsync(ExamDTO exam)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = new("SP_UpdateExam", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@ExamID", SqlDbType.Int).Value = exam.ExamID;
            AddParameters(command, exam);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteExamAsync(int examId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = new("SP_DeleteExam", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@ExamID", SqlDbType.Int).Value = examId;

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> IsExamExistAsync(int examId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = new("SP_IsExamExists", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@ExamID", SqlDbType.Int).Value = examId;

            return Convert.ToBoolean(await command.ExecuteScalarAsync());
        }
        #endregion
    }
}