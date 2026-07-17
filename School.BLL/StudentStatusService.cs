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
                throw new ArgumentException("StatusID must be a positive number.", nameof(statusId));
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
                throw new InvalidOperationException($"StudentStatus with ID {statusId} does not exist.");
        }

        private async Task EnsureStatusNameUniqueAsync(string statusName, int? statusId = null)
        {
            StudentStatusDTO? existing = await _studentStatusData.GetStudentStatusByNameAsync(statusName);

            if (existing != null && (statusId == null || existing.StatusID != statusId.Value))
                throw new InvalidOperationException($"A status named '{statusName}' already exists.");
        }
        #endregion

        #region Public
        public Task<List<StudentStatusDTO>> GetAllStudentStatusesAsync()
        {
            return _studentStatusData.GetAllStudentStatusesAsync();
        }

        public Task<StudentStatusDTO?> GetStudentStatusByIdAsync(int statusId)
        {
            ValidateStatusId(statusId);

            return _studentStatusData.GetStudentStatusByIdAsync(statusId);
        }

        public Task<StudentStatusDTO?> GetStudentStatusByNameAsync(string statusName)
        {
            statusName = ValidateStatusName(statusName);

            return _studentStatusData.GetStudentStatusByNameAsync(statusName);
        }

        public async Task<int> AddStudentStatusAsync(StudentStatusDTO status)
        {
            ValidateStatus(status);

            await EnsureStatusNameUniqueAsync(status.StatusName);

            return await _studentStatusData.AddStudentStatusAsync(status);
        }

        public async Task<bool> UpdateStudentStatusAsync(StudentStatusDTO status)
        {
            ValidateStatus(status);
            ValidateStatusId(status.StatusID);

            await EnsureStatusExistsAsync(status.StatusID);
            await EnsureStatusNameUniqueAsync(status.StatusName, status.StatusID);

            return await _studentStatusData.UpdateStudentStatusAsync(status);
        }

        public async Task<bool> DeleteStudentStatusAsync(int statusId)
        {
            ValidateStatusId(statusId);

            await EnsureStatusExistsAsync(statusId);

            return await _studentStatusData.DeleteStudentStatusAsync(statusId);
        }
        #endregion
    }
}