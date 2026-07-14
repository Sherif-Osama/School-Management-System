using Microsoft.Data.SqlClient;
using School.DTO.ClassesDTOs;
using System.Data;

namespace School.DAL
{
    public class ClassData
    {
        private readonly string _connectionString;

        public ClassData(string connectionString)
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

        private static ClassDetailsDTO MapClassDetails(SqlDataReader reader)
        {
            return new ClassDetailsDTO
            {
                ClassID = reader.GetInt32(reader.GetOrdinal("ClassID")),
                GradeID = reader.GetByte(reader.GetOrdinal("GradeID")),
                GradeName = reader.GetString(reader.GetOrdinal("GradeName")),
                ClassName = reader.GetString(reader.GetOrdinal("ClassName")),
                AcademicYear = reader.GetString(reader.GetOrdinal("AcademicYear")),
                Capacity = reader.GetInt32(reader.GetOrdinal("Capacity")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            };
        }

        private static void AddParameters(SqlCommand command, ClassDTO schoolClass)
        {
            command.Parameters.Add("@GradeID", SqlDbType.TinyInt).Value = schoolClass.GradeID;
            command.Parameters.Add("@ClassName", SqlDbType.NVarChar).Value = schoolClass.ClassName;
            command.Parameters.Add("@AcademicYear", SqlDbType.NVarChar).Value = schoolClass.AcademicYear;
            command.Parameters.Add("@Capacity", SqlDbType.Int).Value = schoolClass.Capacity;
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = schoolClass.IsActive;
        }

        private static async Task<List<ClassDetailsDTO>> ReadClassDetailsAsync(SqlCommand command)
        {
            List<ClassDetailsDTO> classes = [];

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                classes.Add(MapClassDetails(reader));
            }

            return classes;
        }

        #endregion

        #region Public Methods

        public async Task<List<ClassDetailsDTO>> GetAllClassesAsync()
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_GetAllClasses", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            return await ReadClassDetailsAsync(command);
        }

        public async Task<ClassDetailsDTO?> GetClassByIdAsync(int classId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_GetClassByID", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@ClassID", SqlDbType.Int).Value = classId;

            return (await ReadClassDetailsAsync(command)).FirstOrDefault();
        }

        public async Task<ClassDetailsDTO?> GetClassByDetailsAsync(byte gradeId, string className, string academicYear)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_GetClassByName", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@GradeID", SqlDbType.TinyInt).Value = gradeId;
            command.Parameters.Add("@ClassName", SqlDbType.NVarChar).Value = className;
            command.Parameters.Add("@AcademicYear", SqlDbType.NVarChar).Value = academicYear;

            return (await ReadClassDetailsAsync(command)).FirstOrDefault();
        }

        public async Task<int> AddClassAsync(ClassDTO schoolClass)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_AddClass", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            AddParameters(command, schoolClass);

            SqlParameter outputClassId = new("@ClassID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            command.Parameters.Add(outputClassId);

            await command.ExecuteNonQueryAsync();

            return (int)outputClassId.Value;
        }

        public async Task<bool> UpdateClassAsync(ClassDTO schoolClass)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_UpdateClass", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@ClassID", SqlDbType.Int).Value = schoolClass.ClassID;

            AddParameters(command, schoolClass);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteClassAsync(int classId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_DeleteClass", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@ClassID", SqlDbType.Int).Value = classId;

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> IsClassExistAsync(int classId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_IsClassExists", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@ClassID", SqlDbType.Int).Value = classId;

            return Convert.ToBoolean(await command.ExecuteScalarAsync());
        }

        #endregion
    }
}