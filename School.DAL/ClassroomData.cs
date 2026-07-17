using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DTO.ClassroomDTOs;
using System.Data;
namespace School.DAL
{
    public class ClassroomData : BaseData
    {
        public ClassroomData(IConfiguration configuration) : base(configuration) { }

        #region Helper Methods
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
            command.Parameters.Add("@RoomName", SqlDbType.NVarChar, 20).Value = classroom.RoomName.Trim();
            command.Parameters.Add("@Capacity", SqlDbType.Int).Value = classroom.Capacity;
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
            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetAllClassrooms");
            return await ReadClassroomsAsync(command);
        }

        public async Task<ClassroomDTO?> GetClassroomByIdAsync(int classroomId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetClassroomByID");
            command.Parameters.Add("@ClassroomID", SqlDbType.Int).Value = classroomId;
            return (await ReadClassroomsAsync(command)).FirstOrDefault();
        }

        public async Task<ClassroomDTO?> GetClassroomByRoomNameAsync(string roomName)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetClassroomByRoomName");
            command.Parameters.Add("@RoomName", SqlDbType.NVarChar, 20).Value = roomName.Trim();
            return (await ReadClassroomsAsync(command)).FirstOrDefault();
        }

        public async Task<int> AddClassroomAsync(ClassroomDTO classroom)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_AddClassroom");
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
            using SqlCommand command = CreateStoredProcedure(connection, "SP_UpdateClassroom");
            command.Parameters.Add("@ClassroomID", SqlDbType.Int).Value = classroom.ClassroomID;
            AddParameters(command, classroom);
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteClassroomAsync(int classroomId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_DeleteClassroom");
            command.Parameters.Add("@ClassroomID", SqlDbType.Int).Value = classroomId;
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> IsClassroomExistAsync(int classroomId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_IsClassroomExists");
            command.Parameters.Add("@ClassroomID", SqlDbType.Int).Value = classroomId;
            return Convert.ToBoolean(await command.ExecuteScalarAsync());
        }
        #endregion
    }
}