using School.BLL.Common;
using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.GradesDTOs.Requests;
using School.DTO.GradesDTOs.Responses;

namespace School.BLL
{
    public class GradeService : IGradeService
    {
        private readonly IGradeData _gradeData;
        private static int MinGradeNameLength => 2;
        private static int MaxGradeNameLength => 50;
        public GradeService(IGradeData gradeData)
        {
            _gradeData = gradeData;
        }

        #region Private Helpers
        private static void ValidateGrade(CreateGradeRequest grade)
        {
            ArgumentNullException.ThrowIfNull(grade);

            grade.GradeName = ValidationHelper.ValidateString(grade.GradeName, nameof(grade.GradeName), MinGradeNameLength, MaxGradeNameLength);
        }
        private static void ValidateGrade(UpdateGradeRequest grade)
        {
            ArgumentNullException.ThrowIfNull(grade);

            grade.GradeName = ValidationHelper.ValidateString(grade.GradeName, nameof(grade.GradeName), MinGradeNameLength, MaxGradeNameLength);
        }

        #endregion

        #region Public Methods
        public async Task<List<GradeResponse>> GetAllGradesAsync()
        {
            return await _gradeData.GetAllGradesAsync();
        }

        public async Task<GradeResponse?> GetGradeByIdAsync(byte gradeId)
        {
            ValidationHelper.ValidateId(gradeId);

            GradeResponse? grade = await _gradeData.GetGradeByIdAsync(gradeId);

            if (grade == null)
                throw new KeyNotFoundException($"Grade with ID {gradeId} does not exist.");

            return grade;
        }

        public async Task<GradeResponse?> GetGradeByNameAsync(string gradeName)
        {
            gradeName = ValidationHelper.ValidateString(gradeName, nameof(gradeName), MinGradeNameLength, MaxGradeNameLength);

            GradeResponse? gradeDTO = await _gradeData.GetGradeByNameAsync(gradeName);

            if (gradeDTO == null)
                throw new KeyNotFoundException($"Grade with name {gradeName} does not exist.");

            return gradeDTO;
        }

        public async Task<int> AddGradeAsync(CreateGradeRequest grade)
        {
            ValidateGrade(grade);

            await EnsureHelper.EnsureUniqueAsync(_gradeData.GetGradeByNameAsync, grade.GradeName);
            int newGradeId = await _gradeData.AddGradeAsync(grade);

            if (newGradeId <= 0)
                throw new InvalidOperationException("Failed to add grade.");

            return newGradeId;
        }

        public async Task<bool> UpdateGradeAsync(byte gradeID, UpdateGradeRequest grade)
        {
            ValidateGrade(grade);

            ValidationHelper.ValidateId(gradeID);

            await EnsureHelper.EnsureExistsAsync(_gradeData.IsGradeExistAsync, gradeID, "Grade");

            await EnsureHelper.EnsureUniqueAsync(_gradeData.GetGradeByNameAsync, grade.GradeName, g => g.GradeID, gradeID);

            bool isUpdated = await _gradeData.UpdateGradeAsync(gradeID, grade);

            if (!isUpdated)
                throw new InvalidOperationException($"Failed to update grade with ID {gradeID}.");

            return isUpdated;
        }

        public async Task<bool> DeleteGradeAsync(byte gradeId)
        {
            ValidationHelper.ValidateId(gradeId);

            await EnsureHelper.EnsureExistsAsync(_gradeData.IsGradeExistAsync, gradeId, "Grade");

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