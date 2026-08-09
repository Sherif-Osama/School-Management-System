using School.BLL.Common;
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

            ValidationHelper.ValidateId(attendance.StudentID);
            ValidationHelper.ValidateId(attendance.StatusID);
            ValidateAttendanceDate(attendance.AttendanceDate);
        }

        private static DateOnly ValidateAttendanceDate(DateOnly attendanceDate)
        {
            if (attendanceDate == default)
                throw new ArgumentException("AttendanceDate must be a valid date.", nameof(attendanceDate));

            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            if (attendanceDate > today) throw new ArgumentException("AttendanceDate cannot be in the future.", nameof(attendanceDate));

            return attendanceDate;
        }
        #endregion

        #region Ensure
        private async Task<StudentDetailsDTO> GetStudentOrThrowAsync(int studentId)
        {
            return await _studentData.GetStudentByIdAsync(studentId)
                ?? throw new KeyNotFoundException($"Student with ID {studentId} does not exist.");
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

            (DateOnly start, DateOnly end) = AcademicYearHelper.GetAcademicYearRange(schoolClass.AcademicYear);

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
            bool exists = await _attendanceData.IsStudentAttendanceExistsAsync(studentId, attendanceDate, attendanceId);

            if (exists)
            {
                throw new InvalidOperationException($"Student {studentId} already has an attendance record for {attendanceDate:yyyy-MM-dd}.");
            }
        }
        #endregion

        #region Public
        public Task<List<AttendanceDetailsDTO>> GetAllAttendancesAsync()
        {
            return _attendanceData.GetAllAttendancesAsync();
        }

        public async Task<AttendanceDetailsDTO?> GetAttendanceByIdAsync(int attendanceId)
        {
            ValidationHelper.ValidateId(attendanceId);

            AttendanceDetailsDTO? attendance = await _attendanceData.GetAttendanceByIdAsync(attendanceId);

            if (attendance == null)
                throw new KeyNotFoundException($"Attendance with ID {attendanceId} does not exist.");

            return attendance;
        }

        public async Task<List<AttendanceDetailsDTO>> GetAttendancesByStudentIdAsync(int studentId)
        {
            ValidationHelper.ValidateId(studentId);

            return await _attendanceData.GetAttendancesByStudentIdAsync(studentId);
        }

        public async Task<List<AttendanceDetailsDTO>> GetAttendancesByClassIdAsync(int classId)
        {
            ValidationHelper.ValidateId(classId);

            return await _attendanceData.GetAttendancesByClassIdAsync(classId);
        }

        public async Task<List<AttendanceDetailsDTO>> GetAttendancesByDateAsync(DateOnly attendanceDate)
        {
            ValidateAttendanceDate(attendanceDate);

            return await _attendanceData.GetAttendancesByDateAsync(attendanceDate);
        }

        public async Task<List<AttendanceDetailsDTO>> GetAttendancesByStatusIdAsync(int statusId)
        {
            ValidationHelper.ValidateId(statusId);

            return await _attendanceData.GetAttendancesByStatusIdAsync(statusId);
        }

        public async Task<int> AddAttendanceAsync(AttendanceDTO attendance)
        {
            ValidateAttendance(attendance);

            StudentDetailsDTO student = await GetStudentOrThrowAsync(attendance.StudentID);
            await EnsureHelper.EnsureExistsAsync(_attendanceStatusData.IsAttendanceStatusExistAsync, attendance.StatusID, "Attendance Status");

            EnsureStudentIsActive(student);
            await EnsureAttendanceDateWithinAcademicYearAsync(student, attendance.AttendanceDate);
            EnsureAttendanceDateNotBeforeEnrollment(student, attendance.AttendanceDate);
            await EnsureAttendanceUniqueAsync(attendance.StudentID, attendance.AttendanceDate);

            int newAttendanceId = await _attendanceData.AddAttendanceAsync(attendance);

            if (newAttendanceId <= 0)
                throw new InvalidOperationException("Failed to add attendance.");

            return newAttendanceId;
        }

        public async Task<bool> UpdateAttendanceAsync(AttendanceDTO attendance)
        {
            ValidateAttendance(attendance);
            ValidationHelper.ValidateId(attendance.AttendanceID);

            await EnsureHelper.EnsureExistsAsync(_attendanceData.IsAttendanceExistAsync, attendance.AttendanceID, "Attendance");

            StudentDetailsDTO student = await GetStudentOrThrowAsync(attendance.StudentID);
            await EnsureHelper.EnsureExistsAsync(_attendanceStatusData.IsAttendanceStatusExistAsync, attendance.StatusID, "Attendance Status");

            EnsureStudentIsActive(student);
            await EnsureAttendanceDateWithinAcademicYearAsync(student, attendance.AttendanceDate);
            EnsureAttendanceDateNotBeforeEnrollment(student, attendance.AttendanceDate);
            await EnsureAttendanceUniqueAsync(attendance.StudentID, attendance.AttendanceDate, attendance.AttendanceID);

            bool isUpdated = await _attendanceData.UpdateAttendanceAsync(attendance);

            if (!isUpdated)
                throw new InvalidOperationException($"Failed to update attendance with ID {attendance.AttendanceID}.");

            return isUpdated;
        }

        public async Task<bool> DeleteAttendanceAsync(int attendanceId)
        {
            ValidationHelper.ValidateId(attendanceId);

            await EnsureHelper.EnsureExistsAsync(_attendanceData.IsAttendanceExistAsync, attendanceId, "Attendance");

            bool isDeleted = await _attendanceData.DeleteAttendanceAsync(attendanceId);

            if (!isDeleted)
                throw new InvalidOperationException($"Failed to delete attendance with ID {attendanceId}.");

            return isDeleted;
        }
        #endregion
    }
}