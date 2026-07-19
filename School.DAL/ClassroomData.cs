using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DAL.Interfaces;
using School.DTO.ClassroomDTOs;
using System.Data;

namespace School.DAL
{
    public class ClassroomData : BaseData, IClassroomData
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

        #endregion

        #region Public Methods

        public Task<List<ClassroomDTO>> GetAllClassroomsAsync() => QueryListAsync("SP_GetAllClassrooms", null, MapClassroom);

        public Task<ClassroomDTO?> GetClassroomByIdAsync(int classroomId) =>
            QuerySingleAsync("SP_GetClassroomByID", cmd => cmd.Parameters.Add("@ClassroomID", SqlDbType.Int).Value = classroomId,
                MapClassroom);

        public Task<ClassroomDTO?> GetClassroomByRoomNameAsync(string roomName) =>
            QuerySingleAsync("SP_GetClassroomByRoomName", cmd => cmd.Parameters.Add("@RoomName", SqlDbType.NVarChar, 20).Value = roomName.Trim(),
                MapClassroom);

        public Task<int> AddClassroomAsync(ClassroomDTO classroom) =>
            InsertAsync<int>("SP_AddClassroom", cmd => AddParameters(cmd, classroom), "@ClassroomID", SqlDbType.Int);

        public Task<bool> UpdateClassroomAsync(ClassroomDTO classroom) =>
            ExecuteNonQueryAsync("SP_UpdateClassroom",
                cmd =>
                {
                    cmd.Parameters.Add("@ClassroomID", SqlDbType.Int).Value = classroom.ClassroomID;
                    AddParameters(cmd, classroom);
                });

        public Task<bool> DeleteClassroomAsync(int classroomId) =>
            ExecuteNonQueryAsync("SP_DeleteClassroom", cmd => cmd.Parameters.Add("@ClassroomID", SqlDbType.Int).Value = classroomId);

        public Task<bool> IsClassroomExistAsync(int classroomId) =>
            ExecuteExistsAsync("SP_IsClassroomExists", cmd => cmd.Parameters.Add("@ClassroomID", SqlDbType.Int).Value = classroomId);

        #endregion
    }
}