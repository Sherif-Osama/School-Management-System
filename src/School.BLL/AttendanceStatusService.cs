using School.BLL.Common;
using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.AttendanceStatusDTOs.Requests;
using School.DTO.AttendanceStatusDTOs.Responses;

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
        private static void ValidateStatus(AttendanceStatusRequest status)
        {
            ArgumentNullException.ThrowIfNull(status);

            status.StatusName = ValidationHelper.ValidateString(status.StatusName, nameof(status.StatusName), MinStatusNameLength, MaxStatusNameLength);
        }
        #endregion

        #region Public
        public Task<List<AttendanceStatusResponse>> GetAllAttendanceStatusesAsync()
        {
            return _attendanceStatusData.GetAllAttendanceStatusesAsync();
        }

        public async Task<AttendanceStatusResponse> GetAttendanceStatusByIdAsync(int statusId)
        {
            ValidationHelper.ValidateId(statusId);

            AttendanceStatusResponse? attendance = await _attendanceStatusData.GetAttendanceStatusByIdAsync(statusId);

            if (attendance == null)
                throw new KeyNotFoundException($"Attendance status with ID {statusId} does not exist.");

            return attendance;
        }

        public async Task<AttendanceStatusResponse> GetAttendanceStatusByNameAsync(string statusName)
        {
            statusName = ValidationHelper.ValidateString(statusName, nameof(statusName), MinStatusNameLength, MaxStatusNameLength);

            AttendanceStatusResponse? attendance = await _attendanceStatusData.GetAttendanceStatusByNameAsync(statusName);

            if (attendance == null)
                throw new KeyNotFoundException($"Attendance status with name '{statusName}' does not exist.");

            return attendance;
        }

        public async Task<int> AddAttendanceStatusAsync(AttendanceStatusRequest status)
        {
            ValidateStatus(status);
            await EnsureHelper.EnsureUniqueAsync(_attendanceStatusData.GetAttendanceStatusByNameAsync, status.StatusName);

            int newStatusId = await _attendanceStatusData.AddAttendanceStatusAsync(status);

            if (newStatusId <= 0)
                throw new InvalidOperationException("Failed to add the attendance status.");

            return newStatusId;
        }

        public async Task<bool> UpdateAttendanceStatusAsync(int statusID, AttendanceStatusRequest status)
        {
            ValidateStatus(status);
            ValidationHelper.ValidateId(statusID);

            await EnsureHelper.EnsureExistsAsync(_attendanceStatusData.IsAttendanceStatusExistAsync, statusID, "Attendance Status");
            await EnsureHelper.EnsureUniqueAsync(_attendanceStatusData.GetAttendanceStatusByNameAsync, status.StatusName, s => s.StatusID, statusID);

            bool isUpdated = await _attendanceStatusData.UpdateAttendanceStatusAsync(statusID, status);

            if (!isUpdated)
                throw new InvalidOperationException($"Failed to update attendance status with ID {statusID}.");

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