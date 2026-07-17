using School.DAL.Interfaces;
using School.DTO.AssociationsDTOs.ClassSubjectDTOs;
using School.DTO.ScheduleDTOs;

namespace School.BLL
{
    public class ScheduleService
    {
        private readonly IScheduleData _scheduleData;
        private readonly IClassSubjectData _classSubjectData;
        private readonly IClassroomData _classroomData;
        private readonly ITeacherData _teacherData;

        public ScheduleService(IScheduleData scheduleData, IClassSubjectData classSubjectData, IClassroomData classroomData, ITeacherData teacherData)
        {
            _scheduleData = scheduleData;
            _classSubjectData = classSubjectData;
            _classroomData = classroomData;
            _teacherData = teacherData;
        }

        #region Validation
        private static void ValidateSchedule(ScheduleDTO schedule)
        {
            ArgumentNullException.ThrowIfNull(schedule);

            ValidateClassSubjectId(schedule.ClassSubjectID);
            ValidateClassroomId(schedule.ClassroomID);
            ValidateDayOfWeek(schedule.DayOfWeek);
            ValidateTime(schedule.StartTime, schedule.EndTime);
        }

        private static int ValidateScheduleId(int scheduleId)
        {
            if (scheduleId <= 0)
                throw new ArgumentException("ScheduleID must be a positive number.", nameof(scheduleId));

            return scheduleId;
        }

        private static int ValidateClassSubjectId(int classSubjectId)
        {
            if (classSubjectId <= 0)
                throw new ArgumentException("ClassSubjectID must be a positive number.", nameof(classSubjectId));

            return classSubjectId;
        }

        private static int ValidateClassroomId(int classroomId)
        {
            if (classroomId <= 0)
                throw new ArgumentException("ClassroomID must be a positive number.", nameof(classroomId));

            return classroomId;
        }

        private static byte ValidateDayOfWeek(byte dayOfWeek)
        {
            if (dayOfWeek is < 1 or > 5)
                throw new ArgumentException("DayOfWeek must be between 1 (Sunday) and 5 (Thursday).", nameof(dayOfWeek));

            return dayOfWeek;
        }

        private static void ValidateTime(TimeOnly startTime, TimeOnly endTime)
        {
            if (startTime >= endTime)
                throw new ArgumentException("StartTime must be earlier than EndTime.");
        }
        #endregion

        #region Ensure
        private async Task EnsureScheduleExistsAsync(int scheduleId)
        {
            if (!await _scheduleData.IsScheduleExistAsync(scheduleId))
                throw new InvalidOperationException($"Schedule with ID {scheduleId} does not exist.");
        }

        private async Task EnsureClassSubjectExistsAsync(int classSubjectId)
        {
            if (!await _classSubjectData.IsClassSubjectExistAsync(classSubjectId))
                throw new InvalidOperationException($"ClassSubject with ID {classSubjectId} does not exist.");
        }

        private async Task EnsureClassroomExistsAsync(int classroomId)
        {
            if (!await _classroomData.IsClassroomExistAsync(classroomId))
                throw new InvalidOperationException($"Classroom with ID {classroomId} does not exist.");
        }

        private async Task EnsureClassroomAvailableAsync(int classroomId, byte dayOfWeek, TimeOnly startTime, TimeOnly endTime, int? scheduleId = null)
        {
            bool isAvailable = await _scheduleData.IsClassroomAvailableAsync(classroomId, dayOfWeek, startTime, endTime, scheduleId);

            if (!isAvailable)
                throw new InvalidOperationException("The classroom is already booked for the specified day and time.");
        }

        private async Task<ClassSubjectDetailsDTO> EnsureClassSubjectAvailableAsync(int classSubjectId)
        {
            var classSubject = await _classSubjectData.GetClassSubjectByIdAsync(classSubjectId)
                ?? throw new InvalidOperationException($"ClassSubject with ID {classSubjectId} does not exist.");

            return classSubject;
        }

        private async Task EnsureTeacherExistsAsync(int teacherId)
        {
            if (!await _teacherData.IsTeacherExistAsync(teacherId))
                throw new InvalidOperationException($"Teacher with ID {teacherId} does not exist.");
        }

        private async Task EnsureTeacherAvailableAsync(int classSubjectId, byte dayOfWeek, TimeOnly startTime, TimeOnly endTime, int? scheduleId = null)
        {
            var classSubject = await EnsureClassSubjectAvailableAsync(classSubjectId);
            bool isAvailable = await _scheduleData.IsTeacherAvailableAsync(classSubject.TeacherID, dayOfWeek, startTime, endTime, scheduleId);

            if (!isAvailable)
                throw new InvalidOperationException("The teacher already has another class scheduled for the specified day and time.");
        }

