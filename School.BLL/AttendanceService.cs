using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.AttendanceDTOs;
using School.DTO.StudentsDTOs;

namespace School.BLL
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceData _attendanceData;
        private readonly IStudentData _studentData;
        private readonly IClassData _classData;
        private readonly IAttendanceStatusData _attendanceStatusData;

        public AttendanceService(IAttendanceData attendanceData, IStudentData studentData, IClassData classData, IAttendanceStatusData attendanceStatusData)
        {
            _attendanceData = attendanceData;
            _studentData = studentData;
            _classData = classData;
            _attendanceStatusData = attendanceStatusData;
        }

        #region Validation
        private static void ValidateAttendance(AttendanceDTO attendance)
        {
            ArgumentNullException.ThrowIfNull(attendance);

            ValidateStudentId(attendance.StudentID);
            ValidateStatusId(attendance.StatusID);
            ValidateAttendanceDate(attendance.AttendanceDate);
        }

        private static void ValidateAttendanceId(int attendanceId)
        {
            if (attendanceId <= 0)
                throw new ArgumentException("AttendanceID must be a positive number.", nameof(attendanceId));
        }

        private static void ValidateStudentId(int studentId)
        {
            if (studentId <= 0)
                throw new ArgumentException("StudentID must be a positive number.", nameof(studentId));
        }

        private static void ValidateStatusId(int statusId)
        {
            if (statusId <= 0)
                throw new ArgumentException("StatusID must be a positive number.", nameof(statusId));
        }

        private static DateOnly ValidateAttendanceDate(DateOnly attendanceDate)
        {
            if (attendanceDate == default)
                throw new ArgumentException("AttendanceDate must be a valid date.", nameof(attendanceDate));

            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            if (attendanceDate > today)
                throw new ArgumentException("AttendanceDate cannot be in the future.", nameof(attendanceDate));

            return attendanceDate;
        }
        #endregion

        #region Ensure
        private async Task<StudentDetailsDTO> GetStudentOrThrowAsync(int studentId)
        {
            return await _studentData.GetStudentByIdAsync(studentId)
                ?? throw new InvalidOperationException($"Student with ID {studentId} does not exist.");
        }

        private async Task EnsureAttendanceExistsAsync(int attendanceId)
        {
            if (!await _attendanceData.IsAttendanceExistAsync(attendanceId))
                throw new KeyNotFoundException($"Attendance with ID {attendanceId} does not exist.");
        }

        private async Task EnsureAttendanceStatusExistsAsync(int statusId)
        {
            if (!await _attendanceStatusData.IsAttendanceStatusExistAsync(statusId))
                throw new InvalidOperationException($"AttendanceStatus with ID {statusId} does not exist.");
        }

        private static void EnsureStudentIsActive(StudentDetailsDTO student)
        {
            if (student.StatusID != 1) // Assuming 1 represents the "Active" status
                throw new InvalidOperationException($"Cannot record attendance for student {student.StudentID}: status is '{student.StatusName}', not Active '.");
        }

        private async Task EnsureAttendanceDateWithinAcademicYearAsync(StudentDetailsDTO student, DateOnly attendanceDate)
        {
            var schoolClass = await _classData.GetClassByIdAsync(student.ClassID)
                ?? throw new KeyNotFoundException($"Class with ID {student.ClassID} does not exist.");

            if (!schoolClass.IsActive)
                throw new InvalidOperationException("Cannot record attendance for an inactive class.");

            (DateOnly start, DateOnly end) = ParseAcademicYear(schoolClass.AcademicYear);

            if (attendanceDate < start || attendanceDate > end)
                throw new ArgumentException(
                    $"AttendanceDate must fall within the student's academic year {schoolClass.AcademicYear} ({start:yyyy-MM-dd} to {end:yyyy-MM-dd}).",
                    nameof(attendanceDate));
        }

        private static void EnsureAttendanceDateNotBeforeEnrollment(StudentDetailsDTO student, DateOnly attendanceDate)
        {
            DateOnly enrollmentDate = DateOnly.FromDateTime(student.EnrollmentDate);

            if (attendanceDate < enrollmentDate)
                throw new ArgumentException(
                    $"AttendanceDate cannot be before the student's enrollment date ({enrollmentDate:yyyy-MM-dd}).",
                    nameof(attendanceDate));
        }

        private async Task EnsureAttendanceUniqueAsync(int studentId, DateOnly attendanceDate, int? attendanceId = null)
        {
            List<AttendanceDetailsDTO> studentAttendances = await _attendanceData.GetAttendancesByStudentIdAsync(studentId);

            bool isDuplicate = studentAttendances.Exists(a =>
                a.AttendanceDate == attendanceDate &&
                (attendanceId == null || a.AttendanceID != attendanceId.Value));

            if (isDuplicate)
                throw new InvalidOperationException($"Student {studentId} already has an attendance record for {attendanceDate:yyyy-MM-dd}.");
        }

        private static (DateOnly Start, DateOnly End) ParseAcademicYear(string academicYear)
        {
            // Expected format: "2025-2026" -> academic year runs Sep 1, 2025 to Aug 31, 2026
            string[] parts = academicYear.Split('-');

            if (parts.Length != 2 ||
                !int.TryParse(parts[0], out int startYear) ||
                !int.TryParse(parts[1], out int endYear))
                throw new InvalidOperationException($"AcademicYear '{academicYear}' is not in a recognizable 'YYYY-YYYY' format.");

            return (new DateOnly(startYear, 9, 1), new DateOnly(endYear, 8, 31));
        }
        #endregion

        #region Public
        public Task<List<AttendanceDetailsDTO>> GetAllAttendancesAsync()
        {
            return _attendanceData.GetAllAttendancesAsync();
        }

        public Task<AttendanceDetailsDTO?> GetAttendanceByIdAsync(int attendanceId)
        {
            ValidateAttendanceId(attendanceId);

            return _attendanceData.GetAttendanceByIdAsync(attendanceId);
        }

        public Task<List<AttendanceDetailsDTO>> GetAttendancesByStudentIdAsync(int studentId)
        {
            ValidateStudentId(studentId);

            return _attendanceData.GetAttendancesByStudentIdAsync(studentId);
        }

        public Task<List<AttendanceDetailsDTO>> GetAttendancesByClassIdAsync(int classId)
        {
            if (classId <= 0)
                throw new ArgumentException("ClassID must be a positive number.", nameof(classId));

            return _attendanceData.GetAttendancesByClassIdAsync(classId);
        }

        public Task<List<AttendanceDetailsDTO>> GetAttendancesByDateAsync(DateOnly attendanceDate)
        {
            ValidateAttendanceDate(attendanceDate);

            return _attendanceData.GetAttendancesByDateAsync(attendanceDate);
        }

        public Task<List<AttendanceDetailsDTO>> GetAttendancesByStatusIdAsync(int statusId)
        {
            ValidateStatusId(statusId);

            return _attendanceData.GetAttendancesByStatusIdAsync(statusId);
        }

        public async Task<int> AddAttendanceAsync(AttendanceDTO attendance)
        {
            ValidateAttendance(attendance);

            StudentDetailsDTO student = await GetStudentOrThrowAsync(attendance.StudentID);
            await EnsureAttendanceStatusExistsAsync(attendance.StatusID);

            EnsureStudentIsActive(student);
            await EnsureAttendanceDateWithinAcademicYearAsync(student, attendance.AttendanceDate);
            EnsureAttendanceDateNotBeforeEnrollment(student, attendance.AttendanceDate);
            await EnsureAttendanceUniqueAsync(attendance.StudentID, attendance.AttendanceDate);

            return await _attendanceData.AddAttendanceAsync(attendance);
        }

        public async Task<bool> UpdateAttendanceAsync(AttendanceDTO attendance)
        {
            ValidateAttendance(attendance);
            ValidateAttendanceId(attendance.AttendanceID);

            await EnsureAttendanceExistsAsync(attendance.AttendanceID);

            StudentDetailsDTO student = await GetStudentOrThrowAsync(attendance.StudentID);
            await EnsureAttendanceStatusExistsAsync(attendance.StatusID);

            EnsureStudentIsActive(student);
            await EnsureAttendanceDateWithinAcademicYearAsync(student, attendance.AttendanceDate);
            EnsureAttendanceDateNotBeforeEnrollment(student, attendance.AttendanceDate);
            await EnsureAttendanceUniqueAsync(attendance.StudentID, attendance.AttendanceDate, attendance.AttendanceID);

            return await _attendanceData.UpdateAttendanceAsync(attendance);
        }

        public async Task<bool> DeleteAttendanceAsync(int attendanceId)
        {
            ValidateAttendanceId(attendanceId);

            await EnsureAttendanceExistsAsync(attendanceId);

            return await _attendanceData.DeleteAttendanceAsync(attendanceId);
        }
        #endregion
    }
}