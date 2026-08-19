using Moq;
using School.BLL;
using School.DAL.Interfaces;
using School.DTO.ExamDTOs;
using School.DTO.StudentGradeDTOs.Responses;
using School.DTO.StudentsDTOs.Responses;
using School.Tests.TestHelpers;
using Xunit;

namespace School.Tests.Services
{
    public class StudentGradeServiceTests
    {
        private readonly Mock<IStudentGradeData> _studentGradeDataMock = new();
        private readonly Mock<IStudentData> _studentDataMock = new();
        private readonly Mock<IExamData> _examDataMock = new();

        private readonly StudentGradeService _sut;

        public StudentGradeServiceTests()
        {
            _sut = new StudentGradeService(
                _studentGradeDataMock.Object,
                _studentDataMock.Object,
                _examDataMock.Object);
        }

        #region Helpers

        private void SetupExistingStudent(StudentResponse student)
        {
            _studentDataMock.Setup(d => d.GetStudentByIdAsync(student.StudentID))
                .ReturnsAsync(student);
        }

        private void SetupExistingExam(ExamResponse exam)
        {
            _examDataMock.Setup(d => d.GetExamByIdAsync(exam.ExamID))
                .ReturnsAsync(exam);
        }

        private void SetupNoExistingGradesForExam(int examId)
        {
            _studentGradeDataMock.Setup(d => d.GetStudentGradesByExamIdAsync(examId))
                .ReturnsAsync([]);
        }

        private void SetupAddHappyPath(StudentResponse? student = null, ExamResponse? exam = null)
        {
            student ??= TestDataBuilders.ValidStudent();
            exam ??= TestDataBuilders.ValidExam(examId: 1, classId: student.ClassID);

            SetupExistingStudent(student);
            SetupExistingExam(exam);
            SetupNoExistingGradesForExam(exam.ExamID);
        }

        private void SetupUpdateHappyPath(int studentGradeId, StudentGradeResponse? currentGrade = null)
        {
            currentGrade ??= TestDataBuilders.ValidStudentGrade(studentGradeId: studentGradeId, studentId: 1, examId: 1);

            _studentGradeDataMock.Setup(d => d.GetStudentGradeByIdAsync(studentGradeId))
                .ReturnsAsync(currentGrade);

            SetupExistingStudent(TestDataBuilders.ValidStudent(studentId: currentGrade.StudentID, classId: currentGrade.ClassID));

            SetupExistingExam(TestDataBuilders.ValidExam(examId: currentGrade.ExamID, classId: currentGrade.ClassID,
                    totalMarks: currentGrade.TotalMarks));
        }

        #endregion

        #region Add

        [Theory]
        [InlineData(-1)]
        [InlineData(-0.01)]
        public async Task AddStudentGradeAsync_Throws_WhenGradeIsNegative(decimal grade)
        {
            var request = TestDataBuilders.ValidCreateStudentGradeRequest(grade: grade);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.AddStudentGradeAsync(request));
        }

