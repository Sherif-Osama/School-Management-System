using School.BLL.Common;
using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.GradesDTOs;

namespace School.BLL
{
    public class GradeService : IGradeService
    {
        private readonly IGradeData _gradeData;
        private static int minGradeNameLength => 2;
        private static int maxGradeNameLength => 50;
        public GradeService(IGradeData gradeData)
        {
            _gradeData = gradeData;
        }

        #region Private Helpers
        private static void ValidateGrade(GradeDTO grade)
        {
            ArgumentNullException.ThrowIfNull(grade);

            grade.GradeName = ValidationHelper.ValidateString(grade.GradeName, nameof(grade.GradeName), minGradeNameLength, maxGradeNameLength);
        }

        private async Task EnsureGradeExistsAsync(byte gradeId)
        {
            if (!await _gradeData.IsGradeExistAsync(gradeId))
                throw new KeyNotFoundException($"Grade with ID {gradeId} does not exist.");
        }

        private async Task EnsureGradeNameIsUniqueAsync(string gradeName, byte? currentGradeId = null)
        {
            GradeDTO? grade = await _gradeData.GetGradeByNameAsync(gradeName);

            if (grade == null)
                return;

            if (currentGradeId.HasValue && grade.GradeID == currentGradeId.Value)
                return;

            throw new InvalidOperationException($"Grade '{gradeName}' already exists.");
        }
        #endregion

        #region Public Methods
        public async Task<List<GradeDTO>> GetAllGradesAsync()
        {
            return await _gradeData.GetAllGradesAsync();
        }

        public async Task<GradeDTO?> GetGradeByIdAsync(byte gradeId)
        {
            ValidationHelper.ValidateId(gradeId);

            GradeDTO? grade = await _gradeData.GetGradeByIdAsync(gradeId);

            if (grade == null)
                throw new KeyNotFoundException($"Grade with ID {gradeId} does not exist.");

            return grade;
        }

        public async Task<GradeDTO?> GetGradeByNameAsync(string gradeName)
        {
            gradeName = ValidationHelper.ValidateString(gradeName, nameof(gradeName), minGradeNameLength, maxGradeNameLength);

            GradeDTO? gradeDTO = await _gradeData.GetGradeByNameAsync(gradeName);

            if (gradeDTO == null)
                throw new KeyNotFoundException($"Grade with name {gradeName} does not exist.");

            return gradeDTO;
        }

        public async Task<int> AddGradeAsync(GradeDTO grade)
        {
            ValidateGrade(grade);

            await EnsureGradeNameIsUniqueAsync(grade.GradeName);
            int newGradeId = await _gradeData.AddGradeAsync(grade);

            if (newGradeId <= 0)
                throw new InvalidOperationException("Failed to add grade.");

            return newGradeId;
        }

        public async Task<bool> UpdateGradeAsync(GradeDTO grade)
        {
            ValidateGrade(grade);

            ValidationHelper.ValidateId(grade.GradeID);

            await EnsureGradeExistsAsync(grade.GradeID);

            await EnsureGradeNameIsUniqueAsync(grade.GradeName, grade.GradeID);

            bool isUpdated = await _gradeData.UpdateGradeAsync(grade);

            if (!isUpdated)
                throw new InvalidOperationException($"Failed to update grade with ID {grade.GradeID}.");

            return isUpdated;
        }

        public async Task<bool> DeleteGradeAsync(byte gradeId)
        {
            ValidationHelper.ValidateId(gradeId);

            await EnsureGradeExistsAsync(gradeId);

            bool isDeleted = await _gradeData.DeleteGradeAsync(gradeId);
            if (!isDeleted)
                throw new InvalidOperationException($"Failed to delete grade with ID {gradeId}.");

            return isDeleted;
        }

        public async Task<bool> IsGradeExistAsync(byte gradeId)
        {
            ValidationHelper.ValidateId(gradeId);

            return await _gradeData.IsGradeExistAsync(gradeId);
        }
        #endregion
    }
}