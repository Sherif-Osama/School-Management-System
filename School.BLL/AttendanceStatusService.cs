using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.AttendanceStatusDTOs;

namespace School.BLL
{
    public class AttendanceStatusService : IAttendanceStatusService
    {
        private readonly IAttendanceStatusData _attendanceStatusData;

        public AttendanceStatusService(IAttendanceStatusData attendanceStatusData)
        {
            _attendanceStatusData = attendanceStatusData;
        }

        #region Validation
        private static void ValidateStatus(AttendanceStatusDTO status)
        {
            ArgumentNullException.ThrowIfNull(status);

            status.StatusName = ValidateStatusName(status.StatusName);
        }

        private static void ValidateStatusId(int statusId)
        {
            if (statusId <= 0)
                throw new ArgumentException("StatusID must be greater than zero.", nameof(statusId));
        }

        private static string ValidateStatusName(string statusName)
        {
            if (string.IsNullOrWhiteSpace(statusName))
                throw new ArgumentException("StatusName is required.", nameof(statusName));

            statusName = statusName.Trim();

            if (statusName.Length > 50)
                throw new ArgumentException("StatusName cannot exceed 50 characters.", nameof(statusName));

            return statusName;
        }
        #endregion

        #region Ensure
        private async Task EnsureStatusExistsAsync(int statusId)
        {
            if (!await _attendanceStatusData.IsAttendanceStatusExistAsync(statusId))
                throw new KeyNotFoundException($"AttendanceStatus with ID {statusId} does not exist.");
        }

        private async Task EnsureStatusNameUniqueAsync(string statusName, int? statusId = null)
        {
            AttendanceStatusDTO? existing = await _attendanceStatusData.GetAttendanceStatusByNameAsync(statusName);

            if (existing != null && (statusId == null || existing.StatusID != statusId.Value))
                throw new InvalidOperationException($"An attendance status named '{statusName}' already exists.");
        }
        #endregion

        #region Public
        public Task<List<AttendanceStatusDTO>> GetAllAttendanceStatusesAsync()
        {
            return _attendanceStatusData.GetAllAttendanceStatusesAsync();
        }

        public async Task<AttendanceStatusDTO?> GetAttendanceStatusByIdAsync(int statusId)
        {
            ValidateStatusId(statusId);

            AttendanceStatusDTO? attendance = await _attendanceStatusData.GetAttendanceStatusByIdAsync(statusId);

            if (attendance == null)
                throw new KeyNotFoundException($"Attendance status with ID {statusId} does not exist.");

            return attendance;
        }

        public async Task<AttendanceStatusDTO?> GetAttendanceStatusByNameAsync(string statusName)
        {
            statusName = ValidateStatusName(statusName);

            AttendanceStatusDTO? attendance = await _attendanceStatusData.GetAttendanceStatusByNameAsync(statusName);

            if (attendance == null)
                throw new KeyNotFoundException($"Attendance status with name '{statusName}' does not exist.");

            return attendance;
        }

        public async Task<int> AddAttendanceStatusAsync(AttendanceStatusDTO status)
        {
            ValidateStatus(status);

            await EnsureStatusNameUniqueAsync(status.StatusName);

            int newStatusId = await _attendanceStatusData.AddAttendanceStatusAsync(status);

            if (newStatusId <= 0)
                throw new InvalidOperationException("Failed to add the attendance status.");

            return newStatusId;
        }

        public async Task<bool> UpdateAttendanceStatusAsync(AttendanceStatusDTO status)
        {
            ValidateStatus(status);
            ValidateStatusId(status.StatusID);

            await EnsureStatusExistsAsync(status.StatusID);
            await EnsureStatusNameUniqueAsync(status.StatusName, status.StatusID);

            bool isUpdated = await _attendanceStatusData.UpdateAttendanceStatusAsync(status);

            if (!isUpdated)
                throw new InvalidOperationException($"Failed to update attendance status with ID {status.StatusID}.");

            return isUpdated;
        }

        public async Task<bool> DeleteAttendanceStatusAsync(int statusId)
        {
            ValidateStatusId(statusId);

            await EnsureStatusExistsAsync(statusId);

            bool isDeleted = await _attendanceStatusData.DeleteAttendanceStatusAsync(statusId);

            if (!isDeleted)
                throw new InvalidOperationException($"Failed to delete attendance status with ID {statusId}.");

            return isDeleted;
        }
        #endregion
    }
}