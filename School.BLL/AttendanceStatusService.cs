using School.BLL.Common;
using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.AttendanceStatusDTOs;

namespace School.BLL
{
    public class AttendanceStatusService : IAttendanceStatusService
    {
        private readonly IAttendanceStatusData _attendanceStatusData;
        private static int MinStatusNameLength => 2;
        private static int MaxStatusNameLength => 50;
        public AttendanceStatusService(IAttendanceStatusData attendanceStatusData)
        {
            _attendanceStatusData = attendanceStatusData;
        }

        #region Validation
        private static void ValidateStatus(AttendanceStatusDTO status)
        {
            ArgumentNullException.ThrowIfNull(status);

            status.StatusName = ValidationHelper.ValidateString(status.StatusName, nameof(status.StatusName), MinStatusNameLength, MaxStatusNameLength);
        }
        #endregion

        #region Public
        public Task<List<AttendanceStatusDTO>> GetAllAttendanceStatusesAsync()
        {
            return _attendanceStatusData.GetAllAttendanceStatusesAsync();
        }

        public async Task<AttendanceStatusDTO?> GetAttendanceStatusByIdAsync(int statusId)
        {
            ValidationHelper.ValidateId(statusId);

            AttendanceStatusDTO? attendance = await _attendanceStatusData.GetAttendanceStatusByIdAsync(statusId);

            if (attendance == null)
                throw new KeyNotFoundException($"Attendance status with ID {statusId} does not exist.");

            return attendance;
        }

        public async Task<AttendanceStatusDTO?> GetAttendanceStatusByNameAsync(string statusName)
        {
            statusName = ValidationHelper.ValidateString(statusName, nameof(statusName), MinStatusNameLength, MaxStatusNameLength);

            AttendanceStatusDTO? attendance = await _attendanceStatusData.GetAttendanceStatusByNameAsync(statusName);

            if (attendance == null)
                throw new KeyNotFoundException($"Attendance status with name '{statusName}' does not exist.");

            return attendance;
        }

        public async Task<int> AddAttendanceStatusAsync(AttendanceStatusDTO status)
        {
            ValidateStatus(status);
            await EnsureHelper.EnsureUniqueAsync(_attendanceStatusData.GetAttendanceStatusByNameAsync, status.StatusName);

            int newStatusId = await _attendanceStatusData.AddAttendanceStatusAsync(status);

            if (newStatusId <= 0)
                throw new InvalidOperationException("Failed to add the attendance status.");

            return newStatusId;
        }

        public async Task<bool> UpdateAttendanceStatusAsync(AttendanceStatusDTO status)
        {
            ValidateStatus(status);
            ValidationHelper.ValidateId(status.StatusID);

            await EnsureHelper.EnsureExistsAsync(_attendanceStatusData.IsAttendanceStatusExistAsync, status.StatusID, "Attendance Status");
            await EnsureHelper.EnsureUniqueAsync(_attendanceStatusData.GetAttendanceStatusByNameAsync, status.StatusName, s => s.StatusID, status.StatusID);

            bool isUpdated = await _attendanceStatusData.UpdateAttendanceStatusAsync(status);

            if (!isUpdated)
                throw new InvalidOperationException($"Failed to update attendance status with ID {status.StatusID}.");

            return isUpdated;
        }

        public async Task<bool> DeleteAttendanceStatusAsync(int statusId)
        {
            ValidationHelper.ValidateId(statusId);

            await EnsureHelper.EnsureExistsAsync(_attendanceStatusData.IsAttendanceStatusExistAsync, statusId, "Attendance Status");

            bool isDeleted = await _attendanceStatusData.DeleteAttendanceStatusAsync(statusId);

            if (!isDeleted)
                throw new InvalidOperationException($"Failed to delete attendance status with ID {statusId}.");

            return isDeleted;
        }
        #endregion
    }
}