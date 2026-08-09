using School.BLL.Common;
using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.SubjectDTOs;

namespace School.BLL
{
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectData _subjectData;
        private static int MaxSubjectNameLength => 100;
        private static int MinSubjectNameLength => 3;
        public SubjectService(ISubjectData subjectData)
        {
            _subjectData = subjectData;
        }

        #region Validation

        private static void ValidateSubject(SubjectDTO subject)
        {
            ArgumentNullException.ThrowIfNull(subject);

            subject.SubjectName = ValidationHelper.ValidateString(subject.SubjectName, nameof(subject.SubjectName), MinSubjectNameLength, MaxSubjectNameLength);
        }
        #endregion

        #region Public
        public async Task<List<SubjectDTO>> GetAllSubjectsAsync()
        {
            return await _subjectData.GetAllSubjectsAsync();
        }

        public async Task<SubjectDTO?> GetSubjectByIdAsync(int subjectId)
        {
            ValidationHelper.ValidateId(subjectId);

            SubjectDTO? subject = await _subjectData.GetSubjectByIdAsync(subjectId);

            if (subject == null)
                throw new KeyNotFoundException($"Subject with ID {subjectId} does not exist.");

            return subject;
        }

        public async Task<SubjectDTO?> GetSubjectByNameAsync(string subjectName)
        {
            subjectName = ValidationHelper.ValidateString(subjectName, nameof(subjectName), MinSubjectNameLength, MaxSubjectNameLength);

            SubjectDTO? subject = await _subjectData.GetSubjectByNameAsync(subjectName);

            if (subject == null)
                throw new KeyNotFoundException($"Subject '{subjectName}' does not exist.");

            return subject;
        }

        public async Task<int> AddSubjectAsync(SubjectDTO subject)
        {
            ValidateSubject(subject);

            await EnsureHelper.EnsureUniqueAsync(_subjectData.GetSubjectByNameAsync, subject.SubjectName);

            int newSubjectId = await _subjectData.AddSubjectAsync(subject);

            if (newSubjectId <= 0)
                throw new InvalidOperationException("Failed to add subject.");

            return newSubjectId;
        }

        public async Task<bool> UpdateSubjectAsync(SubjectDTO subject)
        {
            ValidateSubject(subject);
            ValidationHelper.ValidateId(subject.SubjectID);

            await EnsureHelper.EnsureExistsAsync(_subjectData.IsSubjectExistAsync, subject.SubjectID, "Subject");
            await EnsureHelper.EnsureUniqueAsync(_subjectData.GetSubjectByNameAsync, subject.SubjectName, s => s.SubjectID, subject.SubjectID);

            bool isUpdated = await _subjectData.UpdateSubjectAsync(subject);

            if (!isUpdated)
                throw new InvalidOperationException($"Failed to update subject with ID {subject.SubjectID}.");

            return isUpdated;
        }

        public async Task<bool> DeleteSubjectAsync(int subjectId)
        {
            ValidationHelper.ValidateId(subjectId);

            await EnsureHelper.EnsureExistsAsync(_subjectData.IsSubjectExistAsync, subjectId, "Subject");

            bool isDeleted = await _subjectData.DeleteSubjectAsync(subjectId);

            if (!isDeleted)
                throw new InvalidOperationException($"Failed to delete subject with ID {subjectId}.");

            return isDeleted;
        }

        public async Task<bool> IsSubjectExistAsync(int subjectId)
        {
            ValidationHelper.ValidateId(subjectId);

            return await _subjectData.IsSubjectExistAsync(subjectId);
        }

        #endregion
    }
}