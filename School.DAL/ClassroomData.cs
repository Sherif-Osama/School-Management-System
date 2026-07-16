using Microsoft.Data.SqlClient;
using School.DTO.ClassroomDTOs;
using System.Data;

namespace School.DAL
{
    public class ClassroomData
    {
        private readonly string _connectionString;

        public ClassroomData(string connectionString)
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

        private static ClassroomDTO MapClassroom(SqlDataReader reader)
        {
            return new ClassroomDTO
            {
                ClassroomID = reader.GetInt32(reader.GetOrdinal("ClassroomID")),
                RoomName = reader.GetString(reader.GetOrdinal("RoomName")),
                Capacity = reader.GetInt32(reader.GetOrdinal("Capacity"))
            };
        }

        private static void AddParameters(SqlCommand command, ClassroomDTO classroom)
        {
            command.Parameters.Add("@RoomName", SqlDbType.NVarChar, 20).Value =
                classroom.RoomName.Trim();

            command.Parameters.Add("@Capacity", SqlDbType.Int).Value =
                classroom.Capacity;
        }

        private static async Task<List<ClassroomDTO>> ReadClassroomsAsync(SqlCommand command)
        {
            List<ClassroomDTO> classrooms = [];

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                classrooms.Add(MapClassroom(reader));

            return classrooms;
        }
        #endregion

        #region Public Methods

        public async Task<List<ClassroomDTO>> GetAllClassroomsAsync()
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_GetAllClassrooms", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            return await ReadClassroomsAsync(command);
        }

        public async Task<ClassroomDTO?> GetClassroomByIdAsync(int classroomId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_GetClassroomByID", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@ClassroomID", SqlDbType.Int).Value =
                classroomId;

            return (await ReadClassroomsAsync(command)).FirstOrDefault();
        }

        public async Task<ClassroomDTO?> GetClassroomByRoomNameAsync(string roomName)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_GetClassroomByRoomName", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@RoomName", SqlDbType.NVarChar, 20).Value =
                roomName.Trim();

            return (await ReadClassroomsAsync(command)).FirstOrDefault();
        }

        public async Task<int> AddClassroomAsync(ClassroomDTO classroom)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_AddClassroom", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            AddParameters(command, classroom);

            SqlParameter outputId = new("@ClassroomID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            command.Parameters.Add(outputId);

            await command.ExecuteNonQueryAsync();

            return (int)outputId.Value;
        }

        public async Task<bool> UpdateClassroomAsync(ClassroomDTO classroom)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_UpdateClassroom", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@ClassroomID", SqlDbType.Int).Value =
                classroom.ClassroomID;

            AddParameters(command, classroom);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteClassroomAsync(int classroomId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_DeleteClassroom", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@ClassroomID", SqlDbType.Int).Value =
                classroomId;

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> IsClassroomExistAsync(int classroomId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_IsClassroomExists", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@ClassroomID", SqlDbType.Int).Value =
                classroomId;

            return Convert.ToBoolean(await command.ExecuteScalarAsync());
        }
        #endregion
    }
}