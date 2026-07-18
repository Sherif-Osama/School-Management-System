using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.AssociationsDTOs.TeacherSubjectDTOs;

namespace School.BLL
{
    public class TeacherSubjectService : ITeacherSubjectService
    {
        private readonly ITeacherSubjectData _teacherSubjectData;
        private readonly ITeacherData _teacherData;
        private readonly ISubjectData _subjectData;

        public TeacherSubjectService(
            ITeacherSubjectData teacherSubjectData,
            ITeacherData teacherData,
            ISubjectData subjectData)
        {
            _teacherSubjectData = teacherSubjectData;
            _teacherData = teacherData;
            _subjectData = subjectData;
        }

        #region Validation

        private static void ValidateRelation(TeacherSubjectDTO relation)
        {
            ArgumentNullException.ThrowIfNull(relation);

            ValidateTeacherId(relation.TeacherID);
            ValidateSubjectId(relation.SubjectID);
        }

        private static void ValidateTeacherId(int teacherId)
        {
            if (teacherId <= 0)
                throw new ArgumentException("Teacher ID must be greater than zero.", nameof(teacherId));
        }

        private static void ValidateSubjectId(int subjectId)
        {
            if (subjectId <= 0)
                throw new ArgumentException("Subject ID must be greater than zero.", nameof(subjectId));
        }
        #endregion

        #region Ensure

        private async Task EnsureTeacherExistsAsync(int teacherId)
        {
            if (!await _teacherData.IsTeacherExistAsync(teacherId))
                throw new KeyNotFoundException($"Teacher with ID {teacherId} does not exist.");
        }

        private async Task EnsureSubjectExistsAsync(int subjectId)
        {
            if (!await _subjectData.IsSubjectExistAsync(subjectId))
                throw new KeyNotFoundException($"Subject with ID {subjectId} does not exist.");
        }

        private async Task EnsureRelationExistsAsync(TeacherSubjectDTO relation)
        {
            if (!await _teacherSubjectData.IsTeacherSubjectExistAsync(relation))
                throw new KeyNotFoundException("The teacher is not assigned to this subject.");
        }

        private async Task EnsureRelationDoesNotExistAsync(TeacherSubjectDTO relation)
        {
            if (await _teacherSubjectData.IsTeacherSubjectExistAsync(relation))
                throw new InvalidOperationException("This subject is already assigned to the teacher.");
        }
        #endregion

        #region Public

        public async Task<List<TeacherSubjectDetailsDTO>> GetAllTeacherSubjectsAsync()
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

            bool isAssigned = await _teacherSubjectData.AssignSubjectToTeacherAsync(relation);

            if (!isAssigned)
                throw new InvalidOperationException("Failed to assign the subject to the teacher.");

            return isAssigned;
        }

        public async Task<bool> RemoveSubjectFromTeacherAsync(TeacherSubjectDTO relation)
        {
            ValidateRelation(relation);

            await EnsureTeacherExistsAsync(relation.TeacherID);
            await EnsureSubjectExistsAsync(relation.SubjectID);
            await EnsureRelationExistsAsync(relation);

            bool isRemoved = await _teacherSubjectData.RemoveSubjectFromTeacherAsync(relation);

            if (!isRemoved)
                throw new InvalidOperationException("Failed to remove the subject from the teacher.");

            return isRemoved;
        }
        #endregion
    }
}