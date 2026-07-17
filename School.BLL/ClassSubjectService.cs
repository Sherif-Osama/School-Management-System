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

            ValidateClassId(classSubject.ClassID);
            ValidateTeacherId(classSubject.TeacherID);
            ValidateSubjectId(classSubject.SubjectID);
        }

        private static void ValidateClassSubjectId(int classSubjectId)
        {
            if (classSubjectId <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(classSubjectId),
                    "ClassSubject ID must be greater than zero.");
        }

        private static void ValidateClassId(int classId)
        {
            if (classId <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(classId),
                    "Class ID must be greater than zero.");
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

        private async Task EnsureClassExistsAsync(int classId)
        {
            if (!await _classData.IsClassExistAsync(classId))
                throw new InvalidOperationException(
                    $"Class with ID {classId} does not exist.");
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

        private async Task EnsureTeacherCanTeachSubjectAsync(
            int teacherId,
            int subjectId)
        {
            bool exists =
                await _teacherSubjectData.IsTeacherSubjectExistAsync(
                    new TeacherSubjectDTO
                    {
                        TeacherID = teacherId,
                        SubjectID = subjectId
                    });

            if (!exists)
                throw new InvalidOperationException(
                    "This teacher is not assigned to teach this subject.");
        }

        private async Task EnsureClassSubjectExistsAsync(int classSubjectId)
        {
            if (!await _classSubjectData.IsClassSubjectExistAsync(classSubjectId))
                throw new InvalidOperationException(
                    $"ClassSubject with ID {classSubjectId} does not exist.");
        }

        private async Task EnsureUniqueClassSubjectAsync(
            ClassSubjectDTO classSubject,
            int? currentClassSubjectId = null)
        {
            ClassSubjectDetailsDTO? relation =
                await _classSubjectData.GetClassSubjectByDetailsAsync(
                    classSubject.ClassID,
                    classSubject.TeacherID,
                    classSubject.SubjectID);

            if (relation == null)
                return;

            if (currentClassSubjectId.HasValue &&
                relation.ClassSubjectID == currentClassSubjectId.Value)
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

        public async Task<ClassSubjectDetailsDTO?> GetClassSubjectByIdAsync(
            int classSubjectId)
        {
            ValidateClassSubjectId(classSubjectId);

            return await _classSubjectData.GetClassSubjectByIdAsync(classSubjectId);
        }

        public async Task<List<ClassSubjectDetailsDTO>> GetClassSubjectsByClassIdAsync(
            int classId)
        {
            ValidateClassId(classId);

            await EnsureClassExistsAsync(classId);

            return await _classSubjectData.GetClassSubjectsByClassIdAsync(classId);
        }

        public async Task<List<ClassSubjectDetailsDTO>> GetClassSubjectsByTeacherIdAsync(
            int teacherId)
        {
            ValidateTeacherId(teacherId);

            await EnsureTeacherExistsAsync(teacherId);

            return await _classSubjectData.GetClassSubjectsByTeacherIdAsync(teacherId);
        }

        public async Task<List<ClassSubjectDetailsDTO>> GetClassSubjectsBySubjectIdAsync(
            byte subjectId)
        {
            ValidateSubjectId(subjectId);

            await EnsureSubjectExistsAsync(subjectId);

            return await _classSubjectData.GetClassSubjectsBySubjectIdAsync(subjectId);
        }

        public async Task<int> AddClassSubjectAsync(ClassSubjectDTO classSubject)
        {
            ValidateClassSubject(classSubject);

            await EnsureClassExistsAsync(classSubject.ClassID);
            await EnsureTeacherExistsAsync(classSubject.TeacherID);
            await EnsureSubjectExistsAsync(classSubject.SubjectID);

            await EnsureTeacherCanTeachSubjectAsync(
                classSubject.TeacherID,
                classSubject.SubjectID);

            await EnsureUniqueClassSubjectAsync(classSubject);

            return await _classSubjectData.AddClassSubjectAsync(classSubject);
        }

        public async Task<bool> UpdateClassSubjectAsync(ClassSubjectDTO classSubject)
        {
            ValidateClassSubject(classSubject);

            ValidateClassSubjectId(classSubject.ClassSubjectID);

            await EnsureClassSubjectExistsAsync(classSubject.ClassSubjectID);

            await EnsureClassExistsAsync(classSubject.ClassID);
            await EnsureTeacherExistsAsync(classSubject.TeacherID);
            await EnsureSubjectExistsAsync(classSubject.SubjectID);

            await EnsureTeacherCanTeachSubjectAsync(
                classSubject.TeacherID,
                classSubject.SubjectID);

            await EnsureUniqueClassSubjectAsync(
                classSubject,
                classSubject.ClassSubjectID);

            return await _classSubjectData.UpdateClassSubjectAsync(classSubject);
        }

        public async Task<bool> DeleteClassSubjectAsync(int classSubjectId)
        {
            ValidateClassSubjectId(classSubjectId);

            await EnsureClassSubjectExistsAsync(classSubjectId);

            return await _classSubjectData.DeleteClassSubjectAsync(classSubjectId);
        }

        public async Task<bool> IsClassSubjectExistAsync(int classSubjectId)
        {
            ValidateClassSubjectId(classSubjectId);

            return await _classSubjectData.IsClassSubjectExistAsync(classSubjectId);
        }

        #endregion
    }
}