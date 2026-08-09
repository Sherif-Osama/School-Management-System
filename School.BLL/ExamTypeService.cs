using School.BLL.Common;
using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.ExamTypeDTOs;

namespace School.BLL
{
    public class ExamTypeService : IExamTypeService
    {
        private readonly IExamTypeData _examTypeData;
        private static int minExamNameLength => 3;
        private static int maxExamNameLength => 50;
        public ExamTypeService(IExamTypeData examTypeData)
        {
            _examTypeData = examTypeData;
        }

        #region Validation

        private static void ValidateExamType(ExamTypeDTO examType)
        {
            ArgumentNullException.ThrowIfNull(examType);

            examType.ExamName = ValidationHelper.ValidateString(examType.ExamName, nameof(examType.ExamName), minExamNameLength, maxExamNameLength);
        }
        #endregion

        #region Ensure
        private async Task EnsureExamTypeExistsAsync(int examTypeId)
        {
            if (!await _examTypeData.IsExamTypeExistAsync(examTypeId))
                throw new KeyNotFoundException($"Exam type with ID '{examTypeId}' does not exist.");
        }

        private async Task EnsureExamTypeNameUniqueAsync(string examName, int? currentExamTypeId = null)
        {
            ExamTypeDTO? examType = await _examTypeData.GetExamTypeByNameAsync(examName);

            if (examType == null)
                return;

            if (currentExamTypeId.HasValue && examType.ExamTypeID == currentExamTypeId.Value)
                return;

            throw new InvalidOperationException("Exam name already exists.");
        }
        #endregion

        #region Public

        public Task<List<ExamTypeDTO>> GetAllExamTypesAsync()
        {
            return _examTypeData.GetAllExamTypesAsync();
        }

        public async Task<ExamTypeDTO?> GetExamTypeByIdAsync(int examTypeId)
        {
            ValidationHelper.ValidateId(examTypeId);

            ExamTypeDTO? examTypeDTO = await _examTypeData.GetExamTypeByIdAsync(examTypeId);

            if (examTypeDTO == null)
                throw new KeyNotFoundException($"Exam type with ID '{examTypeId}' does not exist.");

            return examTypeDTO;
        }

        public async Task<ExamTypeDTO?> GetExamTypeByNameAsync(string examName)
        {
            examName = ValidationHelper.ValidateString(examName, nameof(examName), minExamNameLength, maxExamNameLength);

            ExamTypeDTO? examTypeDTO = await _examTypeData.GetExamTypeByNameAsync(examName);

            if (examTypeDTO == null)
                throw new KeyNotFoundException($"Exam type with name '{examName}' does not exist.");

            return examTypeDTO;
        }

        public async Task<int> AddExamTypeAsync(ExamTypeDTO examType)
        {
            ValidateExamType(examType);

            await EnsureExamTypeNameUniqueAsync(examType.ExamName);

            int newExamTypeId = await _examTypeData.AddExamTypeAsync(examType);

            if (newExamTypeId <= 0)
                throw new InvalidOperationException("Failed to add exam type.");

            return newExamTypeId;
        }

        public async Task<bool> UpdateExamTypeAsync(ExamTypeDTO examType)
        {
            ValidateExamType(examType);
            ValidationHelper.ValidateId(examType.ExamTypeID);

            await EnsureExamTypeExistsAsync(examType.ExamTypeID);
            await EnsureExamTypeNameUniqueAsync(examType.ExamName, examType.ExamTypeID);

            bool isUpdated = await _examTypeData.UpdateExamTypeAsync(examType);

            if (!isUpdated)
                throw new InvalidOperationException($"Failed to update exam type with ID '{examType.ExamTypeID}'.");

            return isUpdated;
        }

        public async Task<bool> DeleteExamTypeAsync(int examTypeId)
        {
            ValidationHelper.ValidateId(examTypeId);

            await EnsureExamTypeExistsAsync(examTypeId);
            bool isDeleted = await _examTypeData.DeleteExamTypeAsync(examTypeId);

            if (!isDeleted)
                throw new InvalidOperationException($"Failed to delete exam type with ID '{examTypeId}'.");

            return isDeleted;
        }

        #endregion
    }
}