using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DAL.Interfaces;
using School.DTO.AssociationsDTOs.ClassSubjectDTOs;
using System.Data;
namespace School.DAL
{
    public class ClassSubjectData : BaseData, IClassSubjectData
    {
        public ClassSubjectData(IConfiguration configuration) : base(configuration) { }

        #region Helper Methods
        private static ClassSubjectDetailsDTO MapClassSubjectDetails(SqlDataReader reader)
        {
            return new ClassSubjectDetailsDTO
            {
                ClassSubjectID = reader.GetInt32(reader.GetOrdinal("ClassSubjectID")),
                GradeID = reader.GetByte(reader.GetOrdinal("GradeID")),
                GradeName = reader.GetString(reader.GetOrdinal("GradeName")),
                ClassID = reader.GetInt32(reader.GetOrdinal("ClassID")),
                ClassName = reader.GetString(reader.GetOrdinal("ClassName")),
                AcademicYear = reader.GetString(reader.GetOrdinal("AcademicYear")),
                SubjectID = reader.GetInt32(reader.GetOrdinal("SubjectID")),
                SubjectName = reader.GetString(reader.GetOrdinal("SubjectName")),
                TeacherID = reader.GetInt32(reader.GetOrdinal("TeacherID")),
                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                SecondName = reader.GetString(reader.GetOrdinal("SecondName")),
                ThirdName = reader.GetString(reader.GetOrdinal("ThirdName")),
                LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? null : reader.GetString(reader.GetOrdinal("LastName"))
            };
        }

        private static void AddParameters(SqlCommand command, ClassSubjectDTO classSubject)
        {
            command.Parameters.Add("@ClassID", SqlDbType.Int).Value = classSubject.ClassID;
            command.Parameters.Add("@TeacherID", SqlDbType.Int).Value = classSubject.TeacherID;
            command.Parameters.Add("@SubjectID", SqlDbType.TinyInt).Value = classSubject.SubjectID;
        }

        private static async Task<List<ClassSubjectDetailsDTO>> ReadClassSubjectsAsync(SqlCommand command)
        {
            List<ClassSubjectDetailsDTO> classSubjects = [];
            using SqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                classSubjects.Add(MapClassSubjectDetails(reader));
            return classSubjects;
        }

        #endregion

        #region Public Methods

        public async Task<List<ClassSubjectDetailsDTO>> GetAllClassSubjectsAsync()
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetAllClassSubjects");
            return await ReadClassSubjectsAsync(command);
        }

        public async Task<ClassSubjectDetailsDTO?> GetClassSubjectByIdAsync(int classSubjectId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetClassSubjectByID");
            command.Parameters.Add("@ClassSubjectID", SqlDbType.Int).Value = classSubjectId;
            return (await ReadClassSubjectsAsync(command)).FirstOrDefault();
        }

        public async Task<List<ClassSubjectDetailsDTO>> GetClassSubjectsByClassIdAsync(int classId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetClassSubjectsByClassID");

            command.Parameters.Add("@ClassID", SqlDbType.Int).Value = classId;

            return await ReadClassSubjectsAsync(command);
        }

        public async Task<List<ClassSubjectDetailsDTO>> GetClassSubjectsByTeacherIdAsync(int teacherId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetClassSubjectsByTeacherID");

            command.Parameters.Add("@TeacherID", SqlDbType.Int).Value = teacherId;

            return await ReadClassSubjectsAsync(command);
        }

        public async Task<List<ClassSubjectDetailsDTO>> GetClassSubjectsBySubjectIdAsync(int subjectId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetClassSubjectsBySubjectID");

            command.Parameters.Add("@SubjectID", SqlDbType.Int).Value = subjectId;

            return await ReadClassSubjectsAsync(command);
        }

        public async Task<ClassSubjectDetailsDTO?> GetClassSubjectByDetailsAsync(int classId, int teacherId, int subjectId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetClassSubjectByDetails");

            command.Parameters.Add("@ClassID", SqlDbType.Int).Value = classId;
            command.Parameters.Add("@TeacherID", SqlDbType.Int).Value = teacherId;
            command.Parameters.Add("@SubjectID", SqlDbType.TinyInt).Value = subjectId;

            return (await ReadClassSubjectsAsync(command)).FirstOrDefault();
        }

        public async Task<int> AddClassSubjectAsync(ClassSubjectDTO classSubject)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = CreateStoredProcedure(connection, "SP_AddClassSubject");

            AddParameters(command, classSubject);

            SqlParameter outputId = new("@ClassSubjectID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            command.Parameters.Add(outputId);

            await command.ExecuteNonQueryAsync();

            return (int)outputId.Value;
        }

        public async Task<bool> UpdateClassSubjectAsync(ClassSubjectDTO classSubject)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = CreateStoredProcedure(connection, "SP_UpdateClassSubject");

            command.Parameters.Add("@ClassSubjectID", SqlDbType.Int)
                .Value = classSubject.ClassSubjectID;

            AddParameters(command, classSubject);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteClassSubjectAsync(int classSubjectId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = CreateStoredProcedure(connection, "SP_DeleteClassSubject");

            command.Parameters.Add("@ClassSubjectID", SqlDbType.Int).Value = classSubjectId;

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> IsClassSubjectExistAsync(int classSubjectId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = CreateStoredProcedure(connection, "SP_IsClassSubjectExists");

            command.Parameters.Add("@ClassSubjectID", SqlDbType.Int).Value = classSubjectId;

            return Convert.ToBoolean(await command.ExecuteScalarAsync());
        }
        #endregion
    }
}