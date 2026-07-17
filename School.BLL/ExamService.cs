using School.DAL.Interfaces;
using School.DTO.AssociationsDTOs.ClassSubjectDTOs;
using School.DTO.ExamDTOs;

namespace School.BLL
{
    public class ExamService
    {
        private readonly IExamData _examData;
        private readonly IClassSubjectData _classSubjectData;
        private readonly IExamTypeData _examTypeData;
        private readonly IClassData _classData;

        private const decimal MinTotalMarks = 1;
        private const decimal MaxTotalMarks = 1000;

        private static readonly DateOnly MinExamDate = new(2000, 1, 1);

        public ExamService(IExamData examData, IClassSubjectData classSubjectData, IExamTypeData examTypeData, IClassData classData)
        {
            _examData = examData;
            _classSubjectData = classSubjectData;
            _examTypeData = examTypeData;
            _classData = classData;
        }

        #region Validation
        private static void ValidateExam(ExamDTO exam)
        {
            ArgumentNullException.ThrowIfNull(exam);

            ValidateClassSubjectId(exam.ClassSubjectID);
            ValidateExamTypeId(exam.ExamTypeID);
            ValidateExamDate(exam.ExamDate);
            ValidateTotalMarks(exam.TotalMarks);
        }

        private static void ValidateExamId(int examId)
        {
            if (examId <= 0)
                throw new ArgumentException("ExamID must be a positive number.", nameof(examId));
        }

        private static void ValidateClassSubjectId(int classSubjectId)
        {
            if (classSubjectId <= 0)
                throw new ArgumentException("ClassSubjectID must be a positive number.", nameof(classSubjectId));
        }

        private static void ValidateExamTypeId(int examTypeId)
        {
            if (examTypeId <= 0)
                throw new ArgumentException("ExamTypeID must be a positive number.", nameof(examTypeId));
        }

        private static DateOnly ValidateExamDate(DateOnly examDate)
        {
            if (examDate == default)
                throw new ArgumentException("ExamDate must be a valid date.", nameof(examDate));

            if (examDate < MinExamDate)
                throw new ArgumentException($"ExamDate cannot be earlier than {MinExamDate:yyyy-MM-dd}.", nameof(examDate));

            DateOnly maxExamDate = DateOnly.FromDateTime(DateTime.Today).AddYears(2);

            if (examDate > maxExamDate)
                throw new ArgumentException($"ExamDate cannot be more than 2 years in the future ({maxExamDate:yyyy-MM-dd}).", nameof(examDate));

            return examDate;
        }

        private static decimal ValidateTotalMarks(decimal totalMarks)
        {
            if (totalMarks < MinTotalMarks)
                throw new ArgumentException($"TotalMarks must be at least {MinTotalMarks}.", nameof(totalMarks));

            if (totalMarks > MaxTotalMarks)
                throw new ArgumentException($"TotalMarks cannot exceed {MaxTotalMarks}.", nameof(totalMarks));

            if (decimal.Round(totalMarks, 2) != totalMarks)
                throw new ArgumentException("TotalMarks cannot have more than 2 decimal places.", nameof(totalMarks));

            return totalMarks;
        }
        #endregion

        #region Ensure
        private async Task EnsureExamExistsAsync(int examId)
        {
            if (!await _examData.IsExamExistAsync(examId))
                throw new InvalidOperationException($"Exam with ID {examId} does not exist.");
        }

        private async Task<ClassSubjectDetailsDTO> GetValidatedClassSubjectAsync(int classSubjectId)
        {
            var classSubject = await _classSubjectData.GetClassSubjectByIdAsync(classSubjectId);

            return classSubject
                ?? throw new InvalidOperationException($"ClassSubject with ID {classSubjectId} does not exist.");
        }

        private async Task EnsureExamTypeExistsAsync(int examTypeId)
        {
            if (!await _examTypeData.IsExamTypeExistAsync(examTypeId))
                throw new InvalidOperationException($"ExamType with ID {examTypeId} does not exist.");
        }

        private async Task EnsureExamUniqueAsync(ClassSubjectDetailsDTO classSubject, int examTypeId, int? examId = null)
        {
            List<ExamDetailsDTO> classExams = await _examData.GetExamsByClassIdAsync(classSubject.ClassID);

            bool isDuplicate = classExams.Exists(e =>
                e.SubjectID == classSubject.SubjectID &&
                e.TeacherID == classSubject.TeacherID &&
                e.ExamTypeID == examTypeId &&
                (examId == null || e.ExamID != examId.Value));

            if (isDuplicate)
                throw new InvalidOperationException("An exam of this type already exists for this class subject.");
        }

