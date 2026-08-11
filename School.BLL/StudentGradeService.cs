using School.BLL.Common;
using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.ExamDTOs;
using School.DTO.StudentGradeDTOs.Requests;
using School.DTO.StudentGradeDTOs.Responses;
using School.DTO.StudentsDTOs.Responses;

namespace School.BLL
{
    public class StudentGradeService : IStudentGradeService
    {
        private readonly IStudentGradeData _studentGradeData;
        private readonly IStudentData _studentData;
        private readonly IExamData _examData;

        public StudentGradeService(IStudentGradeData studentGradeData, IStudentData studentData, IExamData examData)
        {
            _studentGradeData = studentGradeData;
            _studentData = studentData;
            _examData = examData;
        }

        #region Validation
        private static void ValidateStudentGrade(CreateStudentGradeRequest studentGrade)
        {
            ArgumentNullException.ThrowIfNull(studentGrade);

            ValidationHelper.ValidateId(studentGrade.StudentID);
            ValidationHelper.ValidateId(studentGrade.ExamID);
            ValidateGrade(studentGrade.Grade);
        }
        private static void ValidateStudentGrade(UpdateStudentGradeRequest studentGrade)
        {
            ArgumentNullException.ThrowIfNull(studentGrade);
            ValidateGrade(studentGrade.Grade);
        }
        private static void ValidateGrade(decimal grade)
        {
            if (grade < 0)
                throw new ArgumentException("Grade cannot be negative.", nameof(grade));

            if (decimal.Round(grade, 2) != grade)
                throw new ArgumentException("Grade cannot have more than 2 decimal places.", nameof(grade));
        }
        #endregion

        #region Ensure
        private async Task<StudentResponse> GetStudentOrThrowAsync(int studentId)
        {
            return await _studentData.GetStudentByIdAsync(studentId)
                ?? throw new KeyNotFoundException($"Student with ID {studentId} does not exist.");
        }

        private async Task<StudentGradeResponse> GetstudentGradeOrThrowAsync(int studentGradeId)
        {
            return await _studentGradeData.GetStudentGradeByIdAsync(studentGradeId)
                ?? throw new KeyNotFoundException($"student GradeId with ID {studentGradeId} does not exist.");
        }

        private async Task<ExamResponse> GetExamOrThrowAsync(int examId)
        {
            return await _examData.GetExamByIdAsync(examId)
                ?? throw new KeyNotFoundException($"Exam with ID {examId} does not exist.");
        }

        private static void EnsureStudentIsActive(StudentResponse student)
        {
            if (student.StatusID != 1) // Assuming 1 is the ID for active student status
                throw new InvalidOperationException(
                    $"Cannot record a grade for student {student.StudentID}: status is '{student.StatusName}', not 'Active'.");
        }


        private static void EnsureStudentBelongsToExamClass(StudentResponse student, ExamResponse exam)
        {
            if (student.ClassID != exam.ClassID)
                throw new InvalidOperationException(
                    $"Student {student.StudentID} belongs to class '{student.ClassName}', but the exam belongs to class '{exam.ClassName}'.");
        }

        private static void EnsureGradeWithinTotalMarks(decimal grade, ExamResponse exam)
        {
            if (grade > exam.TotalMarks)
                throw new ArgumentException($"Grade ({grade}) cannot exceed the exam's TotalMarks ({exam.TotalMarks}).", nameof(grade));
        }

        private static void EnsureGradeConsistentWithAbsence(decimal grade, bool isAbsent)
        {
            if (isAbsent && grade != 0)
                throw new ArgumentException("Grade must be 0 when IsAbsent is true.", nameof(grade));
        }

        //this method need to optimization!!!
        private async Task EnsureStudentGradeUniqueAsync(int studentId, int examId, int? studentGradeId = null)
        {
            List<StudentGradeResponse> examGrades = await _studentGradeData.GetStudentGradesByExamIdAsync(examId);

            bool isDuplicate = examGrades.Exists(g =>
                g.StudentID == studentId &&
                (studentGradeId == null || g.StudentGradeID != studentGradeId.Value));

            if (isDuplicate)
                throw new InvalidOperationException(
                    $"Student {studentId} already has a grade recorded for exam {examId}.");
        }
        #endregion

