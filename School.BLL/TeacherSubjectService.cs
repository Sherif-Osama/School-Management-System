using School.DAL;
using School.DTO.AssociationsDTOs.TeacherSubjectDTOs;

namespace School.BLL
{
    public class TeacherSubjectService
    {
        private readonly TeacherSubjectData _teacherSubjectData;
        private readonly TeacherData _teacherData;
        private readonly SubjectData _subjectData;

        public TeacherSubjectService(TeacherSubjectData teacherSubjectData, TeacherData teacherData, SubjectData subjectData)
        {
            _teacherSubjectData = teacherSubjectData;
            _teacherData = teacherData;
            _subjectData = subjectData;
        }

        #region Private Helpers
        private static void ValidateRelation(TeacherSubjectDTO relation)
        {
            ArgumentNullException.ThrowIfNull(relation);

            ValidateTeacherId(relation.TeacherID);
            ValidateSubjectId(relation.SubjectID);
        }

        private static void ValidateTeacherId(int teacherId)
        {
            if (teacherId <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(teacherId),
                    "Teacher ID must be greater than zero.");
        }

        private static void ValidateSubjectId(int subjectId)
        {
            if (subjectId <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(subjectId),
                    "Subject ID must be greater than zero.");
        }

        private async Task EnsureTeacherExistsAsync(int teacherId)
        {
            if (!await _teacherData.IsTeacherExistAsync(teacherId))
                throw new InvalidOperationException(
                    $"Teacher with ID {teacherId} does not exist.");
        }

        private async Task EnsureSubjectExistsAsync(int subjectId)
        {
            if (!await _subjectData.IsSubjectExistAsync(subjectId))
                throw new InvalidOperationException(
                    $"Subject with ID {subjectId} does not exist.");
        }

        private async Task EnsureRelationExistsAsync(TeacherSubjectDTO relation)
        {
            if (!await _teacherSubjectData.IsTeacherSubjectExistAsync(relation))
                throw new InvalidOperationException(
                    "The teacher is not assigned to this subject.");
        }

        private async Task EnsureRelationDoesNotExistAsync(TeacherSubjectDTO relation)
        {
            if (await _teacherSubjectData.IsTeacherSubjectExistAsync(relation))
                throw new InvalidOperationException(
                    "This subject is already assigned to the teacher.");
        }

        #endregion

        #region Public Methods

        public async Task<List<TeacherSubjectDetailsDTO>>
            GetAllTeacherSubjectsAsync()
        {
            return await _teacherSubjectData.GetAllTeacherSubjectsAsync();
        }

        public async Task<List<TeacherSubjectDetailsDTO>> GetSubjectsByTeacherIdAsync(int teacherId)
        {
            ValidateTeacherId(teacherId);

            await EnsureTeacherExistsAsync(teacherId);

            return await _teacherSubjectData.GetSubjectsByTeacherIdAsync(teacherId);
        }

        public async Task<List<TeacherSubjectDetailsDTO>> GetTeachersBySubjectIdAsync(int subjectId)
        {
            ValidateSubjectId(subjectId);

            await EnsureSubjectExistsAsync(subjectId);

            return await _teacherSubjectData.GetTeachersBySubjectIdAsync(subjectId);
        }

        public async Task<bool> AssignSubjectToTeacherAsync(TeacherSubjectDTO relation)
        {
            ValidateRelation(relation);

            await EnsureTeacherExistsAsync(relation.TeacherID);

            await EnsureSubjectExistsAsync(relation.SubjectID);

            await EnsureRelationDoesNotExistAsync(relation);

            return await _teacherSubjectData.AssignSubjectToTeacherAsync(relation);
        }

        public async Task<bool> RemoveSubjectFromTeacherAsync(TeacherSubjectDTO relation)
        {
            ValidateRelation(relation);

            await EnsureTeacherExistsAsync(relation.TeacherID);

            await EnsureSubjectExistsAsync(relation.SubjectID);

            await EnsureRelationExistsAsync(relation);

            return await _teacherSubjectData
                .RemoveSubjectFromTeacherAsync(relation);
        }

        #endregion
    }
}