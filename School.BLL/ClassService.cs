using School.BLL.Common;
using School.BLL.Helpers;
using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.ClassesDTOs;

namespace School.BLL
{
    public class ClassService : IClassService
    {
        private readonly IClassData _classData;
        private readonly IGradeData _gradeData;
        private static int minClassNameLength => 2;
        private static int maxClassNameLength => 20;
        public ClassService(IClassData classData, IGradeData gradeData)
        {
            _classData = classData;
            _gradeData = gradeData;
        }
        #region Private Helpers

        private static void ValidateClass(ClassDTO schoolClass)
        {
            ArgumentNullException.ThrowIfNull(schoolClass);

            ValidationHelper.ValidateId(schoolClass.GradeID);

            schoolClass.ClassName = ValidationHelper.ValidateString(schoolClass.ClassName, nameof(schoolClass.ClassName), minClassNameLength, maxClassNameLength);

            schoolClass.AcademicYear = AcademicYearHelper.ValidateAcademicYear(schoolClass.AcademicYear);

            ValidateCapacity(schoolClass.Capacity);
        }

        //private static string ValidateClassName(string className)
        //{
        //    if (string.IsNullOrWhiteSpace(className))
        //        throw new ArgumentException("Class name is required.", nameof(className));

        //    className = className.Trim();

        //    if (className.Length > 10)
        //        throw new ArgumentException("Class name cannot exceed 10 characters.", nameof(className));

        //    return className;
        //}

        private static void ValidateCapacity(int capacity)
        {
            if (capacity is < 1 or > 100)
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be between 1 and 100.");
        }

        private async Task EnsureClassExistsAsync(int classId)
        {
            if (!await _classData.IsClassExistAsync(classId))
                throw new KeyNotFoundException($"Class with ID {classId} does not exist.");
        }

        private async Task EnsureGradeExistsAsync(byte gradeId)
        {
            if (!await _gradeData.IsGradeExistAsync(gradeId))
                throw new KeyNotFoundException($"Grade with ID {gradeId} does not exist.");
        }

        private async Task EnsureUniqueClassAsync(byte gradeId, string className, string academicYear, int? currentClassId = null)
        {
            ClassDetailsDTO? schoolClass = await _classData.GetClassByDetailsAsync(gradeId, className, academicYear);

            if (schoolClass == null)
                return;

            if (currentClassId.HasValue && schoolClass.ClassID == currentClassId.Value)
                return;

            throw new InvalidOperationException($"Class '{className}' already exists in Grade {gradeId} for academic year '{academicYear}'.");
        }
        #endregion

        #region Public Methods

        public async Task<List<ClassDetailsDTO>> GetAllClassesAsync()
        {
            return await _classData.GetAllClassesAsync();
        }

        public async Task<ClassDetailsDTO?> GetClassByIdAsync(int classId)
        {
            ValidationHelper.ValidateId(classId);

            ClassDetailsDTO? schoolClass = await _classData.GetClassByIdAsync(classId);

            if (schoolClass == null)
                throw new KeyNotFoundException($"Class with ID {classId} does not exist.");

            return schoolClass;
        }

        public async Task<ClassDetailsDTO?> GetClassByDetailsAsync(byte gradeId, string className, string academicYear)
        {
            ValidationHelper.ValidateId(gradeId);

            className = ValidationHelper.ValidateString(className, nameof(className), minClassNameLength, maxClassNameLength);
            academicYear = AcademicYearHelper.ValidateAcademicYear(academicYear);

            ClassDetailsDTO? classDetailsDTO = await _classData.GetClassByDetailsAsync(gradeId, className, academicYear);

            if (classDetailsDTO == null)
                throw new KeyNotFoundException($"Class '{className}' does not exist in Grade {gradeId} for academic year '{academicYear}'.");

            return classDetailsDTO;
        }

        public async Task<int> AddClassAsync(ClassDTO schoolClass)
        {
            ValidateClass(schoolClass);

            await EnsureGradeExistsAsync(schoolClass.GradeID);

            await EnsureUniqueClassAsync(schoolClass.GradeID, schoolClass.ClassName, schoolClass.AcademicYear);

            int newClassId = await _classData.AddClassAsync(schoolClass);

            if (newClassId <= 0)
                throw new InvalidOperationException("Failed to add class.");

            return newClassId;
        }

        public async Task<bool> UpdateClassAsync(ClassDTO schoolClass)
        {
            ValidateClass(schoolClass);

            ValidationHelper.ValidateId(schoolClass.ClassID);

            await EnsureClassExistsAsync(schoolClass.ClassID);

            await EnsureGradeExistsAsync(schoolClass.GradeID);

            await EnsureUniqueClassAsync(schoolClass.GradeID, schoolClass.ClassName, schoolClass.AcademicYear, schoolClass.ClassID);


            bool isUpdated = await _classData.UpdateClassAsync(schoolClass);

            if (!isUpdated)
                throw new InvalidOperationException($"Failed to update class with ID {schoolClass.ClassID}.");

            return isUpdated;
        }

        public async Task<bool> DeleteClassAsync(int classId)
        {
            ValidationHelper.ValidateId(classId);

            await EnsureClassExistsAsync(classId);

            bool isDeleted = await _classData.DeleteClassAsync(classId);

            if (!isDeleted)
                throw new InvalidOperationException($"Failed to delete class with ID {classId}.");

            return isDeleted;
        }

        public async Task<bool> IsClassExistAsync(int classId)
        {
            ValidationHelper.ValidateId(classId);

            return await _classData.IsClassExistAsync(classId);
        }
        #endregion
    }
}