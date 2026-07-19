using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DAL.Interfaces;
using School.DTO.ScheduleDTOs;
using System.Data;

namespace School.DAL
{
    public class ScheduleData : BaseData, IScheduleData
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

        private Task<bool> CheckAvailabilityAsync(
            string procedureName, string idParameterName, int id,
            byte dayOfWeek, TimeOnly startTime, TimeOnly endTime, int? scheduleId) =>
            ExecuteExistsAsync(procedureName, cmd =>
            {
                cmd.Parameters.Add(idParameterName, SqlDbType.Int).Value = id;
                cmd.Parameters.Add("@DayOfWeek", SqlDbType.TinyInt).Value = dayOfWeek;
                cmd.Parameters.Add("@StartTime", SqlDbType.Time).Value = startTime.ToTimeSpan();
                cmd.Parameters.Add("@EndTime", SqlDbType.Time).Value = endTime.ToTimeSpan();
                cmd.Parameters.Add("@ScheduleID", SqlDbType.Int).Value = (object?)scheduleId ?? DBNull.Value;
            });

        #endregion Helper Methods

        #region Public Methods

        public Task<List<ScheduleDetailsDTO>> GetAllSchedulesAsync() =>
            QueryListAsync("SP_GetAllSchedules", null, MapScheduleDetails);

        public Task<ScheduleDetailsDTO?> GetScheduleByIdAsync(int scheduleId) =>
            QuerySingleAsync("SP_GetScheduleByID", cmd => cmd.Parameters.Add("@ScheduleID", SqlDbType.Int).Value = scheduleId,
                MapScheduleDetails);

        public Task<List<ScheduleDetailsDTO>> GetSchedulesByClassIdAsync(int classId) =>
            QueryListAsync("SP_GetSchedulesByClassID", cmd => cmd.Parameters.Add("@ClassID", SqlDbType.Int).Value = classId,
                MapScheduleDetails);

        public Task<List<ScheduleDetailsDTO>> GetSchedulesByClassroomIdAsync(int classroomId) =>
            QueryListAsync("SP_GetSchedulesByClassroomID", cmd => cmd.Parameters.Add("@ClassroomID", SqlDbType.Int).Value = classroomId,
                MapScheduleDetails);

        public Task<List<ScheduleDetailsDTO>> GetSchedulesByClassSubjectIdAsync(int classSubjectId) =>
            QueryListAsync("SP_GetSchedulesByClassSubjectID", cmd => cmd.Parameters.Add("@ClassSubjectID", SqlDbType.Int).Value = classSubjectId,
                MapScheduleDetails);

        public Task<List<ScheduleDetailsDTO>> GetSchedulesByTeacherIdAsync(int teacherId) =>
            QueryListAsync("SP_GetSchedulesByTeacherID", cmd => cmd.Parameters.Add("@TeacherID", SqlDbType.Int).Value = teacherId,
                MapScheduleDetails);

        public Task<int> AddScheduleAsync(ScheduleDTO schedule) =>
            InsertAsync<int>("SP_AddSchedule", cmd => AddParameters(cmd, schedule), "@ScheduleID",
                SqlDbType.Int);

        public Task<bool> UpdateScheduleAsync(ScheduleDTO schedule) =>
            ExecuteNonQueryAsync("SP_UpdateSchedule",
                cmd =>
                {
                    cmd.Parameters.Add("@ScheduleID", SqlDbType.Int).Value = schedule.ScheduleID;
                    AddParameters(cmd, schedule);
                });

        public Task<bool> DeleteScheduleAsync(int scheduleId) =>
            ExecuteNonQueryAsync("SP_DeleteSchedule", cmd => cmd.Parameters.Add("@ScheduleID", SqlDbType.Int).Value = scheduleId);

        public Task<bool> IsScheduleExistAsync(int scheduleId) =>
            ExecuteExistsAsync("SP_IsScheduleExists", cmd => cmd.Parameters.Add("@ScheduleID", SqlDbType.Int).Value = scheduleId);

        public Task<bool> IsClassAvailableAsync(int classId, byte dayOfWeek, TimeOnly startTime, TimeOnly endTime, int? scheduleId = null) =>
            CheckAvailabilityAsync("SP_IsClassAvailable", "@ClassID", classId, dayOfWeek, startTime, endTime, scheduleId);

        public Task<bool> IsClassroomAvailableAsync(int classroomId, byte dayOfWeek, TimeOnly startTime, TimeOnly endTime, int? scheduleId = null) =>
            CheckAvailabilityAsync("SP_IsClassroomAvailable", "@ClassroomID", classroomId, dayOfWeek, startTime, endTime, scheduleId);

        public Task<bool> IsTeacherAvailableAsync(int teacherId, byte dayOfWeek, TimeOnly startTime, TimeOnly endTime, int? scheduleId = null) =>
            CheckAvailabilityAsync("SP_IsTeacherAvailable", "@TeacherID", teacherId, dayOfWeek, startTime, endTime, scheduleId);

        #endregion
    }
}