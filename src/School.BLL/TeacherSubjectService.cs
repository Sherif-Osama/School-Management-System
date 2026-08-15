using School.BLL.Common;
using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.AssociationsDTOs.TeacherSubjectDTOs.Requests;
using School.DTO.AssociationsDTOs.TeacherSubjectDTOs.Responses;

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

        private static void ValidateRelation(TeacherSubjectRequest relation)
        {
            ArgumentNullException.ThrowIfNull(relation);

            ValidationHelper.ValidateId(relation.TeacherID);
            ValidationHelper.ValidateId(relation.SubjectID);
        }
        #endregion

        #region Ensure
        private async Task EnsureRelationExistsAsync(TeacherSubjectRequest relation)
        {
            if (!await _teacherSubjectData.IsTeacherSubjectExistAsync(relation))
                throw new KeyNotFoundException("The teacher is not assigned to this subject.");
        }

        private async Task EnsureRelationDoesNotExistAsync(TeacherSubjectRequest relation)
        {
            if (await _teacherSubjectData.IsTeacherSubjectExistAsync(relation))
                throw new InvalidOperationException("This subject is already assigned to the teacher.");
        }
        #endregion

        #region Public

        public async Task<List<TeacherSubjectResponse>> GetAllTeacherSubjectsAsync()
        {
            return await _teacherSubjectData.GetAllTeacherSubjectsAsync();
        }

        public async Task<List<TeacherSubjectResponse>> GetSubjectsByTeacherIdAsync(int teacherId)
        {
            ValidationHelper.ValidateId(teacherId);

            await EnsureHelper.EnsureExistsAsync(_teacherData.IsTeacherExistAsync, teacherId, "Teacher");

            return await _teacherSubjectData.GetSubjectsByTeacherIdAsync(teacherId);
        }

        public async Task<List<TeacherSubjectResponse>> GetTeachersBySubjectIdAsync(int subjectId)
        {
            ValidationHelper.ValidateId(subjectId);

            await EnsureHelper.EnsureExistsAsync(_subjectData.IsSubjectExistAsync, subjectId, "Subject");

            return await _teacherSubjectData.GetTeachersBySubjectIdAsync(subjectId);
        }

        public async Task<bool> AssignSubjectToTeacherAsync(TeacherSubjectRequest relation)
        {
            ValidateRelation(relation);

            await EnsureHelper.EnsureExistsAsync(_teacherData.IsTeacherExistAsync, relation.TeacherID, "Teacher");
            await EnsureHelper.EnsureExistsAsync(_subjectData.IsSubjectExistAsync, relation.SubjectID, "Subject");
            await EnsureRelationDoesNotExistAsync(relation);

            bool isAssigned = await _teacherSubjectData.AssignSubjectToTeacherAsync(relation);

            if (!isAssigned)
                throw new InvalidOperationException("Failed to assign the subject to the teacher.");

            return isAssigned;
        }

        public async Task<bool> RemoveSubjectFromTeacherAsync(TeacherSubjectRequest relation)
        {
            ValidateRelation(relation);

            await EnsureHelper.EnsureExistsAsync(_teacherData.IsTeacherExistAsync, relation.TeacherID, "Teacher");
            await EnsureHelper.EnsureExistsAsync(_subjectData.IsSubjectExistAsync, relation.SubjectID, "Subject");
            await EnsureRelationExistsAsync(relation);

            bool isRemoved = await _teacherSubjectData.RemoveSubjectFromTeacherAsync(relation);

            if (!isRemoved)
                throw new InvalidOperationException("Failed to remove the subject from the teacher.");

            return isRemoved;
        }
        #endregion
    }
}