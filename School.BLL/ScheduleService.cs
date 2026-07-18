using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.AssociationsDTOs.ClassSubjectDTOs;
using School.DTO.ScheduleDTOs;

namespace School.BLL
{
    public class ScheduleService : IScheduleService
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

        private static void ValidateScheduleId(int scheduleId)
        {
            if (scheduleId <= 0)
                throw new ArgumentException("ScheduleID must be a positive number.", nameof(scheduleId));
        }


        private static void ValidateClassSubjectId(int classSubjectId)
        {
            if (classSubjectId <= 0)
                throw new ArgumentException("ClassSubjectID must be a positive number.", nameof(classSubjectId));
        }

        private static void ValidateClassroomId(int classroomId)
        {
            if (classroomId <= 0)
                throw new ArgumentException("ClassroomID must be a positive number.", nameof(classroomId));
        }

        private static void ValidateDayOfWeek(byte dayOfWeek)
        {
            if (dayOfWeek is < 1 or > 5)
                throw new ArgumentOutOfRangeException(nameof(dayOfWeek), "DayOfWeek must be between 1 (Sunday) and 5 (Thursday).");
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
                throw new KeyNotFoundException($"Schedule with ID {scheduleId} does not exist.");
        }

        private async Task EnsureClassroomExistsAsync(int classroomId)
        {
            if (!await _classroomData.IsClassroomExistAsync(classroomId))
                throw new KeyNotFoundException($"Classroom with ID {classroomId} does not exist.");
        }

        private async Task EnsureClassroomAvailableAsync(int classroomId, byte dayOfWeek, TimeOnly startTime, TimeOnly endTime, int? scheduleId = null)
        {
            bool isAvailable = await _scheduleData.IsClassroomAvailableAsync(classroomId, dayOfWeek, startTime, endTime, scheduleId);

            if (!isAvailable)
                throw new InvalidOperationException("The classroom is already booked for the specified day and time.");
        }

        private async Task<ClassSubjectDetailsDTO> GetValidatedClassSubjectAsync(int classSubjectId)
        {
            ClassSubjectDetailsDTO? classSubject = await _classSubjectData.GetClassSubjectByIdAsync(classSubjectId);

            return classSubject
                ?? throw new KeyNotFoundException(
                    $"ClassSubject with ID {classSubjectId} does not exist.");
        }

        private async Task EnsureTeacherExistsAsync(int teacherId)
        {
            if (!await _teacherData.IsTeacherExistAsync(teacherId))
                throw new KeyNotFoundException($"Teacher with ID {teacherId} does not exist.");
        }

        private async Task EnsureTeacherAvailableAsync(ClassSubjectDetailsDTO classSubject, byte dayOfWeek, TimeOnly startTime, TimeOnly endTime, int? scheduleId = null)
        {
            bool isAvailable = await _scheduleData.IsTeacherAvailableAsync(classSubject.TeacherID, dayOfWeek, startTime, endTime, scheduleId);

            if (!isAvailable)
                throw new InvalidOperationException("The teacher already has another class scheduled for the specified day and time.");
        }

        private async Task EnsureClassAvailableAsync(ClassSubjectDetailsDTO classSubject, byte dayOfWeek, TimeOnly startTime, TimeOnly endTime, int? scheduleId = null)
        {
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

        public async Task<ScheduleDetailsDTO?> GetScheduleByIdAsync(int scheduleId)
        {
            ValidateScheduleId(scheduleId);
            ScheduleDetailsDTO? scheduleDetails = await _scheduleData.GetScheduleByIdAsync(scheduleId);

            if (scheduleDetails == null)
                throw new KeyNotFoundException($"Schedule with ID {scheduleId} does not exist.");

            return scheduleDetails;
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

            ClassSubjectDetailsDTO classSubject = await GetValidatedClassSubjectAsync(schedule.ClassSubjectID);

            await EnsureClassroomExistsAsync(schedule.ClassroomID);

            await EnsureClassroomAvailableAsync(schedule.ClassroomID, schedule.DayOfWeek, schedule.StartTime, schedule.EndTime);

            await EnsureTeacherAvailableAsync(classSubject, schedule.DayOfWeek, schedule.StartTime, schedule.EndTime);

            await EnsureClassAvailableAsync(classSubject, schedule.DayOfWeek, schedule.StartTime, schedule.EndTime);

            int newScheduleId = await _scheduleData.AddScheduleAsync(schedule);

            if (newScheduleId <= 0)
                throw new InvalidOperationException("Failed to add the schedule.");

            return newScheduleId;
        }

        public async Task<bool> UpdateScheduleAsync(ScheduleDTO schedule)
        {
            ValidateSchedule(schedule);
            ValidateScheduleId(schedule.ScheduleID);

            await EnsureScheduleExistsAsync(schedule.ScheduleID);
            ClassSubjectDetailsDTO classSubject = await GetValidatedClassSubjectAsync(schedule.ClassSubjectID);
            await EnsureClassroomExistsAsync(schedule.ClassroomID);
            await EnsureClassroomAvailableAsync(schedule.ClassroomID, schedule.DayOfWeek, schedule.StartTime, schedule.EndTime, schedule.ScheduleID);
            await EnsureTeacherAvailableAsync(classSubject, schedule.DayOfWeek, schedule.StartTime, schedule.EndTime, schedule.ScheduleID);
            await EnsureClassAvailableAsync(classSubject, schedule.DayOfWeek, schedule.StartTime, schedule.EndTime, schedule.ScheduleID);

            bool isUpdated = await _scheduleData.UpdateScheduleAsync(schedule);

            if (!isUpdated)
                throw new InvalidOperationException($"Failed to update the schedule with ID {schedule.ScheduleID}.");

            return isUpdated;
        }

        public async Task<bool> DeleteScheduleAsync(int scheduleId)
        {
            ValidateScheduleId(scheduleId);

            await EnsureScheduleExistsAsync(scheduleId);

            bool isDeleted = await _scheduleData.DeleteScheduleAsync(scheduleId);

            if (!isDeleted)
                throw new InvalidOperationException($"Failed to delete the schedule with ID {scheduleId}.");

            return isDeleted;
        }
        #endregion
    }
}