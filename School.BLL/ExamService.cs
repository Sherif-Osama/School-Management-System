using School.BLL.Common;
using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.AssociationsDTOs.ClassSubjectDTOs.Responses;
using School.DTO.ExamDTOs;
using School.DTO.ExamDTOs.Requests;

namespace School.BLL
{
    public class ExamService : IExamService
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
        private static void ValidateExam(CreateExamRequest exam)
        {
            ArgumentNullException.ThrowIfNull(exam);

            ValidationHelper.ValidateId(exam.ClassSubjectID);
            ValidationHelper.ValidateId(exam.ExamTypeID);
            ValidateExamDate(exam.ExamDate);
            ValidateTotalMarks(exam.TotalMarks);
        }

        private static void ValidateExam(UpdateExamRequest exam)
        {
            ArgumentNullException.ThrowIfNull(exam);

            ValidationHelper.ValidateId(exam.ClassSubjectID);
            ValidationHelper.ValidateId(exam.ExamTypeID);
            ValidateExamDate(exam.ExamDate);
            ValidateTotalMarks(exam.TotalMarks);
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
                throw new ArgumentOutOfRangeException(nameof(totalMarks), totalMarks, $"TotalMarks must be at least {MinTotalMarks}.");

            if (totalMarks > MaxTotalMarks)
                throw new ArgumentOutOfRangeException(nameof(totalMarks), totalMarks, $"TotalMarks cannot exceed {MaxTotalMarks}.");

            if (decimal.Round(totalMarks, 2) != totalMarks)
                throw new ArgumentOutOfRangeException(nameof(totalMarks), totalMarks, "TotalMarks cannot have more than 2 decimal places.");

            return totalMarks;
        }
        #endregion

        #region Ensure
        private async Task<ClassSubjectResponse> GetValidatedClassSubjectAsync(int classSubjectId)
        {
            var classSubject = await _classSubjectData.GetClassSubjectByIdAsync(classSubjectId);

            return classSubject
                ?? throw new KeyNotFoundException($"ClassSubject with ID {classSubjectId} does not exist.");
        }

        private async Task EnsureExamUniqueAsync(int classSubjectId, int examTypeId, int? examId = null)
        {
            bool exists = await _examData.IsExamDuplicate(classSubjectId, examTypeId, examId);

            if (exists)
                throw new InvalidOperationException("An exam of this type already exists for this class subject.");
        }

        private async Task EnsureExamDateWithinAcademicYearAsync(ClassSubjectResponse classSubject, DateOnly examDate)
        {
            var schoolClass = await _classData.GetClassByIdAsync(classSubject.ClassID)
                ?? throw new KeyNotFoundException($"Class with ID {classSubject.ClassID} does not exist.");

            if (!schoolClass.IsActive)
                throw new InvalidOperationException("Cannot schedule an exam for an inactive class.");

            (DateOnly start, DateOnly end) = AcademicYearHelper.GetAcademicYearRange(schoolClass.AcademicYear);

            if (examDate < start || examDate > end)
                throw new ArgumentException($"ExamDate must fall within the class's academic year {schoolClass.AcademicYear} ({start:yyyy-MM-dd} to {end:yyyy-MM-dd}).",
                    nameof(examDate));
        }
        #endregion

        #region Public
        public async Task<List<ExamResponse>> GetAllExamsAsync()
        {
            return await _examData.GetAllExamsAsync();
        }

        public async Task<ExamResponse?> GetExamByIdAsync(int examId)
        {
            ValidationHelper.ValidateId(examId);

            ExamResponse? exam = await _examData.GetExamByIdAsync(examId);

            if (exam == null)
                throw new KeyNotFoundException($"Exam with ID {examId} does not exist.");

            return exam;
        }

        public async Task<List<ExamResponse>> GetExamsByClassIdAsync(int classId)
        {
            ValidationHelper.ValidateId(classId);
            return await _examData.GetExamsByClassIdAsync(classId);
        }

        public async Task<List<ExamResponse>> GetExamsByTeacherIdAsync(int teacherId)
        {
            ValidationHelper.ValidateId(teacherId);
            return await _examData.GetExamsByTeacherIdAsync(teacherId);
        }

        public async Task<List<ExamResponse>> GetExamsBySubjectIdAsync(int subjectId)
        {
            ValidationHelper.ValidateId(subjectId);
            return await _examData.GetExamsBySubjectIdAsync(subjectId);
        }

        public async Task<int> AddExamAsync(CreateExamRequest exam)
        {
            ValidateExam(exam);

            var classSubject = await GetValidatedClassSubjectAsync(exam.ClassSubjectID);
            await EnsureHelper.EnsureExistsAsync(_examTypeData.IsExamTypeExistAsync, exam.ExamTypeID, "Exam Type");
            await EnsureExamUniqueAsync(exam.ClassSubjectID, exam.ExamTypeID);
            await EnsureExamDateWithinAcademicYearAsync(classSubject, exam.ExamDate);

            int newExamId = await _examData.AddExamAsync(exam);

            if (newExamId <= 0)
                throw new InvalidOperationException($"Failed to add exam.");

            return newExamId;
        }

        public async Task<bool> UpdateExamAsync(int examId, UpdateExamRequest exam)
        {
            ValidateExam(exam);
            ValidationHelper.ValidateId(examId);

            await EnsureHelper.EnsureExistsAsync(_examData.IsExamExistAsync, examId, "Exam");
            var classSubject = await GetValidatedClassSubjectAsync(exam.ClassSubjectID);
            await EnsureHelper.EnsureExistsAsync(_examTypeData.IsExamTypeExistAsync, exam.ExamTypeID, "Exam Type");
            await EnsureExamUniqueAsync(exam.ClassSubjectID, exam.ExamTypeID, examId);
            await EnsureExamDateWithinAcademicYearAsync(classSubject, exam.ExamDate);

            bool isUpdated = await _examData.UpdateExamAsync(examId, exam);

            if (!isUpdated)
                throw new InvalidOperationException($"Failed to update exam with ID {examId}.");

            return isUpdated;
        }

        public async Task<bool> DeleteExamAsync(int examId)
        {
            ValidationHelper.ValidateId(examId);

            await EnsureHelper.EnsureExistsAsync(_examData.IsExamExistAsync, examId, "Exam");

            bool isDeleted = await _examData.DeleteExamAsync(examId);

            if (!isDeleted)
                throw new InvalidOperationException($"Failed to delete exam with ID {examId}.");

            return isDeleted;
        }
        #endregion
    }
}