        [Fact]
        public async Task AddStudentGradeAsync_Throws_WhenGradeHasMoreThanTwoDecimalPlaces()
        {
            var request = TestDataBuilders.ValidCreateStudentGradeRequest(grade: 12.345m);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.AddStudentGradeAsync(request));
        }

        [Fact]
        public async Task AddStudentGradeAsync_Throws_WhenStudentDoesNotExist()
        {
            var request = TestDataBuilders.ValidCreateStudentGradeRequest();

            _studentDataMock.Setup(d => d.GetStudentByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((StudentResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.AddStudentGradeAsync(request));
        }

        [Fact]
        public async Task AddStudentGradeAsync_Throws_WhenExamDoesNotExist()
        {
            var request = TestDataBuilders.ValidCreateStudentGradeRequest();

            SetupExistingStudent(TestDataBuilders.ValidStudent(studentId: request.StudentID));

            _examDataMock.Setup(d => d.GetExamByIdAsync(It.IsAny<int>())).ReturnsAsync((ExamResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.AddStudentGradeAsync(request));
        }

        [Fact]
        public async Task AddStudentGradeAsync_Throws_WhenStudentIsNotActive()
        {
            var request = TestDataBuilders.ValidCreateStudentGradeRequest();

            SetupExistingStudent(TestDataBuilders.ValidStudent(studentId: request.StudentID, statusId: 2, statusName: "Suspended"));

            SetupExistingExam(TestDataBuilders.ValidExam(examId: request.ExamID));

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AddStudentGradeAsync(request));
        }

        [Fact]
        public async Task AddStudentGradeAsync_Throws_WhenStudentDoesNotBelongToExamClass()
        {
            var request = TestDataBuilders.ValidCreateStudentGradeRequest();

            SetupExistingStudent(TestDataBuilders.ValidStudent(studentId: request.StudentID, classId: 10));

            SetupExistingExam(TestDataBuilders.ValidExam(examId: request.ExamID, classId: 20));

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AddStudentGradeAsync(request));
        }

        [Fact]
        public async Task AddStudentGradeAsync_Throws_WhenGradeExceedsTotalMarks()
        {
            var request = TestDataBuilders.ValidCreateStudentGradeRequest(grade: 150);

            SetupExistingStudent(TestDataBuilders.ValidStudent(studentId: request.StudentID, classId: 10));

            SetupExistingExam(TestDataBuilders.ValidExam(examId: request.ExamID, classId: 10, totalMarks: 100));

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.AddStudentGradeAsync(request));
        }

        [Fact]
        public async Task AddStudentGradeAsync_Throws_WhenGradeIsNonZero_ButStudentIsAbsent()
        {
            var request = TestDataBuilders.ValidCreateStudentGradeRequest(grade: 10,
                    isAbsent: true);

            SetupExistingStudent(TestDataBuilders.ValidStudent(studentId: request.StudentID, classId: 10));

            SetupExistingExam(TestDataBuilders.ValidExam(examId: request.ExamID, classId: 10));

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.AddStudentGradeAsync(request));
        }

        [Fact]
        public async Task AddStudentGradeAsync_AllowsZeroGrade_WhenStudentIsAbsent()
        {
            var request = TestDataBuilders.ValidCreateStudentGradeRequest(grade: 0, isAbsent: true);

            SetupAddHappyPath(exam: TestDataBuilders.ValidExam(examId: request.ExamID, classId: 10));

            _studentGradeDataMock.Setup(d => d.AddStudentGradeAsync(request)).ReturnsAsync(1);

            int result = await _sut.AddStudentGradeAsync(request);

            Assert.Equal(1, result);
        }

        [Fact]
        public async Task AddStudentGradeAsync_Throws_WhenStudentAlreadyHasAGradeForThisExam()
        {
            var request = TestDataBuilders.ValidCreateStudentGradeRequest(studentId: 5);

            SetupAddHappyPath(student: TestDataBuilders.ValidStudent(studentId: request.StudentID, classId: 10),
                exam: TestDataBuilders.ValidExam(
                    examId: request.ExamID, classId: 10));

            var existingGrade = TestDataBuilders.ValidStudentGrade(studentGradeId: 99, studentId: request.StudentID,
                    examId: request.ExamID,
                    grade: 60);

            _studentGradeDataMock.Setup(d => d.GetStudentGradesByExamIdAsync(request.ExamID)).ReturnsAsync([existingGrade]);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AddStudentGradeAsync(request));
        }

        [Fact]
        public async Task AddStudentGradeAsync_ReturnsNewId_WhenRequestIsValid()
        {
            var request = TestDataBuilders.ValidCreateStudentGradeRequest(grade: 75);

            SetupAddHappyPath();

            _studentGradeDataMock.Setup(d => d.AddStudentGradeAsync(request)).ReturnsAsync(42);

            int result = await _sut.AddStudentGradeAsync(request);

            Assert.Equal(42, result);
        }

        [Fact]
        public async Task AddStudentGradeAsync_Throws_WhenDataLayerFailsToInsert()
        {
            var request = TestDataBuilders.ValidCreateStudentGradeRequest();

            SetupAddHappyPath();

            _studentGradeDataMock.Setup(d => d.AddStudentGradeAsync(request)).ReturnsAsync(0);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AddStudentGradeAsync(request));
        }

        #endregion

        #region Update

        [Fact]
        public async Task UpdateStudentGradeAsync_Throws_WhenStudentGradeDoesNotExist()
        {
            const int studentGradeId = 1;

            var request = TestDataBuilders.ValidUpdateStudentGradeRequest();

            _studentGradeDataMock.Setup(d => d.GetStudentGradeByIdAsync(studentGradeId))
                .ReturnsAsync((StudentGradeResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.UpdateStudentGradeAsync(studentGradeId,
                    request));
        }

        [Fact]
        public async Task UpdateStudentGradeAsync_Throws_WhenGradeExceedsTotalMarks()
        {
            const int studentGradeId = 1;

            var request = TestDataBuilders.ValidUpdateStudentGradeRequest(grade: 150);

            SetupUpdateHappyPath(studentGradeId);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.UpdateStudentGradeAsync(studentGradeId, request));
        }

        [Fact]
        public async Task UpdateStudentGradeAsync_ReturnsTrue_WhenUpdateSucceeds()
        {
            const int studentGradeId = 1;

            var request = TestDataBuilders.ValidUpdateStudentGradeRequest(grade: 80);

            SetupUpdateHappyPath(studentGradeId);

            _studentGradeDataMock.Setup(d => d.UpdateStudentGradeAsync(studentGradeId, request))
                .ReturnsAsync(true);

            bool result = await _sut.UpdateStudentGradeAsync(studentGradeId, request);

            Assert.True(result);
        }

        #endregion

        #region Get

        [Fact]
        public async Task GetStudentGradeByIdAsync_Throws_WhenNotFound()
        {
            _studentGradeDataMock.Setup(d => d.GetStudentGradeByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((StudentGradeResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetStudentGradeByIdAsync(1));
        }

        [Fact]
        public async Task GetStudentGradeByIdAsync_ReturnsGrade_WhenFound()
        {
            var grade = TestDataBuilders.ValidStudentGrade(studentGradeId: 1, studentId: 1, examId: 1);

            _studentGradeDataMock.Setup(d => d.GetStudentGradeByIdAsync(1))
                .ReturnsAsync(grade);

            var result = await _sut.GetStudentGradeByIdAsync(1);

            Assert.Equal(grade.StudentGradeID, result.StudentGradeID);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task DeleteStudentGradeAsync_Throws_WhenGradeDoesNotExist()
        {
            _studentGradeDataMock.Setup(d => d.IsStudentGradeExistAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.DeleteStudentGradeAsync(1));
        }

        [Fact]
        public async Task DeleteStudentGradeAsync_ReturnsTrue_WhenDeletionSucceeds()
        {
            _studentGradeDataMock.Setup(d => d.IsStudentGradeExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _studentGradeDataMock.Setup(d => d.DeleteStudentGradeAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            bool result = await _sut.DeleteStudentGradeAsync(1);

            Assert.True(result);
        }

        #endregion
    }
}