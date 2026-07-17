using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.GradesDTOs;

namespace School.BLL
{
    public class GradeService : IGradeService
    {
        private readonly IGradeData _gradeData;

        public GradeService(IGradeData gradeData)
        {
            _gradeData = gradeData;
        }

        #region Private Helpers
        private static void ValidateGrade(GradeDTO grade)
        {
            ArgumentNullException.ThrowIfNull(grade);

            grade.GradeName = ValidateGradeName(grade.GradeName);
        }

        private static void ValidateGradeId(byte gradeId)
        {
            if (gradeId <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(gradeId),
                    "Grade ID must be greater than zero.");
        }

        private static string ValidateGradeName(string gradeName)
        {
            if (string.IsNullOrWhiteSpace(gradeName))
                throw new ArgumentException(
                    "Grade name is required.",
                    nameof(gradeName));

            gradeName = gradeName.Trim();

            if (gradeName.Length > 50)
                throw new ArgumentException(
                    "Grade name cannot exceed 50 characters.",
                    nameof(gradeName));

            return gradeName;
        }

        private async Task EnsureGradeExistsAsync(byte gradeId)
        {
            if (!await _gradeData.IsGradeExistAsync(gradeId))
                throw new InvalidOperationException(
                    $"Grade with ID {gradeId} does not exist.");
        }

        private async Task EnsureGradeNameIsUniqueAsync(
            string gradeName,
            byte? currentGradeId = null)
        {
            GradeDTO? grade = await _gradeData.GetGradeByNameAsync(gradeName);

            if (grade == null)
                return;

            if (currentGradeId.HasValue &&
                grade.GradeID == currentGradeId.Value)
                return;

            throw new InvalidOperationException(
                $"Grade '{gradeName}' already exists.");
        }

        #endregion

        #region Public Methods

        public async Task<List<GradeDTO>> GetAllGradesAsync()
        {
            return await _gradeData.GetAllGradesAsync();
        }

        public async Task<GradeDTO?> GetGradeByIdAsync(byte gradeId)
        {
            ValidateGradeId(gradeId);

            return await _gradeData.GetGradeByIdAsync(gradeId);
        }

        public async Task<GradeDTO?> GetGradeByNameAsync(string gradeName)
        {
            gradeName = ValidateGradeName(gradeName);

            return await _gradeData.GetGradeByNameAsync(gradeName);
        }

        public async Task<int> AddGradeAsync(GradeDTO grade)
        {
            ValidateGrade(grade);

            await EnsureGradeNameIsUniqueAsync(grade.GradeName);

            return await _gradeData.AddGradeAsync(grade);
        }

        public async Task<bool> UpdateGradeAsync(GradeDTO grade)
        {
            ValidateGrade(grade);

            ValidateGradeId(grade.GradeID);

            await EnsureGradeExistsAsync(grade.GradeID);

            await EnsureGradeNameIsUniqueAsync(
                grade.GradeName,
                grade.GradeID);

            return await _gradeData.UpdateGradeAsync(grade);
        }

        public async Task<bool> DeleteGradeAsync(byte gradeId)
        {
            ValidateGradeId(gradeId);

            await EnsureGradeExistsAsync(gradeId);

            return await _gradeData.DeleteGradeAsync(gradeId);
        }

        public async Task<bool> IsGradeExistAsync(byte gradeId)
        {
            ValidateGradeId(gradeId);

            return await _gradeData.IsGradeExistAsync(gradeId);
        }
        #endregion
    }
}