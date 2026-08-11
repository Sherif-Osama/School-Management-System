using School.BLL.Common;
using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.AssociationsDTOs.ClassSubjectDTOs.Responses;
using School.DTO.ScheduleDTOs.Requests;
using School.DTO.ScheduleDTOs.Responses;

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
        private static void ValidateSchedule(CreateScheduleRequest schedule)
        {
            ArgumentNullException.ThrowIfNull(schedule);

            ValidationHelper.ValidateId(schedule.ClassSubjectID);
            ValidationHelper.ValidateId(schedule.ClassroomID);
            ValidateDayOfWeek(schedule.DayOfWeek);
            ValidateTime(schedule.StartTime, schedule.EndTime);
        }
        private static void ValidateSchedule(UpdateScheduleRequest schedule)
        {
            ArgumentNullException.ThrowIfNull(schedule);
            ValidationHelper.ValidateId(schedule.ClassSubjectID);
            ValidationHelper.ValidateId(schedule.ClassroomID);
            ValidateDayOfWeek(schedule.DayOfWeek);
            ValidateTime(schedule.StartTime, schedule.EndTime);
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
        private async Task EnsureClassroomAvailableAsync(int classroomId, byte dayOfWeek, TimeOnly startTime, TimeOnly endTime, int? scheduleId = null)
        {
            bool isAvailable = await _scheduleData.IsClassroomAvailableAsync(classroomId, dayOfWeek, startTime, endTime, scheduleId);

            if (!isAvailable)
                throw new InvalidOperationException("The classroom is already booked for the specified day and time.");
        }

        private async Task<ClassSubjectResponse> GetValidatedClassSubjectAsync(int classSubjectId)
        {
            ClassSubjectResponse? classSubject = await _classSubjectData.GetClassSubjectByIdAsync(classSubjectId);

            return classSubject
                ?? throw new KeyNotFoundException(
                    $"ClassSubject with ID {classSubjectId} does not exist.");
        }

        private async Task EnsureTeacherAvailableAsync(ClassSubjectResponse classSubject, byte dayOfWeek, TimeOnly startTime, TimeOnly endTime, int? scheduleId = null)
        {
            bool isAvailable = await _scheduleData.IsTeacherAvailableAsync(classSubject.TeacherID, dayOfWeek, startTime, endTime, scheduleId);

            if (!isAvailable)
                throw new InvalidOperationException("The teacher already has another class scheduled for the specified day and time.");
        }

        private async Task EnsureClassAvailableAsync(ClassSubjectResponse classSubject, byte dayOfWeek, TimeOnly startTime, TimeOnly endTime, int? scheduleId = null)
        {
            bool isAvailable = await _scheduleData.IsClassAvailableAsync(classSubject.ClassID, dayOfWeek, startTime, endTime, scheduleId);

            if (!isAvailable)
                throw new InvalidOperationException("The class already has another subject scheduled for the specified day and time.");
        }
        #endregion

        #region Public
        public Task<List<ScheduleResponse>> GetAllSchedulesAsync()
        {
            return _scheduleData.GetAllSchedulesAsync();
        }

        public async Task<ScheduleResponse?> GetScheduleByIdAsync(int scheduleId)
        {
            ValidationHelper.ValidateId(scheduleId);
            ScheduleResponse? scheduleDetails = await _scheduleData.GetScheduleByIdAsync(scheduleId);

            if (scheduleDetails == null)
                throw new KeyNotFoundException($"Schedule with ID {scheduleId} does not exist.");

            return scheduleDetails;
        }

        public Task<List<ScheduleResponse>> GetSchedulesByClassIdAsync(int classId)
        {
            ValidationHelper.ValidateId(classId);

            return _scheduleData.GetSchedulesByClassIdAsync(classId);
        }

        public async Task<List<ScheduleResponse>> GetSchedulesByTeacherIdAsync(int teacherId)
        {
            ValidationHelper.ValidateId(teacherId);

            await EnsureHelper.EnsureExistsAsync(_teacherData.IsTeacherExistAsync, teacherId, "Teacher");

            return await _scheduleData.GetSchedulesByTeacherIdAsync(teacherId);
        }

        public async Task<List<ScheduleResponse>> GetSchedulesByClassroomIdAsync(int classroomId)
        {
            ValidationHelper.ValidateId(classroomId);

            await EnsureHelper.EnsureExistsAsync(_classroomData.IsClassroomExistAsync, classroomId, "Classroom");

            return await _scheduleData.GetSchedulesByClassroomIdAsync(classroomId);
        }

        public Task<List<ScheduleResponse>> GetSchedulesByClassSubjectIdAsync(int classSubjectId)
        {
            ValidationHelper.ValidateId(classSubjectId);

            return _scheduleData.GetSchedulesByClassSubjectIdAsync(classSubjectId);
        }

        public async Task<int> AddScheduleAsync(CreateScheduleRequest schedule)
        {
            ValidateSchedule(schedule);

            ClassSubjectResponse classSubject = await GetValidatedClassSubjectAsync(schedule.ClassSubjectID);

            await EnsureHelper.EnsureExistsAsync(_classroomData.IsClassroomExistAsync, schedule.ClassroomID, "Classroom");

            await EnsureClassroomAvailableAsync(schedule.ClassroomID, schedule.DayOfWeek, schedule.StartTime, schedule.EndTime);

            await EnsureTeacherAvailableAsync(classSubject, schedule.DayOfWeek, schedule.StartTime, schedule.EndTime);

            await EnsureClassAvailableAsync(classSubject, schedule.DayOfWeek, schedule.StartTime, schedule.EndTime);

            int newScheduleId = await _scheduleData.AddScheduleAsync(schedule);

            if (newScheduleId <= 0)
                throw new InvalidOperationException("Failed to add the schedule.");

            return newScheduleId;
        }

        public async Task<bool> UpdateScheduleAsync(int scheduleId, UpdateScheduleRequest schedule)
        {
            ValidateSchedule(schedule);
            ValidationHelper.ValidateId(scheduleId);

            await EnsureHelper.EnsureExistsAsync(_scheduleData.IsScheduleExistAsync, scheduleId, "Schedule");
            ClassSubjectResponse classSubject = await GetValidatedClassSubjectAsync(schedule.ClassSubjectID);
            await EnsureHelper.EnsureExistsAsync(_classroomData.IsClassroomExistAsync, schedule.ClassroomID, "Classroom");
            await EnsureClassroomAvailableAsync(schedule.ClassroomID, schedule.DayOfWeek, schedule.StartTime, schedule.EndTime, scheduleId);
            await EnsureTeacherAvailableAsync(classSubject, schedule.DayOfWeek, schedule.StartTime, schedule.EndTime, scheduleId);
            await EnsureClassAvailableAsync(classSubject, schedule.DayOfWeek, schedule.StartTime, schedule.EndTime, scheduleId);

            bool isUpdated = await _scheduleData.UpdateScheduleAsync(scheduleId, schedule);

            if (!isUpdated)
                throw new InvalidOperationException($"Failed to update the schedule with ID {scheduleId}.");

            return isUpdated;
        }

        public async Task<bool> DeleteScheduleAsync(int scheduleId)
        {
            ValidationHelper.ValidateId(scheduleId);

            await EnsureHelper.EnsureExistsAsync(_scheduleData.IsScheduleExistAsync, scheduleId, "Schedule");

            bool isDeleted = await _scheduleData.DeleteScheduleAsync(scheduleId);

            if (!isDeleted)
                throw new InvalidOperationException($"Failed to delete the schedule with ID {scheduleId}.");

            return isDeleted;
        }
        #endregion
    }
}