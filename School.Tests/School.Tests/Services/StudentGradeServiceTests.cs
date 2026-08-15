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
            _studentDataMock
                .Setup(d => d.GetStudentByIdAsync(student.StudentID))
                .ReturnsAsync(student);
        }

        private void SetupExistingExam(ExamResponse exam)
        {
            _examDataMock
                .Setup(d => d.GetExamByIdAsync(exam.ExamID))
                .ReturnsAsync(exam);
        }

        private void SetupNoExistingGradesForExam(int examId)
        {
            _studentGradeDataMock
                .Setup(d => d.GetStudentGradesByExamIdAsync(examId))
                .ReturnsAsync([]);
        }
        #endregion

        #region AddStudentGradeAsync — Input validation
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
        public async Task AddStudentGradeAsync_Throws_WhenStudentIdIsInvalid()
        {
            var request = TestDataBuilders.ValidCreateStudentGradeRequest(studentId: 0);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.AddStudentGradeAsync(request));
        }
        #endregion

        #region AddStudentGradeAsync — Business rules
        [Fact]
        public async Task AddStudentGradeAsync_Throws_WhenStudentDoesNotExist()
        {
            var request = TestDataBuilders.ValidCreateStudentGradeRequest();
            _studentDataMock.Setup(d => d.GetStudentByIdAsync(request.StudentID)).ReturnsAsync((StudentResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.AddStudentGradeAsync(request));
        }

        [Fact]
        public async Task AddStudentGradeAsync_Throws_WhenExamDoesNotExist()
        {
            var request = TestDataBuilders.ValidCreateStudentGradeRequest();
            SetupExistingStudent(TestDataBuilders.ValidStudent(studentId: request.StudentID));
            _examDataMock.Setup(d => d.GetExamByIdAsync(request.ExamID)).ReturnsAsync((ExamResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.AddStudentGradeAsync(request));
        }

        [Fact]
        public async Task AddStudentGradeAsync_Throws_WhenStudentIsNotActive()
        {
            var request = TestDataBuilders.ValidCreateStudentGradeRequest();
            SetupExistingStudent(TestDataBuilders.ValidStudent(
                studentId: request.StudentID, statusId: 2, statusName: "Suspended"));
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
            var request = TestDataBuilders.ValidCreateStudentGradeRequest(grade: 10, isAbsent: true);
            SetupExistingStudent(TestDataBuilders.ValidStudent(studentId: request.StudentID, classId: 10));
            SetupExistingExam(TestDataBuilders.ValidExam(examId: request.ExamID, classId: 10));

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.AddStudentGradeAsync(request));
        }

        [Fact]
        public async Task AddStudentGradeAsync_AllowsZeroGrade_WhenStudentIsAbsent()
        {
            var request = TestDataBuilders.ValidCreateStudentGradeRequest(grade: 0, isAbsent: true);
            SetupExistingStudent(TestDataBuilders.ValidStudent(studentId: request.StudentID, classId: 10));
            SetupExistingExam(TestDataBuilders.ValidExam(examId: request.ExamID, classId: 10));
            SetupNoExistingGradesForExam(request.ExamID);
            _studentGradeDataMock.Setup(d => d.AddStudentGradeAsync(request)).ReturnsAsync(1);

            int newId = await _sut.AddStudentGradeAsync(request);

            Assert.Equal(1, newId);
        }

        [Fact]
        public async Task AddStudentGradeAsync_Throws_WhenStudentAlreadyHasAGradeForThisExam()
        {
            var request = TestDataBuilders.ValidCreateStudentGradeRequest(studentId: 5);
            SetupExistingStudent(TestDataBuilders.ValidStudent(studentId: request.StudentID, classId: 10));
            SetupExistingExam(TestDataBuilders.ValidExam(examId: request.ExamID, classId: 10));

            var existingGrade = new StudentGradeResponse
            {
                StudentGradeID = 99,
                StudentID = request.StudentID,
                PersonID = 100,
                FirstName = "Ahmed",
                SecondName = "Mohamed",
                ThirdName = "Ali",
                GradeID = 1,
                GradeName = "Grade 1",
                ClassID = 10,
                ClassName = "A",
                AcademicYear = "2025/2026",
                SubjectID = 1,
                SubjectName = "Math",
                ExamID = request.ExamID,
                ExamTypeID = 1,
                ExamName = "Midterm",
                ExamDate = DateTime.Today,
                TotalMarks = 100,
                Grade = 60,
                IsAbsent = false
            };
            _studentGradeDataMock
                .Setup(d => d.GetStudentGradesByExamIdAsync(request.ExamID))
                .ReturnsAsync([existingGrade]);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AddStudentGradeAsync(request));
        }

        [Fact]
        public async Task AddStudentGradeAsync_Throws_WhenDataLayerFailsToInsert()
        {
            var request = TestDataBuilders.ValidCreateStudentGradeRequest();
            SetupExistingStudent(TestDataBuilders.ValidStudent(studentId: request.StudentID, classId: 10));
            SetupExistingExam(TestDataBuilders.ValidExam(examId: request.ExamID, classId: 10));
            SetupNoExistingGradesForExam(request.ExamID);
            _studentGradeDataMock.Setup(d => d.AddStudentGradeAsync(request)).ReturnsAsync(0);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AddStudentGradeAsync(request));
        }

        [Fact]
        public async Task AddStudentGradeAsync_ReturnsNewId_WhenRequestIsValid()
        {
            var request = TestDataBuilders.ValidCreateStudentGradeRequest(grade: 75);
            SetupExistingStudent(TestDataBuilders.ValidStudent(studentId: request.StudentID, classId: 10));
            SetupExistingExam(TestDataBuilders.ValidExam(examId: request.ExamID, classId: 10, totalMarks: 100));
            SetupNoExistingGradesForExam(request.ExamID);
            _studentGradeDataMock.Setup(d => d.AddStudentGradeAsync(request)).ReturnsAsync(42);

            int newId = await _sut.AddStudentGradeAsync(request);

            Assert.Equal(42, newId);
        }
        #endregion

        #region UpdateStudentGradeAsync
        [Fact]
        public async Task UpdateStudentGradeAsync_Throws_WhenGradeExceedsTotalMarks()
        {
            int studentGradeId = 1;
            var request = TestDataBuilders.ValidUpdateStudentGradeRequest(grade: 150);

            var currentGrade = MakeStudentGradeResponse(studentGradeId, studentId: 1, examId: 1);
            _studentGradeDataMock.Setup(d => d.GetStudentGradeByIdAsync(studentGradeId)).ReturnsAsync(currentGrade);
            SetupExistingStudent(TestDataBuilders.ValidStudent(studentId: 1, classId: 10));
            SetupExistingExam(TestDataBuilders.ValidExam(examId: 1, classId: 10, totalMarks: 100));

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.UpdateStudentGradeAsync(studentGradeId, request));
        }

        [Fact]
        public async Task UpdateStudentGradeAsync_Throws_WhenStudentGradeDoesNotExist()
        {
            int studentGradeId = 1;
            var request = TestDataBuilders.ValidUpdateStudentGradeRequest();
            _studentGradeDataMock.Setup(d => d.GetStudentGradeByIdAsync(studentGradeId)).ReturnsAsync((StudentGradeResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.UpdateStudentGradeAsync(studentGradeId, request));
        }

        [Fact]
        public async Task UpdateStudentGradeAsync_ReturnsTrue_WhenUpdateSucceeds()
        {
            int studentGradeId = 1;
            var request = TestDataBuilders.ValidUpdateStudentGradeRequest(grade: 80);

            var currentGrade = MakeStudentGradeResponse(studentGradeId, studentId: 1, examId: 1);
            _studentGradeDataMock.Setup(d => d.GetStudentGradeByIdAsync(studentGradeId)).ReturnsAsync(currentGrade);
            SetupExistingStudent(TestDataBuilders.ValidStudent(studentId: 1, classId: 10));
            SetupExistingExam(TestDataBuilders.ValidExam(examId: 1, classId: 10, totalMarks: 100));
            _studentGradeDataMock.Setup(d => d.UpdateStudentGradeAsync(studentGradeId, request)).ReturnsAsync(true);

            bool result = await _sut.UpdateStudentGradeAsync(studentGradeId, request);

            Assert.True(result);
        }
        #endregion

        #region GetStudentGradeByIdAsync
        [Fact]
        public async Task GetStudentGradeByIdAsync_Throws_WhenNotFound()
        {
            _studentGradeDataMock.Setup(d => d.GetStudentGradeByIdAsync(1)).ReturnsAsync((StudentGradeResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetStudentGradeByIdAsync(1));
        }

        [Fact]
        public async Task GetStudentGradeByIdAsync_ReturnsGrade_WhenFound()
        {
            var grade = MakeStudentGradeResponse(1, studentId: 1, examId: 1);
            _studentGradeDataMock.Setup(d => d.GetStudentGradeByIdAsync(1)).ReturnsAsync(grade);

            var result = await _sut.GetStudentGradeByIdAsync(1);

            Assert.Equal(grade.StudentGradeID, result.StudentGradeID);
        }
        #endregion

        #region DeleteStudentGradeAsync
        [Fact]
        public async Task DeleteStudentGradeAsync_Throws_WhenGradeDoesNotExist()
        {
            _studentGradeDataMock.Setup(d => d.IsStudentGradeExistAsync(1)).ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.DeleteStudentGradeAsync(1));
        }

        [Fact]
        public async Task DeleteStudentGradeAsync_ReturnsTrue_WhenDeletionSucceeds()
        {
            _studentGradeDataMock.Setup(d => d.IsStudentGradeExistAsync(1)).ReturnsAsync(true);
            _studentGradeDataMock.Setup(d => d.DeleteStudentGradeAsync(1)).ReturnsAsync(true);

            bool result = await _sut.DeleteStudentGradeAsync(1);

            Assert.True(result);
        }
        #endregion

        private static StudentGradeResponse MakeStudentGradeResponse(int studentGradeId, int studentId, int examId) => new()
        {
            StudentGradeID = studentGradeId,
            StudentID = studentId,
            PersonID = 100,
            FirstName = "Ahmed",
            SecondName = "Mohamed",
            ThirdName = "Ali",
            GradeID = 1,
            GradeName = "Grade 1",
            ClassID = 10,
            ClassName = "A",
            AcademicYear = "2025/2026",
            SubjectID = 1,
            SubjectName = "Math",
            ExamID = examId,
            ExamTypeID = 1,
            ExamName = "Midterm",
            ExamDate = DateTime.Today,
            TotalMarks = 100,
            Grade = 50,
            IsAbsent = false
        };
    }
}
