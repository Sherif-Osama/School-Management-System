using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.SubjectDTO;

namespace School.BLL
{
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectData _subjectData;

        public SubjectService(ISubjectData subjectData)
        {
            _subjectData = subjectData;
        }

        #region Validation

        private static void ValidateSubject(SubjectDTO subject)
        {
            ArgumentNullException.ThrowIfNull(subject);

            subject.SubjectName = ValidateSubjectName(subject.SubjectName);
        }

        private static void ValidateSubjectId(int subjectId)
        {
            if (subjectId <= 0)
                throw new ArgumentOutOfRangeException(nameof(subjectId), "Subject ID must be greater than zero.");
        }

        private static string ValidateSubjectName(string subjectName)
        {
            if (string.IsNullOrWhiteSpace(subjectName))
                throw new ArgumentException("Subject name is required.", nameof(subjectName));

            subjectName = subjectName.Trim();

            if (subjectName.Length > 100)
                throw new ArgumentException("Subject name cannot exceed 100 characters.", nameof(subjectName));

            return subjectName;
        }

        #endregion

        #region Ensure

        private async Task EnsureSubjectExistsAsync(int subjectId)
        {
            if (!await _subjectData.IsSubjectExistAsync(subjectId))
                throw new KeyNotFoundException($"Subject with ID {subjectId} does not exist.");
        }

        private async Task EnsureSubjectNameUniqueAsync(string subjectName, int? currentSubjectId = null)
        {
            SubjectDTO? subject = await _subjectData.GetSubjectByNameAsync(subjectName);

            if (subject == null)
                return;

            if (currentSubjectId.HasValue && subject.SubjectID == currentSubjectId.Value)
                return;

            throw new InvalidOperationException($"Subject '{subjectName}' already exists.");
        }

        #endregion

        #region Public

        public async Task<List<SubjectDTO>> GetAllSubjectsAsync()
        {
            return await _subjectData.GetAllSubjectsAsync();
        }

        public async Task<SubjectDTO?> GetSubjectByIdAsync(int subjectId)
        {
            ValidateSubjectId(subjectId);

            SubjectDTO? subject = await _subjectData.GetSubjectByIdAsync(subjectId);

            if (subject == null)
                throw new KeyNotFoundException($"Subject with ID {subjectId} does not exist.");

            return subject;
        }

        public async Task<SubjectDTO?> GetSubjectByNameAsync(string subjectName)
        {
            subjectName = ValidateSubjectName(subjectName);

            SubjectDTO? subject = await _subjectData.GetSubjectByNameAsync(subjectName);

            if (subject == null)
                throw new KeyNotFoundException($"Subject '{subjectName}' does not exist.");

            return subject;
        }

        public async Task<int> AddSubjectAsync(SubjectDTO subject)
        {
            ValidateSubject(subject);

            await EnsureSubjectNameUniqueAsync(subject.SubjectName);

            int newSubjectId = await _subjectData.AddSubjectAsync(subject);

            if (newSubjectId <= 0)
                throw new InvalidOperationException("Failed to add subject.");

            return newSubjectId;
        }

        public async Task<bool> UpdateSubjectAsync(SubjectDTO subject)
        {
            ValidateSubject(subject);
            ValidateSubjectId(subject.SubjectID);

            await EnsureSubjectExistsAsync(subject.SubjectID);
            await EnsureSubjectNameUniqueAsync(subject.SubjectName, subject.SubjectID);

            bool isUpdated = await _subjectData.UpdateSubjectAsync(subject);

            if (!isUpdated)
                throw new InvalidOperationException($"Failed to update subject with ID {subject.SubjectID}.");

            return isUpdated;
        }

        public async Task<bool> DeleteSubjectAsync(int subjectId)
        {
            ValidateSubjectId(subjectId);

            await EnsureSubjectExistsAsync(subjectId);

            bool isDeleted = await _subjectData.DeleteSubjectAsync(subjectId);

            if (!isDeleted)
                throw new InvalidOperationException($"Failed to delete subject with ID {subjectId}.");

            return isDeleted;
        }

        public async Task<bool> IsSubjectExistAsync(int subjectId)
        {
            ValidateSubjectId(subjectId);

            return await _subjectData.IsSubjectExistAsync(subjectId);
        }

        #endregion
    }
}