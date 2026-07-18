using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.ExamDTOs;
using School.DTO.StudentGradeDetailsDTOs;
using School.DTO.StudentGradeDTOs;
using School.DTO.StudentsDTOs;

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
        private static void ValidateStudentGrade(StudentGradeDTO studentGrade)
        {
            ArgumentNullException.ThrowIfNull(studentGrade);

            ValidateStudentId(studentGrade.StudentID);
            ValidateExamId(studentGrade.ExamID);
            ValidateGrade(studentGrade.Grade);
        }

        private static void ValidateStudentGradeId(int studentGradeId)
        {
            if (studentGradeId <= 0)
                throw new ArgumentException("StudentGradeID must be a positive number.", nameof(studentGradeId));
        }


        private static void ValidateStudentId(int studentId)
        {
            if (studentId <= 0)
                throw new ArgumentException("StudentID must be a positive number.", nameof(studentId));
        }

        private static void ValidateExamId(int examId)
        {
            if (examId <= 0)
                throw new ArgumentException("ExamID must be a positive number.", nameof(examId));
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
        private async Task<StudentDetailsDTO> GetStudentOrThrowAsync(int studentId)
        {
            return await _studentData.GetStudentByIdAsync(studentId)
                ?? throw new KeyNotFoundException($"Student with ID {studentId} does not exist.");
        }

        private async Task<ExamDetailsDTO> GetExamOrThrowAsync(int examId)
        {
            return await _examData.GetExamByIdAsync(examId)
                ?? throw new KeyNotFoundException($"Exam with ID {examId} does not exist.");
        }

        private async Task EnsureStudentGradeExistsAsync(int studentGradeId)
        {
            if (!await _studentGradeData.IsStudentGradeExistAsync(studentGradeId))
                throw new KeyNotFoundException($"StudentGrade with ID {studentGradeId} does not exist.");
        }

        private static void EnsureStudentIsActive(StudentDetailsDTO student)
        {
            if (student.StatusID != 1) // Assuming 1 is the ID for active student status
                throw new InvalidOperationException(
                    $"Cannot record a grade for student {student.StudentID}: status is '{student.StatusName}', not 'Active'.");
        }


        private static void EnsureStudentBelongsToExamClass(StudentDetailsDTO student, ExamDetailsDTO exam)
        {
            if (student.ClassID != exam.ClassID)
                throw new InvalidOperationException(
                    $"Student {student.StudentID} belongs to class '{student.ClassName}', but the exam belongs to class '{exam.ClassName}'.");
        }

        private static void EnsureGradeWithinTotalMarks(decimal grade, ExamDetailsDTO exam)
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
            List<StudentGradeDetailsDTO> examGrades = await _studentGradeData.GetStudentGradesByExamIdAsync(examId);

            bool isDuplicate = examGrades.Exists(g =>
                g.StudentID == studentId &&
                (studentGradeId == null || g.StudentGradeID != studentGradeId.Value));

            if (isDuplicate)
                throw new InvalidOperationException(
                    $"Student {studentId} already has a grade recorded for exam {examId}.");
        }
        #endregion

        #region Public
        public Task<List<StudentGradeDetailsDTO>> GetAllStudentGradesAsync()
        {
            return _studentGradeData.GetAllStudentGradesAsync();
        }

        public async Task<StudentGradeDetailsDTO?> GetStudentGradeByIdAsync(int studentGradeId)
        {
            ValidateStudentGradeId(studentGradeId);

            StudentGradeDetailsDTO? studentGradeDetails = await _studentGradeData.GetStudentGradeByIdAsync(studentGradeId);

            if (studentGradeDetails == null)
                throw new KeyNotFoundException($"StudentGrade with ID {studentGradeId} does not exist.");

            return studentGradeDetails;
        }

        public Task<List<StudentGradeDetailsDTO>> GetStudentGradesByStudentIdAsync(int studentId)
        {
            ValidateStudentId(studentId);

            return _studentGradeData.GetStudentGradesByStudentIdAsync(studentId);
        }

        public Task<List<StudentGradeDetailsDTO>> GetStudentGradesByExamIdAsync(int examId)
        {
            ValidateExamId(examId);

            return _studentGradeData.GetStudentGradesByExamIdAsync(examId);
        }

        public Task<List<StudentGradeDetailsDTO>> GetStudentGradesByClassIdAsync(int classId)
        {
            if (classId <= 0)
                throw new ArgumentException("ClassID must be a positive number.", nameof(classId));

            return _studentGradeData.GetStudentGradesByClassIdAsync(classId);
        }

        public Task<List<StudentGradeDetailsDTO>> GetStudentGradesBySubjectIdAsync(int subjectId)
        {
            if (subjectId <= 0)
                throw new ArgumentException("SubjectID must be a positive number.", nameof(subjectId));

            return _studentGradeData.GetStudentGradesBySubjectIdAsync(subjectId);
        }

        public async Task<int> AddStudentGradeAsync(StudentGradeDTO studentGrade)
        {
            ValidateStudentGrade(studentGrade);

            StudentDetailsDTO student = await GetStudentOrThrowAsync(studentGrade.StudentID);
            ExamDetailsDTO exam = await GetExamOrThrowAsync(studentGrade.ExamID);

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

        public async Task<bool> UpdateStudentGradeAsync(StudentGradeDTO studentGrade)
        {
            ValidateStudentGrade(studentGrade);
            ValidateStudentGradeId(studentGrade.StudentGradeID);

            await EnsureStudentGradeExistsAsync(studentGrade.StudentGradeID);

            StudentDetailsDTO student = await GetStudentOrThrowAsync(studentGrade.StudentID);
            ExamDetailsDTO exam = await GetExamOrThrowAsync(studentGrade.ExamID);

            EnsureStudentIsActive(student);
            EnsureStudentBelongsToExamClass(student, exam);
            EnsureGradeWithinTotalMarks(studentGrade.Grade, exam);
            EnsureGradeConsistentWithAbsence(studentGrade.Grade, studentGrade.IsAbsent);
            await EnsureStudentGradeUniqueAsync(studentGrade.StudentID, studentGrade.ExamID, studentGrade.StudentGradeID);

            bool isUpdated = await _studentGradeData.UpdateStudentGradeAsync(studentGrade);

            if (!isUpdated)
                throw new InvalidOperationException($"Failed to update the student grade with ID {studentGrade.StudentGradeID}");

            return isUpdated;
        }

        public async Task<bool> DeleteStudentGradeAsync(int studentGradeId)
        {
            ValidateStudentGradeId(studentGradeId);

            await EnsureStudentGradeExistsAsync(studentGradeId);

            bool isDeleted = await _studentGradeData.DeleteStudentGradeAsync(studentGradeId);
            if (!isDeleted)
                throw new InvalidOperationException($"Failed to delete the student grade with ID {studentGradeId}");

            return isDeleted;
        }
        #endregion
    }
}