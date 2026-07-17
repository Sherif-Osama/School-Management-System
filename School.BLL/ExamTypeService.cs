using School.DAL;
using School.DTO.ExamTypeDTOs;

namespace School.BLL
{
    public class ExamTypeService
    {
        private readonly ExamTypeData _examTypeData;

        public ExamTypeService(ExamTypeData examTypeData)
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
                throw new ArgumentException("Exam name must be between 3 and 50 characters.", nameof(examName));

            return examName;
        }

        #endregion

        #region Ensure

        private async Task EnsureExamTypeExistsAsync(int examTypeId)
        {
            if (!await _examTypeData.IsExamTypeExistAsync(examTypeId))
                throw new InvalidOperationException($"ExamType with ID {examTypeId} does not exist.");
        }

        private async Task EnsureExamTypeNameUniqueAsync(string examName, int? currentExamTypeId = null)
        {
            ExamTypeDTO? examType = await _examTypeData.GetExamTypeByNameAsync(examName);

            if (examType == null)
                return;

            if (currentExamTypeId.HasValue &&
                examType.ExamTypeID == currentExamTypeId.Value)
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

            return await _examTypeData.GetExamTypeByIdAsync(examTypeId);
        }

        public async Task<ExamTypeDTO?> GetExamTypeByNameAsync(string examName)
        {
            examName = ValidateExamTypeName(examName);

            return await _examTypeData.GetExamTypeByNameAsync(examName);
        }

        public async Task<int> AddExamTypeAsync(ExamTypeDTO examType)
        {
            ValidateExamType(examType);

            await EnsureExamTypeNameUniqueAsync(examType.ExamName);

            return await _examTypeData.AddExamTypeAsync(examType);
        }

        public async Task<bool> UpdateExamTypeAsync(ExamTypeDTO examType)
        {
            ValidateExamType(examType);
            ValidateExamTypeId(examType.ExamTypeID);

            await EnsureExamTypeExistsAsync(examType.ExamTypeID);
            await EnsureExamTypeNameUniqueAsync(examType.ExamName, examType.ExamTypeID);

            return await _examTypeData.UpdateExamTypeAsync(examType);
        }

        public async Task<bool> DeleteExamTypeAsync(int examTypeId)
        {
            ValidateExamTypeId(examTypeId);

            await EnsureExamTypeExistsAsync(examTypeId);

            return await _examTypeData.DeleteExamTypeAsync(examTypeId);
        }

        #endregion
    }
}