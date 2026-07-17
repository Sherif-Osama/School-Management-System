using School.DAL.Interfaces;
using School.DTO.ClassesDTOs;
using System.Text.RegularExpressions;

namespace School.BLL
{
    public class ClassService
    {
        private readonly IClassData _classData;
        private readonly IGradeData _gradeData;

        public ClassService(IClassData classData, IGradeData gradeData)
        {
            _classData = classData;
            _gradeData = gradeData;
        }

        #region Private Helpers

        private static void ValidateClass(ClassDTO schoolClass)
        {
            ArgumentNullException.ThrowIfNull(schoolClass);

            ValidateGradeId(schoolClass.GradeID);

            schoolClass.ClassName =
                ValidateClassName(schoolClass.ClassName);

            schoolClass.AcademicYear =
                ValidateAcademicYear(schoolClass.AcademicYear);

            ValidateCapacity(schoolClass.Capacity);
        }

        private static void ValidateClassId(int classId)
        {
            if (classId <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(classId),
                    "Class ID must be greater than zero.");
        }

        private static void ValidateGradeId(byte gradeId)
        {
            if (gradeId <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(gradeId),
                    "Grade ID must be greater than zero.");
        }

        private static string ValidateClassName(string className)
        {
            if (string.IsNullOrWhiteSpace(className))
                throw new ArgumentException(
                    "Class name is required.",
                    nameof(className));

            className = className.Trim();

            if (className.Length > 10)
                throw new ArgumentException(
                    "Class name cannot exceed 10 characters.",
                    nameof(className));

            return className;
        }

        private static string ValidateAcademicYear(string academicYear)
        {
            if (string.IsNullOrWhiteSpace(academicYear))
                throw new ArgumentException(
                    "Academic year is required.",
                    nameof(academicYear));

            academicYear = academicYear.Trim();

            if (!Regex.IsMatch(academicYear, @"^\d{4}-\d{4}$"))
                throw new ArgumentException(
                    "Academic year must be in the format YYYY-YYYY.",
                    nameof(academicYear));

            int startYear = int.Parse(academicYear[..4]);
            int endYear = int.Parse(academicYear[5..]);

            if (endYear != startYear + 1)
                throw new ArgumentException(
                    "Academic year is invalid.",
                    nameof(academicYear));

            return academicYear;
        }

        private static void ValidateCapacity(int capacity)
        {
            if (capacity is < 1 or > 100)
                throw new ArgumentOutOfRangeException(
                    nameof(capacity),
                    "Capacity must be between 1 and 100.");
        }

        private async Task EnsureClassExistsAsync(int classId)
        {
            if (!await _classData.IsClassExistAsync(classId))
                throw new InvalidOperationException(
                    $"Class with ID {classId} does not exist.");
        }

        private async Task EnsureGradeExistsAsync(byte gradeId)
        {
            if (!await _gradeData.IsGradeExistAsync(gradeId))
                throw new InvalidOperationException(
                    $"Grade with ID {gradeId} does not exist.");
        }

        private async Task EnsureUniqueClassAsync(
            byte gradeId,
            string className,
            string academicYear,
            int? currentClassId = null)
        {
            ClassDetailsDTO? schoolClass =
                await _classData.GetClassByDetailsAsync(
                    gradeId,
                    className,
                    academicYear);

            if (schoolClass == null)
                return;

            if (currentClassId.HasValue &&
                schoolClass.ClassID == currentClassId.Value)
                return;

            throw new InvalidOperationException(
                $"Class '{className}' already exists in Grade {gradeId} for academic year '{academicYear}'.");
        }

        #endregion

        #region Public Methods

        public async Task<List<ClassDetailsDTO>> GetAllClassesAsync()
        {
            return await _classData.GetAllClassesAsync();
        }

        public async Task<ClassDetailsDTO?> GetClassByIdAsync(int classId)
        {
            ValidateClassId(classId);

            return await _classData.GetClassByIdAsync(classId);
        }

        public async Task<ClassDetailsDTO?> GetClassByDetailsAsync(
            byte gradeId,
            string className,
            string academicYear)
        {
            ValidateGradeId(gradeId);

            className = ValidateClassName(className);
            academicYear = ValidateAcademicYear(academicYear);

            return await _classData.GetClassByDetailsAsync(
                gradeId,
                className,
                academicYear);
        }

        public async Task<int> AddClassAsync(ClassDTO schoolClass)
        {
            ValidateClass(schoolClass);

            await EnsureGradeExistsAsync(schoolClass.GradeID);

            await EnsureUniqueClassAsync(
                schoolClass.GradeID,
                schoolClass.ClassName,
                schoolClass.AcademicYear);

            return await _classData.AddClassAsync(schoolClass);
        }

        public async Task<bool> UpdateClassAsync(ClassDTO schoolClass)
        {
            ValidateClass(schoolClass);

            ValidateClassId(schoolClass.ClassID);

            await EnsureClassExistsAsync(schoolClass.ClassID);

            await EnsureGradeExistsAsync(schoolClass.GradeID);

            await EnsureUniqueClassAsync(
                schoolClass.GradeID,
                schoolClass.ClassName,
                schoolClass.AcademicYear,
                schoolClass.ClassID);

            return await _classData.UpdateClassAsync(schoolClass);
        }

        public async Task<bool> DeleteClassAsync(int classId)
        {
            ValidateClassId(classId);

            await EnsureClassExistsAsync(classId);

            return await _classData.DeleteClassAsync(classId);
        }

        public async Task<bool> IsClassExistAsync(int classId)
        {
            ValidateClassId(classId);

            return await _classData.IsClassExistAsync(classId);
        }
        #endregion
    }
}