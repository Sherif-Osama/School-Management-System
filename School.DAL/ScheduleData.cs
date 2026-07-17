using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DTO.ScheduleDTOs.School.DTO.ScheduleDTOs;
using System.Data;
namespace School.DAL
{
    public class ScheduleData : BaseData
    {

        public ScheduleData(IConfiguration configuration) : base(configuration) { }

        #region Helper Methods
        private static ScheduleDetailsDTO MapScheduleDetails(SqlDataReader reader)
        {
            return new ScheduleDetailsDTO
            {
                ScheduleID = reader.GetInt32(reader.GetOrdinal("ScheduleID")),

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

                LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? null : reader.GetString(reader.GetOrdinal("LastName")),

                ClassroomID = reader.GetInt32(reader.GetOrdinal("ClassroomID")),

                RoomName = reader.GetString(reader.GetOrdinal("RoomName")),

                DayOfWeek = reader.GetByte(reader.GetOrdinal("DayOfWeek")),

                DayName = reader.GetString(reader.GetOrdinal("DayName")),

                StartTime = TimeOnly.FromTimeSpan(reader.GetTimeSpan(reader.GetOrdinal("StartTime"))),

                EndTime = TimeOnly.FromTimeSpan(reader.GetTimeSpan(reader.GetOrdinal("EndTime")))
            };
        }

        private static void AddParameters(SqlCommand command, ScheduleDTO schedule)
        {
            command.Parameters.Add("@ClassSubjectID", SqlDbType.Int).Value = schedule.ClassSubjectID;
            command.Parameters.Add("@DayOfWeek", SqlDbType.TinyInt).Value = schedule.DayOfWeek;
            command.Parameters.Add("@StartTime", SqlDbType.Time).Value = schedule.StartTime.ToTimeSpan();
            command.Parameters.Add("@EndTime", SqlDbType.Time).Value = schedule.EndTime.ToTimeSpan();
            command.Parameters.Add("@ClassroomID", SqlDbType.Int).Value = schedule.ClassroomID;
        }

        private static async Task<List<ScheduleDetailsDTO>> ReadScheduleDetailsAsync(SqlCommand command)
        {
            List<ScheduleDetailsDTO> schedulesDetails = [];
            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                schedulesDetails.Add(MapScheduleDetails(reader));
            }

            return schedulesDetails;
        }

        private async Task<bool> CheckAvailabilityAsync(string procedureName, string idParameterName, int id, byte dayOfWeek, TimeOnly startTime, TimeOnly endTime, int? scheduleId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, procedureName);

            command.Parameters.Add(idParameterName, SqlDbType.Int).Value = id;
            command.Parameters.Add("@DayOfWeek", SqlDbType.TinyInt).Value = dayOfWeek;
            command.Parameters.Add("@StartTime", SqlDbType.Time).Value = startTime.ToTimeSpan();
            command.Parameters.Add("@EndTime", SqlDbType.Time).Value = endTime.ToTimeSpan();
            command.Parameters.Add("@ScheduleID", SqlDbType.Int).Value = (object?)scheduleId ?? DBNull.Value;

            return Convert.ToBoolean(await command.ExecuteScalarAsync());
        }
        #endregion

        #region Public Methods
        public async Task<List<ScheduleDetailsDTO>> GetAllSchedulesAsync()
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetAllSchedules");

            return await ReadScheduleDetailsAsync(command);
        }

        public async Task<ScheduleDetailsDTO?> GetScheduleByIdAsync(int scheduleId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetScheduleByID");

            command.Parameters.Add("@ScheduleID", SqlDbType.Int).Value = scheduleId;

            return (await ReadScheduleDetailsAsync(command)).FirstOrDefault();
        }

        public async Task<List<ScheduleDetailsDTO>> GetSchedulesByClassIdAsync(int classId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetSchedulesByClassID");
            command.Parameters.Add("@ClassID", SqlDbType.Int).Value = classId;
            return await ReadScheduleDetailsAsync(command);
        }

        public async Task<List<ScheduleDetailsDTO>> GetSchedulesByClassroomIdAsync(int classroomId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetSchedulesByClassroomID");
            command.Parameters.Add("@ClassroomID", SqlDbType.Int).Value = classroomId;

            return await ReadScheduleDetailsAsync(command);
        }

        public async Task<List<ScheduleDetailsDTO>> GetSchedulesByClassSubjectIdAsync(int classSubjectId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetSchedulesByClassSubjectID");

            command.Parameters.Add("@ClassSubjectID", SqlDbType.Int).Value = classSubjectId;

            return await ReadScheduleDetailsAsync(command);
        }

        public async Task<List<ScheduleDetailsDTO>> GetSchedulesByTeacherIdAsync(int teacherId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetSchedulesByTeacherID");
            command.Parameters.Add("@TeacherID", SqlDbType.Int).Value = teacherId;
            return await ReadScheduleDetailsAsync(command);
        }

        public async Task<int> AddScheduleAsync(ScheduleDTO schedule)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = CreateStoredProcedure(connection, "SP_AddSchedule");

            AddParameters(command, schedule);

            var outputScheduleId = new SqlParameter("@ScheduleID", SqlDbType.Int) { Direction = ParameterDirection.Output };

            command.Parameters.Add(outputScheduleId);

            await command.ExecuteNonQueryAsync();

            return (int)outputScheduleId.Value;
        }

        public async Task<bool> UpdateScheduleAsync(ScheduleDTO schedule)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_UpdateSchedule");
            command.Parameters.Add("@ScheduleID", SqlDbType.Int).Value = schedule.ScheduleID;
            AddParameters(command, schedule);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteScheduleAsync(int scheduleId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_DeleteSchedule");
            command.Parameters.Add("@ScheduleID", SqlDbType.Int).Value = scheduleId;
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> IsScheduleExistAsync(int scheduleId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_IsScheduleExists");

            command.Parameters.Add("@ScheduleID", SqlDbType.Int).Value = scheduleId;

            return Convert.ToBoolean(await command.ExecuteScalarAsync());
        }

        public Task<bool> IsClassAvailableAsync(int classId, byte dayOfWeek, TimeOnly startTime, TimeOnly endTime, int? scheduleId = null)
        {
            return CheckAvailabilityAsync("SP_IsClassAvailable", "@ClassID", classId, dayOfWeek, startTime, endTime, scheduleId);
        }

        public Task<bool> IsClassroomAvailableAsync(int classroomId, byte dayOfWeek, TimeOnly startTime, TimeOnly endTime, int? scheduleId = null)
        {
            return CheckAvailabilityAsync("SP_IsClassroomAvailable", "@ClassroomID", classroomId, dayOfWeek, startTime, endTime, scheduleId);
        }

        public Task<bool> IsTeacherAvailableAsync(int teacherId, byte dayOfWeek, TimeOnly startTime, TimeOnly endTime, int? scheduleId = null)
        {
            return CheckAvailabilityAsync("SP_IsTeacherAvailable", "@TeacherID", teacherId, dayOfWeek, startTime, endTime, scheduleId);
        }
        #endregion
    }
}