        #region Public
        public Task<List<StudentGradeResponse>> GetAllStudentGradesAsync()
        {
            return _studentGradeData.GetAllStudentGradesAsync();
        }

        public async Task<StudentGradeResponse?> GetStudentGradeByIdAsync(int studentGradeId)
        {
            ValidationHelper.ValidateId(studentGradeId);

            StudentGradeResponse? studentGradeDetails = await _studentGradeData.GetStudentGradeByIdAsync(studentGradeId);

            if (studentGradeDetails == null)
                throw new KeyNotFoundException($"StudentGrade with ID {studentGradeId} does not exist.");

            return studentGradeDetails;
        }

        public Task<List<StudentGradeResponse>> GetStudentGradesByStudentIdAsync(int studentId)
        {
            ValidationHelper.ValidateId(studentId);

            return _studentGradeData.GetStudentGradesByStudentIdAsync(studentId);
        }

        public Task<List<StudentGradeResponse>> GetStudentGradesByExamIdAsync(int examId)
        {
            ValidationHelper.ValidateId(examId);

            return _studentGradeData.GetStudentGradesByExamIdAsync(examId);
        }

        public Task<List<StudentGradeResponse>> GetStudentGradesByClassIdAsync(int classId)
        {
            ValidationHelper.ValidateId(classId);

            return _studentGradeData.GetStudentGradesByClassIdAsync(classId);
        }

        public Task<List<StudentGradeResponse>> GetStudentGradesBySubjectIdAsync(int subjectId)
        {
            ValidationHelper.ValidateId(subjectId);

            return _studentGradeData.GetStudentGradesBySubjectIdAsync(subjectId);
        }

        public async Task<int> AddStudentGradeAsync(CreateStudentGradeRequest studentGrade)
        {
            ValidateStudentGrade(studentGrade);

            StudentResponse student = await GetStudentOrThrowAsync(studentGrade.StudentID);
            ExamResponse exam = await GetExamOrThrowAsync(studentGrade.ExamID);

            EnsureStudentIsActive(student);
            EnsureStudentBelongsToExamClass(student, exam);
            EnsureGradeWithinTotalMarks(studentGrade.Grade, exam);
            EnsureGradeConsistentWithAbsence(studentGrade.Grade, studentGrade.IsAbsent);
            await EnsureStudentGradeUniqueAsync(studentGrade.StudentID, studentGrade.ExamID);

            int newStudentGradeId = await _studentGradeData.AddStudentGradeAsync(studentGrade);

            if (newStudentGradeId <= 0)
                throw new InvalidOperationException("Failed to add the student grade.");

            return newStudentGradeId;
        }

        public async Task<bool> UpdateStudentGradeAsync(int studentGradeId, UpdateStudentGradeRequest studentGrade)
        {
            ValidationHelper.ValidateId(studentGradeId);

            ValidateStudentGrade(studentGrade);

            var CurrentstudentGrade = await GetstudentGradeOrThrowAsync(studentGradeId);
            var student = await GetStudentOrThrowAsync(CurrentstudentGrade.StudentID);
            var exam = await GetExamOrThrowAsync(CurrentstudentGrade.ExamID);

            EnsureStudentIsActive(student);
            EnsureGradeWithinTotalMarks(studentGrade.Grade, exam);
            EnsureGradeConsistentWithAbsence(studentGrade.Grade, studentGrade.IsAbsent);

            bool isUpdated = await _studentGradeData.UpdateStudentGradeAsync(studentGradeId, studentGrade);

            if (!isUpdated)
                throw new InvalidOperationException($"Failed to update the student grade with ID {studentGradeId}");

            return isUpdated;
        }

        public async Task<bool> DeleteStudentGradeAsync(int studentGradeId)
        {
            ValidationHelper.ValidateId(studentGradeId);

            await EnsureHelper.EnsureExistsAsync(_studentGradeData.IsStudentGradeExistAsync, studentGradeId, "Student Grade");

            bool isDeleted = await _studentGradeData.DeleteStudentGradeAsync(studentGradeId);
            if (!isDeleted)
                throw new InvalidOperationException($"Failed to delete the student grade with ID {studentGradeId}");

            return isDeleted;
        }
        #endregion
    }
}