using Microsoft.Data.SqlClient;
using School.DTO.AttendanceDTOs;
using System.Data;

namespace School.DAL
{
    public class AttendanceData
    {
        private readonly string _connectionString;

        public AttendanceData(string connectionString)
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

        private static async Task<List<AttendanceDetailsDTO>> ReadAttendancesAsync(SqlCommand command)
        {
            List<AttendanceDetailsDTO> attendances = [];
            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                attendances.Add(MapAttendanceDetails(reader));
            }

            return attendances;
        }
        #endregion

        #region Public Methods
        public async Task<List<AttendanceDetailsDTO>> GetAllAttendancesAsync()
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = new("SP_GetAllAttendances", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            return await ReadAttendancesAsync(command);
        }

        public async Task<AttendanceDetailsDTO?> GetAttendanceByIdAsync(int attendanceId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = new("SP_GetAttendanceByID", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@AttendanceID", SqlDbType.Int).Value = attendanceId;

            return (await ReadAttendancesAsync(command)).FirstOrDefault();
        }

        public async Task<List<AttendanceDetailsDTO>> GetAttendancesByStudentIdAsync(int studentId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = new("SP_GetAttendancesByStudentID", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@StudentID", SqlDbType.Int).Value = studentId;

            return await ReadAttendancesAsync(command);
        }

        public async Task<List<AttendanceDetailsDTO>> GetAttendancesByClassIdAsync(int classId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = new("SP_GetAttendancesByClassID", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@ClassID", SqlDbType.Int).Value = classId;

            return await ReadAttendancesAsync(command);
        }

        public async Task<List<AttendanceDetailsDTO>> GetAttendancesByDateAsync(DateOnly attendanceDate)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = new("SP_GetAttendancesByDate", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@AttendanceDate", SqlDbType.Date).Value = attendanceDate.ToDateTime(TimeOnly.MinValue);

            return await ReadAttendancesAsync(command);
        }

        public async Task<List<AttendanceDetailsDTO>> GetAttendancesByStatusIdAsync(int statusId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = new("SP_GetAttendancesByStatusID", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@StatusID", SqlDbType.Int).Value = statusId;

            return await ReadAttendancesAsync(command);
        }

        public async Task<int> AddAttendanceAsync(AttendanceDTO attendance)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_AddAttendance", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            AddParameters(command, attendance);

            var outputAttendanceId = new SqlParameter("@AttendanceID", SqlDbType.Int) { Direction = ParameterDirection.Output };

            command.Parameters.Add(outputAttendanceId);

            await command.ExecuteNonQueryAsync();

            return (int)outputAttendanceId.Value;
        }

        public async Task<bool> UpdateAttendanceAsync(AttendanceDTO attendance)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = new("SP_UpdateAttendance", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@AttendanceID", SqlDbType.Int).Value = attendance.AttendanceID;
            AddParameters(command, attendance);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAttendanceAsync(int attendanceId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = new("SP_DeleteAttendance", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@AttendanceID", SqlDbType.Int).Value = attendanceId;

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> IsAttendanceExistAsync(int attendanceId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = new("SP_IsAttendanceExists", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@AttendanceID", SqlDbType.Int).Value = attendanceId;

            return Convert.ToBoolean(await command.ExecuteScalarAsync());
        }
        #endregion
    }
}