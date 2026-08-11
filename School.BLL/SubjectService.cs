using School.BLL.Common;
using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.SubjectDTOs.Requests;
using School.DTO.SubjectDTOs.Responses;

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

        private static void ValidateSubject(CreateSubjectRequest subject)
        {
            ArgumentNullException.ThrowIfNull(subject);

            subject.SubjectName = ValidationHelper.ValidateString(subject.SubjectName, nameof(subject.SubjectName), MinSubjectNameLength, MaxSubjectNameLength);
        }
        private static void ValidateSubject(UpdateSubjectRequest subject)
        {
            ArgumentNullException.ThrowIfNull(subject);
            subject.SubjectName = ValidationHelper.ValidateString(subject.SubjectName, nameof(subject.SubjectName), MinSubjectNameLength, MaxSubjectNameLength);
        }
        #endregion

        #region Public
        public async Task<List<SubjectResponse>> GetAllSubjectsAsync()
        {
            return await _subjectData.GetAllSubjectsAsync();
        }

        public async Task<SubjectResponse?> GetSubjectByIdAsync(int subjectId)
        {
            ValidationHelper.ValidateId(subjectId);

            SubjectResponse? subject = await _subjectData.GetSubjectByIdAsync(subjectId);

            if (subject == null)
                throw new KeyNotFoundException($"Subject with ID {subjectId} does not exist.");

            return subject;
        }

        public async Task<SubjectResponse?> GetSubjectByNameAsync(string subjectName)
        {
            subjectName = ValidationHelper.ValidateString(subjectName, nameof(subjectName), MinSubjectNameLength, MaxSubjectNameLength);

            SubjectResponse? subject = await _subjectData.GetSubjectByNameAsync(subjectName);

            if (subject == null)
                throw new KeyNotFoundException($"Subject '{subjectName}' does not exist.");

            return subject;
        }

        public async Task<int> AddSubjectAsync(CreateSubjectRequest subject)
        {
            ValidateSubject(subject);

            await EnsureHelper.EnsureUniqueAsync(_subjectData.GetSubjectByNameAsync, subject.SubjectName);

            int newSubjectId = await _subjectData.AddSubjectAsync(subject);

            if (newSubjectId <= 0)
                throw new InvalidOperationException("Failed to add subject.");

            return newSubjectId;
        }

        public async Task<bool> UpdateSubjectAsync(int subjectId, UpdateSubjectRequest subject)
        {
            ValidateSubject(subject);
            ValidationHelper.ValidateId(subjectId);

            await EnsureHelper.EnsureExistsAsync(_subjectData.IsSubjectExistAsync, subjectId, "Subject");
            await EnsureHelper.EnsureUniqueAsync(_subjectData.GetSubjectByNameAsync, subject.SubjectName, s => s.SubjectID, subjectId);

            bool isUpdated = await _subjectData.UpdateSubjectAsync(subjectId, subject);

            if (!isUpdated)
                throw new InvalidOperationException($"Failed to update subject with ID {subjectId}.");

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