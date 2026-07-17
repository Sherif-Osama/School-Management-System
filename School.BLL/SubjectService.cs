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

        #region Private Helpers

        private static void ValidateSubject(SubjectDTO subject)
        {
            ArgumentNullException.ThrowIfNull(subject);

            if (string.IsNullOrWhiteSpace(subject.SubjectName))
                throw new ArgumentException(
                    "Subject name is required.",
                    nameof(subject.SubjectName));

            subject.SubjectName = ValidateSubjectName(subject.SubjectName);
        }

        private static void ValidateSubjectId(int subjectId)
        {
            if (subjectId <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(subjectId),
                    "Subject ID must be greater than zero.");
        }

        private static string ValidateSubjectName(string subjectName)
        {
            if (string.IsNullOrWhiteSpace(subjectName))
                throw new ArgumentException(
                    "Subject name is required.",
                    nameof(subjectName));

            subjectName = subjectName.Trim();

            if (subjectName.Length > 100)
                throw new ArgumentException(
                    "Subject name cannot exceed 100 characters.",
                    nameof(subjectName));

            return subjectName;
        }

        private async Task EnsureSubjectExistsAsync(int subjectId)
        {
            if (!await _subjectData.IsSubjectExistAsync(subjectId))
                throw new InvalidOperationException(
                    $"Subject with ID {subjectId} does not exist.");
        }

        private async Task EnsureSubjectNameIsUniqueAsync(
            string subjectName,
            int? currentSubjectId = null)
        {
            SubjectDTO? subject =
                await _subjectData.GetSubjectByNameAsync(subjectName);

            if (subject == null)
                return;

            if (currentSubjectId.HasValue &&
                subject.SubjectID == currentSubjectId.Value)
                return;

            throw new InvalidOperationException(
                $"Subject '{subjectName}' already exists.");
        }

        #endregion

        #region Public Methods

        public async Task<List<SubjectDTO>> GetAllSubjectsAsync()
        {
            return await _subjectData.GetAllSubjectsAsync();
        }

        public async Task<SubjectDTO?> GetSubjectByIdAsync(int subjectId)
        {
            ValidateSubjectId(subjectId);

            return await _subjectData.GetSubjectByIdAsync(subjectId);
        }

        public async Task<SubjectDTO?> GetSubjectByNameAsync(string subjectName)
        {
            subjectName = ValidateSubjectName(subjectName);

            return await _subjectData.GetSubjectByNameAsync(subjectName);
        }

        public async Task<int> AddSubjectAsync(SubjectDTO subject)
        {
            ValidateSubject(subject);

            await EnsureSubjectNameIsUniqueAsync(subject.SubjectName);

            return await _subjectData.AddSubjectAsync(subject);
        }

        public async Task<bool> UpdateSubjectAsync(SubjectDTO subject)
        {
            ValidateSubject(subject);

            ValidateSubjectId(subject.SubjectID);

            await EnsureSubjectExistsAsync(subject.SubjectID);

            await EnsureSubjectNameIsUniqueAsync(
                subject.SubjectName,
                subject.SubjectID);

            return await _subjectData.UpdateSubjectAsync(subject);
        }

        public async Task<bool> DeleteSubjectAsync(int subjectId)
        {
            ValidateSubjectId(subjectId);

            await EnsureSubjectExistsAsync(subjectId);

            return await _subjectData.DeleteSubjectAsync(subjectId);
        }

        public async Task<bool> IsSubjectExistAsync(int subjectId)
        {
            ValidateSubjectId(subjectId);

            return await _subjectData.IsSubjectExistAsync(subjectId);
        }

        #endregion
    }
}