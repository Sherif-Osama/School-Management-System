using School.BLL.Common;
using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.AssociationsDTOs.ClassSubjectDTOs;
using School.DTO.AssociationsDTOs.TeacherSubjectDTOs;

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

        private static void ValidateClassSubject(ClassSubjectDTO classSubject)
        {
            ArgumentNullException.ThrowIfNull(classSubject);

            ValidationHelper.ValidateId(classSubject.ClassID);
            ValidationHelper.ValidateId(classSubject.TeacherID);
            ValidationHelper.ValidateId(classSubject.SubjectID);
        }

        private async Task EnsureClassExistsAsync(int classId)
        {
            if (!await _classData.IsClassExistAsync(classId))
                throw new KeyNotFoundException($"Class with ID {classId} does not exist.");
        }

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

        private async Task EnsureTeacherCanTeachSubjectAsync(int teacherId, int subjectId)
        {
            bool exists =
                await _teacherSubjectData.IsTeacherSubjectExistAsync(
                    new TeacherSubjectDTO
                    {
                        TeacherID = teacherId,
                        SubjectID = subjectId
                    });

            if (!exists)
                throw new KeyNotFoundException("This teacher is not assigned to teach this subject.");
        }

        private async Task EnsureClassSubjectExistsAsync(int classSubjectId)
        {
            if (!await _classSubjectData.IsClassSubjectExistAsync(classSubjectId))
                throw new KeyNotFoundException($"ClassSubject with ID {classSubjectId} does not exist.");
        }

        private async Task EnsureUniqueClassSubjectAsync(ClassSubjectDTO classSubject, int? currentClassSubjectId = null)
        {
            ClassSubjectDetailsDTO? relation = await _classSubjectData.GetClassSubjectByDetailsAsync(classSubject.ClassID, classSubject.TeacherID, classSubject.SubjectID);

            if (relation == null)
                return;

            if (currentClassSubjectId.HasValue && relation.ClassSubjectID == currentClassSubjectId.Value)
                return;

            throw new InvalidOperationException(
                "This class, teacher and subject assignment already exists.");
        }

        #endregion

        #region Public Methods

        public async Task<List<ClassSubjectDetailsDTO>> GetAllClassSubjectsAsync()
        {
            return await _classSubjectData.GetAllClassSubjectsAsync();
        }

        public async Task<ClassSubjectDetailsDTO?> GetClassSubjectByIdAsync(int classSubjectId)
        {
            ValidationHelper.ValidateId(classSubjectId);

            ClassSubjectDetailsDTO? classSubject = await _classSubjectData.GetClassSubjectByIdAsync(classSubjectId);

            if (classSubject == null)
                throw new KeyNotFoundException($"ClassSubject with ID {classSubjectId} does not exist.");

            return classSubject;
        }

        public async Task<List<ClassSubjectDetailsDTO>> GetClassSubjectsByClassIdAsync(int classId)
        {
            ValidationHelper.ValidateId(classId);

            await EnsureClassExistsAsync(classId);

            return await _classSubjectData.GetClassSubjectsByClassIdAsync(classId);
        }

        public async Task<List<ClassSubjectDetailsDTO>> GetClassSubjectsByTeacherIdAsync(int teacherId)
        {
            ValidationHelper.ValidateId(teacherId);

            await EnsureTeacherExistsAsync(teacherId);

            return await _classSubjectData.GetClassSubjectsByTeacherIdAsync(teacherId);
        }

        public async Task<List<ClassSubjectDetailsDTO>> GetClassSubjectsBySubjectIdAsync(int subjectId)
        {
            ValidationHelper.ValidateId(subjectId);

            await EnsureSubjectExistsAsync(subjectId);

            return await _classSubjectData.GetClassSubjectsBySubjectIdAsync(subjectId);
        }

        public async Task<int> AddClassSubjectAsync(ClassSubjectDTO classSubject)
        {
            ValidateClassSubject(classSubject);

            await EnsureClassExistsAsync(classSubject.ClassID);
            await EnsureTeacherExistsAsync(classSubject.TeacherID);
            await EnsureSubjectExistsAsync(classSubject.SubjectID);
            await EnsureTeacherCanTeachSubjectAsync(classSubject.TeacherID, classSubject.SubjectID);
            await EnsureUniqueClassSubjectAsync(classSubject);

            int newClassSubjectId = await _classSubjectData.AddClassSubjectAsync(classSubject);

            if (newClassSubjectId <= 0)
                throw new InvalidOperationException("Failed to add class subject.");

            return newClassSubjectId;
        }

        public async Task<bool> UpdateClassSubjectAsync(ClassSubjectDTO classSubject)
        {
            ValidateClassSubject(classSubject);

            ValidationHelper.ValidateId(classSubject.ClassSubjectID);

            await EnsureClassSubjectExistsAsync(classSubject.ClassSubjectID);

            await EnsureClassExistsAsync(classSubject.ClassID);
            await EnsureTeacherExistsAsync(classSubject.TeacherID);
            await EnsureSubjectExistsAsync(classSubject.SubjectID);

            await EnsureTeacherCanTeachSubjectAsync(classSubject.TeacherID, classSubject.SubjectID);

            await EnsureUniqueClassSubjectAsync(classSubject, classSubject.ClassSubjectID);

            bool isUpdated = await _classSubjectData.UpdateClassSubjectAsync(classSubject);

            if (!isUpdated)
                throw new InvalidOperationException($"Failed to update class subject with ID {classSubject.ClassSubjectID}.");

            return isUpdated;
        }

        public async Task<bool> DeleteClassSubjectAsync(int classSubjectId)
        {
            ValidationHelper.ValidateId(classSubjectId);

            await EnsureClassSubjectExistsAsync(classSubjectId);

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