        private async Task EnsureExamDateWithinAcademicYearAsync(ClassSubjectDetailsDTO classSubject, DateOnly examDate)
        {
            var schoolClass = await _classData.GetClassByIdAsync(classSubject.ClassID)
                ?? throw new InvalidOperationException($"Class with ID {classSubject.ClassID} does not exist.");

            if (!schoolClass.IsActive)
                throw new InvalidOperationException("Cannot schedule an exam for an inactive class.");

            (DateOnly start, DateOnly end) = ParseAcademicYear(schoolClass.AcademicYear);

            if (examDate < start || examDate > end)
                throw new ArgumentException(
                    $"ExamDate must fall within the class's academic year {schoolClass.AcademicYear} ({start:yyyy-MM-dd} to {end:yyyy-MM-dd}).",
                    nameof(examDate));
        }

        private static (DateOnly Start, DateOnly End) ParseAcademicYear(string academicYear)
        {
            string[] parts = academicYear.Split('-');

            if (parts.Length != 2 ||
                !int.TryParse(parts[0], out int startYear) ||
                !int.TryParse(parts[1], out int endYear))
                throw new InvalidOperationException($"AcademicYear '{academicYear}' is not in a recognizable 'YYYY-YYYY' format.");

            return (new DateOnly(startYear, 9, 1), new DateOnly(endYear, 8, 31));
        }
        #endregion

        #region Public
        public async Task<List<ExamDetailsDTO>> GetAllExamsAsync()
        {
            return await _examData.GetAllExamsAsync();
        }

        public async Task<ExamDetailsDTO?> GetExamByIdAsync(int examId)
        {
            ValidateExamId(examId);

            return await _examData.GetExamByIdAsync(examId);
        }

        public async Task<List<ExamDetailsDTO>> GetExamsByClassIdAsync(int classId)
        {
            if (classId <= 0)
                throw new ArgumentException("ClassID must be a positive number.", nameof(classId));
            return await _examData.GetExamsByClassIdAsync(classId);
        }

        public async Task<List<ExamDetailsDTO>> GetExamsByTeacherIdAsync(int teacherId)
        {
            if (teacherId <= 0)
                throw new ArgumentException("TeacherID must be a positive number.", nameof(teacherId));

            return await _examData.GetExamsByTeacherIdAsync(teacherId);
        }

        public async Task<List<ExamDetailsDTO>> GetExamsBySubjectIdAsync(int subjectId)
        {
            if (subjectId <= 0)
                throw new ArgumentException("SubjectID must be a positive number.", nameof(subjectId));

            return await _examData.GetExamsBySubjectIdAsync(subjectId);
        }

        public async Task<int> AddExamAsync(ExamDTO exam)
        {
            ValidateExam(exam);

            var classSubject = await GetValidatedClassSubjectAsync(exam.ClassSubjectID);
            await EnsureExamTypeExistsAsync(exam.ExamTypeID);
            await EnsureExamUniqueAsync(classSubject, exam.ExamTypeID);
            await EnsureExamDateWithinAcademicYearAsync(classSubject, exam.ExamDate);

            return await _examData.AddExamAsync(exam);
        }

        public async Task<bool> UpdateExamAsync(ExamDTO exam)
        {
            ValidateExam(exam);
            ValidateExamId(exam.ExamID);

            await EnsureExamExistsAsync(exam.ExamID);
            var classSubject = await GetValidatedClassSubjectAsync(exam.ClassSubjectID);
            await EnsureExamTypeExistsAsync(exam.ExamTypeID);
            await EnsureExamUniqueAsync(classSubject, exam.ExamTypeID, exam.ExamID);
            await EnsureExamDateWithinAcademicYearAsync(classSubject, exam.ExamDate);

            return await _examData.UpdateExamAsync(exam);
        }

        public async Task<bool> DeleteExamAsync(int examId)
        {
            ValidateExamId(examId);

            await EnsureExamExistsAsync(examId);

            return await _examData.DeleteExamAsync(examId);
        }
        #endregion
    }
}