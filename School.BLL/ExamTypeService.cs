using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.ExamTypeDTOs;

namespace School.BLL
{
    public class ExamTypeService : IExamTypeService
    {
        private readonly IExamTypeData _examTypeData;

        public ExamTypeService(IExamTypeData examTypeData)
        {
            _examTypeData = examTypeData;
        }

        #region Validation

        private static void ValidateExamType(ExamTypeDTO examType)
        {
            ArgumentNullException.ThrowIfNull(examType);

            examType.ExamName = ValidateExamTypeName(examType.ExamName);
        }

        private static void ValidateExamTypeId(int examTypeId)
        {
            if (examTypeId <= 0)
                throw new ArgumentException("ExamTypeID must be a positive number.", nameof(examTypeId));

        }

        private static string ValidateExamTypeName(string examName)
        {
            if (string.IsNullOrWhiteSpace(examName))
                throw new ArgumentException("Exam name is required.", nameof(examName));

            examName = examName.Trim();

            if (examName.Length < 3 || examName.Length > 50)
                throw new ArgumentOutOfRangeException(nameof(examName), "Exam name must be between 3 and 50 characters.");

            return examName;
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
            ValidateExamTypeId(examTypeId);

            ExamTypeDTO? examTypeDTO = await _examTypeData.GetExamTypeByIdAsync(examTypeId);

            if (examTypeDTO == null)
                throw new KeyNotFoundException($"Exam type with ID '{examTypeId}' does not exist.");

            return examTypeDTO;
        }

        public async Task<ExamTypeDTO?> GetExamTypeByNameAsync(string examName)
        {
            examName = ValidateExamTypeName(examName);

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
            ValidateExamTypeId(examType.ExamTypeID);

            await EnsureExamTypeExistsAsync(examType.ExamTypeID);
            await EnsureExamTypeNameUniqueAsync(examType.ExamName, examType.ExamTypeID);

            bool isUpdated = await _examTypeData.UpdateExamTypeAsync(examType);

            if (!isUpdated)
                throw new InvalidOperationException($"Failed to update exam type with ID '{examType.ExamTypeID}'.");

            return isUpdated;
        }

        public async Task<bool> DeleteExamTypeAsync(int examTypeId)
        {
            ValidateExamTypeId(examTypeId);

            await EnsureExamTypeExistsAsync(examTypeId);
            bool isDeleted = await _examTypeData.DeleteExamTypeAsync(examTypeId);

            if (!isDeleted)
                throw new InvalidOperationException($"Failed to delete exam type with ID '{examTypeId}'.");

            return isDeleted;
        }

        #endregion
    }
}