        private async Task EnsureClassAvailableAsync(
            int classSubjectId,
            byte dayOfWeek,
            TimeOnly startTime,
            TimeOnly endTime,
            int? scheduleId = null)
        {

            var classSubject = await EnsureClassSubjectAvailableAsync(classSubjectId);
            bool isAvailable = await _scheduleData.IsClassAvailableAsync(classSubject.ClassID, dayOfWeek, startTime, endTime, scheduleId);

            if (!isAvailable)
                throw new InvalidOperationException("The class already has another subject scheduled for the specified day and time.");
        }
        #endregion

        #region Public
        public Task<List<ScheduleDetailsDTO>> GetAllSchedulesAsync()
        {
            return _scheduleData.GetAllSchedulesAsync();
        }

        public Task<ScheduleDetailsDTO?> GetScheduleByIdAsync(int scheduleId)
        {
            ValidateScheduleId(scheduleId);

            return _scheduleData.GetScheduleByIdAsync(scheduleId);
        }

        public Task<List<ScheduleDetailsDTO>> GetSchedulesByClassIdAsync(int classId)
        {
            if (classId <= 0)
                throw new ArgumentException("ClassID must be a positive number.", nameof(classId));

            return _scheduleData.GetSchedulesByClassIdAsync(classId);
        }

        public async Task<List<ScheduleDetailsDTO>> GetSchedulesByTeacherIdAsync(int teacherId)
        {
            if (teacherId <= 0)
                throw new ArgumentException("TeacherID must be a positive number.", nameof(teacherId));

            await EnsureTeacherExistsAsync(teacherId);

            return await _scheduleData.GetSchedulesByTeacherIdAsync(teacherId);
        }

        public async Task<List<ScheduleDetailsDTO>> GetSchedulesByClassroomIdAsync(int classroomId)
        {
            ValidateClassroomId(classroomId);
            await EnsureClassroomExistsAsync(classroomId);
            return await _scheduleData.GetSchedulesByClassroomIdAsync(classroomId);
        }

        public Task<List<ScheduleDetailsDTO>> GetSchedulesByClassSubjectIdAsync(int classSubjectId)
        {
            ValidateClassSubjectId(classSubjectId);
            return _scheduleData.GetSchedulesByClassSubjectIdAsync(classSubjectId);
        }

        public async Task<int> AddScheduleAsync(ScheduleDTO schedule)
        {
            ValidateSchedule(schedule);

            await EnsureClassSubjectExistsAsync(schedule.ClassSubjectID);
            await EnsureClassroomExistsAsync(schedule.ClassroomID);

            await EnsureClassroomAvailableAsync(schedule.ClassroomID, schedule.DayOfWeek, schedule.StartTime, schedule.EndTime);
            await EnsureTeacherAvailableAsync(schedule.ClassSubjectID, schedule.DayOfWeek, schedule.StartTime, schedule.EndTime);
            await EnsureClassAvailableAsync(schedule.ClassSubjectID, schedule.DayOfWeek, schedule.StartTime, schedule.EndTime);

            return await _scheduleData.AddScheduleAsync(schedule);
        }

        public async Task<bool> UpdateScheduleAsync(ScheduleDTO schedule)
        {
            ValidateSchedule(schedule);
            ValidateScheduleId(schedule.ScheduleID);

            await EnsureScheduleExistsAsync(schedule.ScheduleID);
            await EnsureClassSubjectExistsAsync(schedule.ClassSubjectID);
            await EnsureClassroomExistsAsync(schedule.ClassroomID);

            await EnsureClassroomAvailableAsync(schedule.ClassroomID, schedule.DayOfWeek, schedule.StartTime, schedule.EndTime, schedule.ScheduleID);
            await EnsureTeacherAvailableAsync(schedule.ClassSubjectID, schedule.DayOfWeek, schedule.StartTime, schedule.EndTime, schedule.ScheduleID);
            await EnsureClassAvailableAsync(schedule.ClassSubjectID, schedule.DayOfWeek, schedule.StartTime, schedule.EndTime, schedule.ScheduleID);

            return await _scheduleData.UpdateScheduleAsync(schedule);
        }

        public async Task<bool> DeleteScheduleAsync(int scheduleId)
        {
            ValidateScheduleId(scheduleId);

            await EnsureScheduleExistsAsync(scheduleId);

            return await _scheduleData.DeleteScheduleAsync(scheduleId);
        }
        #endregion
    }
}