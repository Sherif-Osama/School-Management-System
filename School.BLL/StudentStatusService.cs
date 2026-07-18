using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.StudentStatusDTOs;

namespace School.BLL
{
    public class StudentStatusService : IStudentStatusService
    {
        private readonly IStudentStatusData _studentStatusData;

        public StudentStatusService(IStudentStatusData studentStatusData)
        {
            _studentStatusData = studentStatusData;
        }

        #region Validation

        private static void ValidateStatus(StudentStatusDTO status)
        {
            ArgumentNullException.ThrowIfNull(status);

            ValidateStatusName(status.StatusName);
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

            if (statusName.Length > 20)
                throw new ArgumentException("StatusName cannot exceed 20 characters.", nameof(statusName));

            return statusName;
        }

        #endregion

        #region Ensure

        private async Task EnsureStatusExistsAsync(int statusId)
        {
            if (!await _studentStatusData.IsStudentStatusExistAsync(statusId))
                throw new KeyNotFoundException($"Student status with ID {statusId} does not exist.");
        }

        private async Task EnsureStatusNameUniqueAsync(string statusName, int? currentStatusId = null)
        {
            StudentStatusDTO? existing = await _studentStatusData.GetStudentStatusByNameAsync(statusName);

            if (existing == null)
                return;

            if (currentStatusId.HasValue && existing.StatusID == currentStatusId.Value)
                return;

            throw new InvalidOperationException($"Student status '{statusName}' already exists.");
        }

        #endregion

        #region Public

        public async Task<List<StudentStatusDTO>> GetAllStudentStatusesAsync()
        {
            return await _studentStatusData.GetAllStudentStatusesAsync();
        }

        public async Task<StudentStatusDTO?> GetStudentStatusByIdAsync(int statusId)
        {
            ValidateStatusId(statusId);

            StudentStatusDTO? status = await _studentStatusData.GetStudentStatusByIdAsync(statusId);

            if (status == null)
                throw new KeyNotFoundException($"Student status with ID {statusId} does not exist.");

            return status;
        }

        public async Task<StudentStatusDTO?> GetStudentStatusByNameAsync(string statusName)
        {
            statusName = ValidateStatusName(statusName);

            StudentStatusDTO? status = await _studentStatusData.GetStudentStatusByNameAsync(statusName);

            if (status == null)
                throw new KeyNotFoundException($"Student status '{statusName}' does not exist.");

            return status;
        }

        public async Task<int> AddStudentStatusAsync(StudentStatusDTO status)
        {
            ValidateStatus(status);

            await EnsureStatusNameUniqueAsync(status.StatusName);

            int newStatusId = await _studentStatusData.AddStudentStatusAsync(status);

            if (newStatusId <= 0)
                throw new InvalidOperationException("Failed to add student status.");

            return newStatusId;
        }

        public async Task<bool> UpdateStudentStatusAsync(StudentStatusDTO status)
        {
            ValidateStatus(status);
            ValidateStatusId(status.StatusID);

            await EnsureStatusExistsAsync(status.StatusID);
            await EnsureStatusNameUniqueAsync(status.StatusName, status.StatusID);

            bool isUpdated = await _studentStatusData.UpdateStudentStatusAsync(status);

            if (!isUpdated)
                throw new InvalidOperationException($"Failed to update student status with ID {status.StatusID}.");

            return isUpdated;
        }

        public async Task<bool> DeleteStudentStatusAsync(int statusId)
        {
            ValidateStatusId(statusId);

            await EnsureStatusExistsAsync(statusId);

            bool isDeleted = await _studentStatusData.DeleteStudentStatusAsync(statusId);

            if (!isDeleted)
                throw new InvalidOperationException($"Failed to delete student status with ID {statusId}.");

            return isDeleted;
        }
        #endregion
    }
}