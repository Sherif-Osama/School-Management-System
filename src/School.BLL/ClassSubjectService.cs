using School.BLL.Common;
using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.AssociationsDTOs.ClassSubjectDTOs.Requests;
using School.DTO.AssociationsDTOs.ClassSubjectDTOs.Responses;
using School.DTO.AssociationsDTOs.TeacherSubjectDTOs.Requests;

namespace School.BLL
{
    public class ClassSubjectService : IClassSubjectService
    {
        private readonly IClassSubjectData _classSubjectData;
        private readonly IClassData _classData;
        private readonly ITeacherData _teacherData;
        private readonly ISubjectData _subjectData;
        private readonly ITeacherSubjectData _teacherSubjectData;

        public ClassSubjectService(IClassSubjectData classSubjectData, IClassData classData, ITeacherData teacherData, ISubjectData subjectData, ITeacherSubjectData teacherSubjectData)
        {
            _classSubjectData = classSubjectData;
            _classData = classData;
            _teacherData = teacherData;
            _subjectData = subjectData;
            _teacherSubjectData = teacherSubjectData;
        }

        #region Private Helpers

        private static void ValidateClassSubject(CreateClassSubjectRequest classSubject)
        {
            ArgumentNullException.ThrowIfNull(classSubject);

            ValidationHelper.ValidateId(classSubject.ClassID);
            ValidationHelper.ValidateId(classSubject.TeacherID);
            ValidationHelper.ValidateId(classSubject.SubjectID);
        }
        private static void ValidateClassSubject(UpdateClassSubjectRequest classSubject)
        {
            ArgumentNullException.ThrowIfNull(classSubject);

            ValidationHelper.ValidateId(classSubject.TeacherID);
        }

        private async Task EnsureTeacherCanTeachSubjectAsync(int teacherId, int subjectId)
        {
            bool exists =
                await _teacherSubjectData.IsTeacherSubjectExistAsync(
                    new TeacherSubjectRequest
                    {
                        TeacherID = teacherId,
                        SubjectID = subjectId
                    });

            if (!exists)
                throw new KeyNotFoundException("This teacher is not assigned to teach this subject.");
        }

        // This uniqueness check is intentionally kept separate from EnsureHelper
        // because ClassSubject uniqueness is based on a composite key
        // (ClassID, TeacherID, SubjectID), while EnsureHelper currently handles
        // single-key lookups.
        // Future improvement: update EnsureHelper to support lookup delegates
        // that accept multiple parameters!!!
        private async Task EnsureUniqueClassSubjectAsync(CreateClassSubjectRequest classSubject, int? currentClassSubjectId = null)
        {
            ClassSubjectResponse? relation = await _classSubjectData.GetClassSubjectByDetailsAsync(classSubject.ClassID, classSubject.TeacherID, classSubject.SubjectID);

            if (relation == null)
                return;

            if (currentClassSubjectId.HasValue && relation.ClassSubjectID == currentClassSubjectId.Value)
                return;

            throw new InvalidOperationException(
                "This class, teacher and subject assignment already exists.");
        }

        private async Task<ClassSubjectResponse> EnsureClassSubjectExistsAsync(int classSubjectId)
        {
            var classSubject = await _classSubjectData.GetClassSubjectByIdAsync(classSubjectId);

            if (classSubject == null)
                throw new KeyNotFoundException($"ClassSubject with ID {classSubjectId} does not exist.");

            return classSubject;
        }
        #endregion

        #region Public Methods
        public Task<List<ClassSubjectResponse>> GetAllClassSubjectsAsync() => _classSubjectData.GetAllClassSubjectsAsync();

        public async Task<ClassSubjectResponse> GetClassSubjectByIdAsync(int classSubjectId)
        {
            ValidationHelper.ValidateId(classSubjectId);

            ClassSubjectResponse? classSubject = await _classSubjectData.GetClassSubjectByIdAsync(classSubjectId);

            if (classSubject == null)
                throw new KeyNotFoundException($"ClassSubject with ID {classSubjectId} does not exist.");

            return classSubject;
        }

