using School.DAL.Interfaces;
using School.DTO.AttendanceStatusDTOs;

namespace School.BLL
{
    public class AttendanceStatusService
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

            ValidateStatusName(status.StatusName);
        }

        private static int ValidateStatusId(int statusId)
        {
            if (statusId <= 0)
                throw new ArgumentException("StatusID must be a positive number.", nameof(statusId));

            return statusId;
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
                throw new InvalidOperationException($"AttendanceStatus with ID {statusId} does not exist.");
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

        public Task<AttendanceStatusDTO?> GetAttendanceStatusByIdAsync(int statusId)
        {
            ValidateStatusId(statusId);

            return _attendanceStatusData.GetAttendanceStatusByIdAsync(statusId);
        }

        public Task<AttendanceStatusDTO?> GetAttendanceStatusByNameAsync(string statusName)
        {
            statusName = ValidateStatusName(statusName);

            return _attendanceStatusData.GetAttendanceStatusByNameAsync(statusName);
        }

        public async Task<int> AddAttendanceStatusAsync(AttendanceStatusDTO status)
        {
            ValidateStatus(status);

            await EnsureStatusNameUniqueAsync(status.StatusName);

            return await _attendanceStatusData.AddAttendanceStatusAsync(status);
        }

        public async Task<bool> UpdateAttendanceStatusAsync(AttendanceStatusDTO status)
        {
            ValidateStatus(status);
            ValidateStatusId(status.StatusID);

            await EnsureStatusExistsAsync(status.StatusID);
            await EnsureStatusNameUniqueAsync(status.StatusName, status.StatusID);

            return await _attendanceStatusData.UpdateAttendanceStatusAsync(status);
        }

        public async Task<bool> DeleteAttendanceStatusAsync(int statusId)
        {
            ValidateStatusId(statusId);

            await EnsureStatusExistsAsync(statusId);

            return await _attendanceStatusData.DeleteAttendanceStatusAsync(statusId);
        }
        #endregion
    }
}