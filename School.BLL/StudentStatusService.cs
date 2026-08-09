using School.BLL.Common;
using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.StudentStatusDTOs;

namespace School.BLL
{
    public class StudentStatusService : IStudentStatusService
    {
        private readonly IStudentStatusData _studentStatusData;
        private static int MaxStatusNameLength => 20;
        private static int MinStatusNameLength => 3;
        public StudentStatusService(IStudentStatusData studentStatusData)
        {
            _studentStatusData = studentStatusData;
        }

        #region Validation
        private static void ValidateStatus(StudentStatusDTO status)
        {
            ArgumentNullException.ThrowIfNull(status);

            status.StatusName = ValidationHelper.ValidateString(status.StatusName, nameof(status.StatusName), MinStatusNameLength, MaxStatusNameLength);
        }
        #endregion

        #region Public
        public async Task<List<StudentStatusDTO>> GetAllStudentStatusesAsync()
        {
            return await _studentStatusData.GetAllStudentStatusesAsync();
        }

        public async Task<StudentStatusDTO?> GetStudentStatusByIdAsync(int statusId)
        {
            ValidationHelper.ValidateId(statusId);

            StudentStatusDTO? status = await _studentStatusData.GetStudentStatusByIdAsync(statusId);

            if (status == null)
                throw new KeyNotFoundException($"Student status with ID {statusId} does not exist.");

            return status;
        }

        public async Task<StudentStatusDTO?> GetStudentStatusByNameAsync(string statusName)
        {
            statusName = ValidationHelper.ValidateString(statusName, nameof(statusName), MinStatusNameLength, MaxStatusNameLength);

            StudentStatusDTO? status = await _studentStatusData.GetStudentStatusByNameAsync(statusName);

            if (status == null)
                throw new KeyNotFoundException($"Student status '{statusName}' does not exist.");

            return status;
        }

        public async Task<int> AddStudentStatusAsync(StudentStatusDTO status)
        {
            ValidateStatus(status);

            await EnsureHelper.EnsureUniqueAsync(_studentStatusData.GetStudentStatusByNameAsync, status.StatusName);

            int newStatusId = await _studentStatusData.AddStudentStatusAsync(status);

            if (newStatusId <= 0)
                throw new InvalidOperationException("Failed to add student status.");

            return newStatusId;
        }

        public async Task<bool> UpdateStudentStatusAsync(StudentStatusDTO status)
        {
            ValidateStatus(status);
            ValidationHelper.ValidateId(status.StatusID);

            await EnsureHelper.EnsureExistsAsync(_studentStatusData.IsStudentStatusExistAsync, status.StatusID, "Student Status");
            await EnsureHelper.EnsureUniqueAsync(_studentStatusData.GetStudentStatusByNameAsync, status.StatusName, s => s.StatusID, status.StatusID);

            bool isUpdated = await _studentStatusData.UpdateStudentStatusAsync(status);

            if (!isUpdated)
                throw new InvalidOperationException($"Failed to update student status with ID {status.StatusID}.");

            return isUpdated;
        }

        public async Task<bool> DeleteStudentStatusAsync(int statusId)
        {
            ValidationHelper.ValidateId(statusId);

            await EnsureHelper.EnsureExistsAsync(_studentStatusData.IsStudentStatusExistAsync, statusId, "Student Status");

            bool isDeleted = await _studentStatusData.DeleteStudentStatusAsync(statusId);

            if (!isDeleted)
                throw new InvalidOperationException($"Failed to delete student status with ID {statusId}.");

            return isDeleted;
        }
        #endregion
    }
}