        public async Task<List<ClassSubjectResponse>> GetClassSubjectsByClassIdAsync(int classId)
        {
            ValidationHelper.ValidateId(classId);

            await EnsureHelper.EnsureExistsAsync(_classData.IsClassExistAsync, classId, "Class");

            return await _classSubjectData.GetClassSubjectsByClassIdAsync(classId);
        }

        public async Task<List<ClassSubjectResponse>> GetClassSubjectsByTeacherIdAsync(int teacherId)
        {
            ValidationHelper.ValidateId(teacherId);

            await EnsureHelper.EnsureExistsAsync(_teacherData.IsTeacherExistAsync, teacherId, "Teacher");

            return await _classSubjectData.GetClassSubjectsByTeacherIdAsync(teacherId);
        }

        public async Task<List<ClassSubjectResponse>> GetClassSubjectsBySubjectIdAsync(int subjectId)
        {
            ValidationHelper.ValidateId(subjectId);

            await EnsureHelper.EnsureExistsAsync(_subjectData.IsSubjectExistAsync, subjectId, "Subject");

            return await _classSubjectData.GetClassSubjectsBySubjectIdAsync(subjectId);
        }

        public async Task<int> AddClassSubjectAsync(CreateClassSubjectRequest classSubject)
        {
            ValidateClassSubject(classSubject);

            await EnsureHelper.EnsureExistsAsync(_classData.IsClassExistAsync, classSubject.ClassID, "Class");
            await EnsureHelper.EnsureExistsAsync(_teacherData.IsTeacherExistAsync, classSubject.TeacherID, "Teacher");
            await EnsureHelper.EnsureExistsAsync(_subjectData.IsSubjectExistAsync, classSubject.SubjectID, "Subject");
            await EnsureTeacherCanTeachSubjectAsync(classSubject.TeacherID, classSubject.SubjectID);
            await EnsureUniqueClassSubjectAsync(classSubject);

            int newClassSubjectId = await _classSubjectData.AddClassSubjectAsync(classSubject);

            if (newClassSubjectId <= 0)
                throw new InvalidOperationException("Failed to add class subject.");

            return newClassSubjectId;
        }

        public async Task<bool> UpdateClassSubjectAsync(int classsubjectID, UpdateClassSubjectRequest classSubject)
        {
            ValidateClassSubject(classSubject);

            ValidationHelper.ValidateId(classsubjectID);

            var CurrentClassSubject = await EnsureClassSubjectExistsAsync(classsubjectID);

            await EnsureHelper.EnsureExistsAsync(_teacherData.IsTeacherExistAsync, classSubject.TeacherID, "Teacher");

            await EnsureTeacherCanTeachSubjectAsync(classSubject.TeacherID, CurrentClassSubject.SubjectID);

            bool isUpdated = await _classSubjectData.UpdateClassSubjectAsync(classsubjectID, classSubject);

            if (!isUpdated)
                throw new InvalidOperationException($"Failed to update class subject with ID {classsubjectID}.");

            return isUpdated;
        }

        public async Task<bool> DeleteClassSubjectAsync(int classSubjectId)
        {
            ValidationHelper.ValidateId(classSubjectId);

            await EnsureHelper.EnsureExistsAsync(_classSubjectData.IsClassSubjectExistAsync, classSubjectId, "Class Subject");

            bool isDeleted = await _classSubjectData.DeleteClassSubjectAsync(classSubjectId);

            if (!isDeleted)
                throw new InvalidOperationException($"Failed to delete class subject with ID {classSubjectId}.");

            return isDeleted;
        }

        public async Task<bool> IsClassSubjectExistAsync(int classSubjectId)
        {
            ValidationHelper.ValidateId(classSubjectId);

            return await _classSubjectData.IsClassSubjectExistAsync(classSubjectId);
        }

        #endregion
    }
}