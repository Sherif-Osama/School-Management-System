using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DAL.Interfaces;
using School.DTO.AttendanceDTOs;
using System.Data;

namespace School.DAL
{
    public class AttendanceData : BaseData, IAttendanceData
    {
        public AttendanceData(IConfiguration configuration) : base(configuration) { }

        #region Helper Methods

        private static AttendanceDetailsDTO MapAttendanceDetails(SqlDataReader reader)
        {
            return new AttendanceDetailsDTO
            {
                AttendanceID = reader.GetInt32(reader.GetOrdinal("AttendanceID")),
                StudentID = reader.GetInt32(reader.GetOrdinal("StudentID")),
                PersonID = reader.GetInt32(reader.GetOrdinal("PersonID")),
                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                SecondName = reader.GetString(reader.GetOrdinal("SecondName")),
                ThirdName = reader.GetString(reader.GetOrdinal("ThirdName")),
                LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? null : reader.GetString(reader.GetOrdinal("LastName")),
                GradeID = reader.GetByte(reader.GetOrdinal("GradeID")),
                GradeName = reader.GetString(reader.GetOrdinal("GradeName")),
                ClassID = reader.GetInt32(reader.GetOrdinal("ClassID")),
                ClassName = reader.GetString(reader.GetOrdinal("ClassName")),
                AcademicYear = reader.GetString(reader.GetOrdinal("AcademicYear")),
                AttendanceDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("AttendanceDate"))),
                StatusID = reader.GetInt32(reader.GetOrdinal("StatusID")),
                StatusName = reader.GetString(reader.GetOrdinal("StatusName"))
            };
        }

        private static void AddParameters(SqlCommand command, AttendanceDTO attendance)
        {
            command.Parameters.Add("@StudentID", SqlDbType.Int).Value = attendance.StudentID;
            command.Parameters.Add("@AttendanceDate", SqlDbType.Date).Value = attendance.AttendanceDate.ToDateTime(TimeOnly.MinValue);
            command.Parameters.Add("@StatusID", SqlDbType.Int).Value = attendance.StatusID;
        }

        #endregion

        #region Public Methods

        public Task<List<AttendanceDetailsDTO>> GetAllAttendancesAsync() =>
            QueryListAsync("SP_GetAllAttendances", null, MapAttendanceDetails);

        public Task<AttendanceDetailsDTO?> GetAttendanceByIdAsync(int attendanceId) =>
            QuerySingleAsync("SP_GetAttendanceByID", cmd => cmd.Parameters.Add("@AttendanceID", SqlDbType.Int).Value = attendanceId,
                MapAttendanceDetails);

        public Task<List<AttendanceDetailsDTO>> GetAttendancesByStudentIdAsync(int studentId) =>
            QueryListAsync("SP_GetAttendancesByStudentID", cmd => cmd.Parameters.Add("@StudentID", SqlDbType.Int).Value = studentId,
                MapAttendanceDetails);

        public Task<List<AttendanceDetailsDTO>> GetAttendancesByClassIdAsync(int classId) =>
            QueryListAsync("SP_GetAttendancesByClassID", cmd => cmd.Parameters.Add("@ClassID", SqlDbType.Int).Value = classId,
                MapAttendanceDetails);

        public Task<List<AttendanceDetailsDTO>> GetAttendancesByDateAsync(DateOnly attendanceDate) =>
            QueryListAsync("SP_GetAttendancesByDate", cmd => cmd.Parameters.Add("@AttendanceDate", SqlDbType.Date).Value = attendanceDate.ToDateTime(TimeOnly.MinValue),
                MapAttendanceDetails);

        public Task<List<AttendanceDetailsDTO>> GetAttendancesByStatusIdAsync(int statusId) =>
            QueryListAsync("SP_GetAttendancesByStatusID", cmd => cmd.Parameters.Add("@StatusID", SqlDbType.Int).Value = statusId,
                MapAttendanceDetails);

        public Task<int> AddAttendanceAsync(AttendanceDTO attendance) =>
            InsertAsync<int>("SP_AddAttendance", cmd => AddParameters(cmd, attendance), "@AttendanceID", SqlDbType.Int);

        public Task<bool> UpdateAttendanceAsync(AttendanceDTO attendance) =>
            ExecuteNonQueryAsync("SP_UpdateAttendance",
                cmd =>
                {
                    cmd.Parameters.Add("@AttendanceID", SqlDbType.Int).Value = attendance.AttendanceID;
                    AddParameters(cmd, attendance);
                });

        public Task<bool> DeleteAttendanceAsync(int attendanceId) =>
            ExecuteNonQueryAsync("SP_DeleteAttendance", cmd => cmd.Parameters.Add("@AttendanceID", SqlDbType.Int).Value = attendanceId);

        public Task<bool> IsAttendanceExistAsync(int attendanceId) =>
            ExecuteExistsAsync("SP_IsAttendanceExists", cmd => cmd.Parameters.Add("@AttendanceID", SqlDbType.Int).Value = attendanceId);

        public Task<bool> IsStudentAttendanceExistsAsync(int studentId, DateOnly attendanceDate, int? attendanceId = null) =>
            ExecuteExistsAsync("SP_IsStudentAttendanceExists",
                cmd =>
                {
                    cmd.Parameters.Add("@StudentID", SqlDbType.Int).Value = studentId;
                    cmd.Parameters.Add("@AttendanceDate", SqlDbType.Date).Value = attendanceDate.ToDateTime(TimeOnly.MinValue);
                    cmd.Parameters.Add("@AttendanceID", SqlDbType.Int).Value = attendanceId ?? (object)DBNull.Value;
                });

        #endregion
    }
}