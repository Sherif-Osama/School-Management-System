using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DAL.Interfaces;
using School.DTO.AssociationsDTOs.TeacherSubjectDTOs;
using System.Data;
namespace School.DAL
{
    public class TeacherSubjectData : BaseData, ITeacherSubjectData
    {
        public TeacherSubjectData(IConfiguration configuration) : base(configuration) { }

        #region Helper Methods
        private static TeacherSubjectDetailsDTO MapTeacherSubjectDetails(SqlDataReader reader)
        {
            return new TeacherSubjectDetailsDTO
            {
                TeacherID = reader.GetInt32(reader.GetOrdinal("TeacherID")),
                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                SecondName = reader.GetString(reader.GetOrdinal("SecondName")),
                ThirdName = reader.GetString(reader.GetOrdinal("ThirdName")),
                LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? null : reader.GetString(reader.GetOrdinal("LastName")),
                SubjectID = reader.GetInt32(reader.GetOrdinal("SubjectID")),
                SubjectName = reader.GetString(reader.GetOrdinal("SubjectName"))
            };
        }

        private static void AddParameters(SqlCommand command, TeacherSubjectDTO teacherSubject)
        {
            command.Parameters.Add("@TeacherID", SqlDbType.Int).Value = teacherSubject.TeacherID;

            command.Parameters.Add("@SubjectID", SqlDbType.Int).Value = teacherSubject.SubjectID;
        }

        private static async Task<List<TeacherSubjectDetailsDTO>> ReadTeacherSubjectsAsync(SqlCommand command)
        {
            List<TeacherSubjectDetailsDTO> teacherSubjects = [];

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                teacherSubjects.Add(MapTeacherSubjectDetails(reader));
            }

            return teacherSubjects;
        }

        #endregion

        #region Public Methods

        public async Task<List<TeacherSubjectDetailsDTO>> GetAllTeacherSubjectsAsync()
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetAllTeacherSubjects");

            return await ReadTeacherSubjectsAsync(command);
        }

        public async Task<List<TeacherSubjectDetailsDTO>> GetSubjectsByTeacherIdAsync(int teacherId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetSubjectsByTeacherID");

            command.Parameters.Add("@TeacherID", SqlDbType.Int).Value = teacherId;

            return await ReadTeacherSubjectsAsync(command);
        }

        public async Task<List<TeacherSubjectDetailsDTO>> GetTeachersBySubjectIdAsync(int subjectId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetTeachersBySubjectID");

            command.Parameters.Add("@SubjectID", SqlDbType.Int).Value = subjectId;

            return await ReadTeacherSubjectsAsync(command);
        }

        public async Task<bool> AssignSubjectToTeacherAsync(TeacherSubjectDTO relation)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = CreateStoredProcedure(connection, "SP_AssignSubjectToTeacher");

            AddParameters(command, relation);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> RemoveSubjectFromTeacherAsync(TeacherSubjectDTO relation)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = CreateStoredProcedure(connection, "SP_RemoveSubjectFromTeacher");

            AddParameters(command, relation);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> IsTeacherSubjectExistAsync(TeacherSubjectDTO relation)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = CreateStoredProcedure(connection, "SP_IsTeacherSubjectExists");

            AddParameters(command, relation);

            return Convert.ToBoolean(await command.ExecuteScalarAsync());
        }

        #endregion
    }
}