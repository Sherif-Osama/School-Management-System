using Moq;
using School.BLL;
using School.DAL.Interfaces;
using School.DTO.AssociationsDTOs.ClassSubjectDTOs.Responses;
using School.DTO.ExamDTOs;
using School.Tests.TestHelpers;
using Xunit;

namespace School.Tests.Services
{
    public class ExamServiceTests
    {
        private readonly Mock<IExamData> _examDataMock = new();
        private readonly Mock<IClassSubjectData> _classSubjectDataMock = new();
        private readonly Mock<IExamTypeData> _examTypeDataMock = new();
        private readonly Mock<IClassData> _classDataMock = new();

        private readonly ExamService _sut;

        public ExamServiceTests()
        {
            _sut = new ExamService(
                _examDataMock.Object,
                _classSubjectDataMock.Object,
                _examTypeDataMock.Object,
                _classDataMock.Object);
        }

        #region Helpers

        private void SetupAddHappyPath()
        {
            var classSubject =
                TestDataBuilders.ValidClassSubject();

            _classSubjectDataMock
                .Setup(d => d.GetClassSubjectByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(classSubject);

            _examTypeDataMock
                .Setup(d => d.IsExamTypeExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _examDataMock
                .Setup(d => d.IsExamDuplicate(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int?>()))
                .ReturnsAsync(false);

            _classDataMock
                .Setup(d => d.GetClassByIdAsync(classSubject.ClassID))
                .ReturnsAsync(TestDataBuilders.ValidClass());
        }

        private void SetupUpdateHappyPath(int examId)
        {
            var classSubject =
                TestDataBuilders.ValidClassSubject();

            _examDataMock
                .Setup(d => d.IsExamExistAsync(examId))
                .ReturnsAsync(true);

            _classSubjectDataMock
                .Setup(d => d.GetClassSubjectByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(classSubject);

            _examTypeDataMock
                .Setup(d => d.IsExamTypeExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _examDataMock
                .Setup(d => d.IsExamDuplicate(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int?>()))
                .ReturnsAsync(false);

            _classDataMock
                .Setup(d => d.GetClassByIdAsync(classSubject.ClassID))
                .ReturnsAsync(TestDataBuilders.ValidClass());
        }

        #endregion

        #region Get

        [Fact]
        public async Task GetExamByIdAsync_ReturnsExam_WhenFound()
        {
            var exam = TestDataBuilders.ValidExam(examId: 3);

            _examDataMock
                .Setup(d => d.GetExamByIdAsync(3))
                .ReturnsAsync(exam);

            var result = await _sut.GetExamByIdAsync(3);

            Assert.Equal(3, result.ExamID);
        }

        [Fact]
        public async Task GetExamByIdAsync_Throws_WhenNotFound()
        {
            _examDataMock
                .Setup(d => d.GetExamByIdAsync(1))
                .ReturnsAsync((ExamResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.GetExamByIdAsync(1));
        }

        #endregion

        #region Add

        [Fact]
        public async Task AddExamAsync_Throws_WhenExamIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _sut.AddExamAsync(null!));
        }

        [Fact]
        public async Task AddExamAsync_Throws_WhenExamDateIsDefault()
        {
            var exam = TestDataBuilders.ValidCreateExamRequest(
                examDate: DateOnly.MinValue);

            await Assert.ThrowsAsync<ArgumentException>(
                () => _sut.AddExamAsync(exam));
        }

        [Fact]
        public async Task AddExamAsync_Throws_WhenExamDateIsTooEarly()
        {
            var exam = TestDataBuilders.ValidCreateExamRequest(
                examDate: new DateOnly(1999, 12, 31));

            await Assert.ThrowsAsync<ArgumentException>(
                () => _sut.AddExamAsync(exam));
        }

        [Fact]
        public async Task AddExamAsync_Throws_WhenExamDateIsMoreThanTwoYearsInFuture()
        {
            var exam = TestDataBuilders.ValidCreateExamRequest(
                examDate: DateOnly.FromDateTime(DateTime.Today.AddYears(2).AddDays(1)));

            await Assert.ThrowsAsync<ArgumentException>(
                () => _sut.AddExamAsync(exam));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1001)]
        public async Task AddExamAsync_Throws_WhenTotalMarksIsOutOfRange(
            decimal totalMarks)
        {
            var exam = TestDataBuilders.ValidCreateExamRequest(
                totalMarks: totalMarks);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                () => _sut.AddExamAsync(exam));
        }

        [Fact]
        public async Task AddExamAsync_Throws_WhenTotalMarksHasMoreThanTwoDecimalPlaces()
        {
            var exam = TestDataBuilders.ValidCreateExamRequest(
                totalMarks: 100.123m);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                () => _sut.AddExamAsync(exam));
        }

        [Fact]
        public async Task AddExamAsync_Throws_WhenClassSubjectDoesNotExist()
        {
            var exam = TestDataBuilders.ValidCreateExamRequest();

            _classSubjectDataMock
                .Setup(d => d.GetClassSubjectByIdAsync(exam.ClassSubjectID))
                .ReturnsAsync((ClassSubjectResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.AddExamAsync(exam));
        }

        [Fact]
        public async Task AddExamAsync_Throws_WhenExamTypeDoesNotExist()
        {
            var exam = TestDataBuilders.ValidCreateExamRequest();

            _classSubjectDataMock
                .Setup(d => d.GetClassSubjectByIdAsync(exam.ClassSubjectID))
                .ReturnsAsync(TestDataBuilders.ValidClassSubject());

            _examTypeDataMock
                .Setup(d => d.IsExamTypeExistAsync(exam.ExamTypeID))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.AddExamAsync(exam));
        }

        [Fact]
        public async Task AddExamAsync_Throws_WhenExamAlreadyExists()
        {
            var exam = TestDataBuilders.ValidCreateExamRequest();

            SetupAddHappyPath();

            _examDataMock
                .Setup(d => d.IsExamDuplicate(
                    exam.ClassSubjectID,
                    exam.ExamTypeID,
                    null))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.AddExamAsync(exam));
        }

        [Fact]
        public async Task AddExamAsync_Throws_WhenClassDoesNotExist()
        {
            var exam = TestDataBuilders.ValidCreateExamRequest();

            _classSubjectDataMock
                .Setup(d => d.GetClassSubjectByIdAsync(exam.ClassSubjectID))
                .ReturnsAsync(TestDataBuilders.ValidClassSubject());

            _examTypeDataMock
                .Setup(d => d.IsExamTypeExistAsync(exam.ExamTypeID))
                .ReturnsAsync(true);

            _examDataMock
                .Setup(d => d.IsExamDuplicate(
                    exam.ClassSubjectID,
                    exam.ExamTypeID,
                    null))
                .ReturnsAsync(false);

            _classDataMock
                .Setup(d => d.GetClassByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((School.DTO.ClassesDTOs.Responses.ClassResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.AddExamAsync(exam));
        }

        [Fact]
        public async Task AddExamAsync_Throws_WhenClassIsInactive()
        {
            var exam = TestDataBuilders.ValidCreateExamRequest();

            _classSubjectDataMock
                .Setup(d => d.GetClassSubjectByIdAsync(exam.ClassSubjectID))
                .ReturnsAsync(TestDataBuilders.ValidClassSubject());

            _examTypeDataMock
                .Setup(d => d.IsExamTypeExistAsync(exam.ExamTypeID))
                .ReturnsAsync(true);

            _examDataMock
                .Setup(d => d.IsExamDuplicate(
                    exam.ClassSubjectID,
                    exam.ExamTypeID,
                    null))
                .ReturnsAsync(false);

            _classDataMock
                .Setup(d => d.GetClassByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(TestDataBuilders.ValidClass(isActive: false));

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.AddExamAsync(exam));
        }

        [Fact]
        public async Task AddExamAsync_Throws_WhenExamDateIsOutsideAcademicYear()
        {
            var exam = TestDataBuilders.ValidCreateExamRequest(
                examDate: new DateOnly(2025, 8, 31));

            SetupAddHappyPath();

            await Assert.ThrowsAsync<ArgumentException>(
                () => _sut.AddExamAsync(exam));
        }

        [Fact]
        public async Task AddExamAsync_ReturnsNewId_WhenExamIsAdded()
        {
            var exam = TestDataBuilders.ValidCreateExamRequest(
                examDate: DateOnly.FromDateTime(DateTime.Today));

            SetupAddHappyPath();

            _examDataMock
                .Setup(d => d.AddExamAsync(exam))
                .ReturnsAsync(10);

            var result = await _sut.AddExamAsync(exam);

            Assert.Equal(10, result);
        }

        [Fact]
        public async Task AddExamAsync_Throws_WhenDataLayerFailsToInsert()
        {
            var exam = TestDataBuilders.ValidCreateExamRequest(
                examDate: DateOnly.FromDateTime(DateTime.Today));

            SetupAddHappyPath();

            _examDataMock
                .Setup(d => d.AddExamAsync(exam))
                .ReturnsAsync(0);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.AddExamAsync(exam));
        }

        #endregion

        #region Update

        [Fact]
        public async Task UpdateExamAsync_Throws_WhenExamIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _sut.UpdateExamAsync(1, null!));
        }

        [Fact]
        public async Task UpdateExamAsync_Throws_WhenExamDoesNotExist()
        {
            var exam = TestDataBuilders.ValidUpdateExamRequest();

            _examDataMock
                .Setup(d => d.IsExamExistAsync(1))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.UpdateExamAsync(1, exam));
        }

        [Fact]
        public async Task UpdateExamAsync_Throws_WhenClassSubjectDoesNotExist()
        {
            var exam = TestDataBuilders.ValidUpdateExamRequest();

            _examDataMock
                .Setup(d => d.IsExamExistAsync(1))
                .ReturnsAsync(true);

            _classSubjectDataMock
                .Setup(d => d.GetClassSubjectByIdAsync(exam.ClassSubjectID))
                .ReturnsAsync((ClassSubjectResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.UpdateExamAsync(1, exam));
        }

        [Fact]
        public async Task UpdateExamAsync_Throws_WhenExamTypeDoesNotExist()
        {
            var exam = TestDataBuilders.ValidUpdateExamRequest();

            _examDataMock
                .Setup(d => d.IsExamExistAsync(1))
                .ReturnsAsync(true);

            _classSubjectDataMock
                .Setup(d => d.GetClassSubjectByIdAsync(exam.ClassSubjectID))
                .ReturnsAsync(TestDataBuilders.ValidClassSubject());

            _examTypeDataMock
                .Setup(d => d.IsExamTypeExistAsync(exam.ExamTypeID))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.UpdateExamAsync(1, exam));
        }

        [Fact]
        public async Task UpdateExamAsync_Throws_WhenExamAlreadyExists()
        {
            var exam = TestDataBuilders.ValidUpdateExamRequest();

            SetupUpdateHappyPath(1);

            _examDataMock
                .Setup(d => d.IsExamDuplicate(
                    exam.ClassSubjectID,
                    exam.ExamTypeID,
                    1))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.UpdateExamAsync(1, exam));
        }

        [Fact]
        public async Task UpdateExamAsync_DoesNotThrow_WhenDuplicateIsCurrentExam()
        {
            var exam = TestDataBuilders.ValidUpdateExamRequest();

            SetupUpdateHappyPath(1);

            _examDataMock
                .Setup(d => d.IsExamDuplicate(
                    exam.ClassSubjectID,
                    exam.ExamTypeID,
                    1))
                .ReturnsAsync(false);

            _examDataMock
                .Setup(d => d.UpdateExamAsync(1, exam))
                .ReturnsAsync(true);

            var result = await _sut.UpdateExamAsync(1, exam);

            Assert.True(result);
        }

        [Fact]
        public async Task UpdateExamAsync_Throws_WhenClassDoesNotExist()
        {
            var exam = TestDataBuilders.ValidUpdateExamRequest();

            _examDataMock
                .Setup(d => d.IsExamExistAsync(1))
                .ReturnsAsync(true);

            _classSubjectDataMock
                .Setup(d => d.GetClassSubjectByIdAsync(exam.ClassSubjectID))
                .ReturnsAsync(TestDataBuilders.ValidClassSubject());

            _examTypeDataMock
                .Setup(d => d.IsExamTypeExistAsync(exam.ExamTypeID))
                .ReturnsAsync(true);

            _examDataMock
                .Setup(d => d.IsExamDuplicate(
                    exam.ClassSubjectID,
                    exam.ExamTypeID,
                    1))
                .ReturnsAsync(false);

            _classDataMock
                .Setup(d => d.GetClassByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((School.DTO.ClassesDTOs.Responses.ClassResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.UpdateExamAsync(1, exam));
        }

        [Fact]
        public async Task UpdateExamAsync_Throws_WhenClassIsInactive()
        {
            var exam = TestDataBuilders.ValidUpdateExamRequest();

            _examDataMock
                .Setup(d => d.IsExamExistAsync(1))
                .ReturnsAsync(true);

            _classSubjectDataMock
                .Setup(d => d.GetClassSubjectByIdAsync(exam.ClassSubjectID))
                .ReturnsAsync(TestDataBuilders.ValidClassSubject());

            _examTypeDataMock
                .Setup(d => d.IsExamTypeExistAsync(exam.ExamTypeID))
                .ReturnsAsync(true);

            _examDataMock
                .Setup(d => d.IsExamDuplicate(
                    exam.ClassSubjectID,
                    exam.ExamTypeID,
                    1))
                .ReturnsAsync(false);

            _classDataMock
                .Setup(d => d.GetClassByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(TestDataBuilders.ValidClass(isActive: false));

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.UpdateExamAsync(1, exam));
        }

        [Fact]
        public async Task UpdateExamAsync_Throws_WhenExamDateIsOutsideAcademicYear()
        {
            var exam = TestDataBuilders.ValidUpdateExamRequest(
                examDate: new DateOnly(2025, 8, 31));

            SetupUpdateHappyPath(1);

            await Assert.ThrowsAsync<ArgumentException>(
                () => _sut.UpdateExamAsync(1, exam));
        }

        [Fact]
        public async Task UpdateExamAsync_ReturnsTrue_WhenExamIsUpdated()
        {
            var exam = TestDataBuilders.ValidUpdateExamRequest(
                examDate: DateOnly.FromDateTime(DateTime.Today));

            SetupUpdateHappyPath(1);

            _examDataMock
                .Setup(d => d.UpdateExamAsync(1, exam))
                .ReturnsAsync(true);

            var result = await _sut.UpdateExamAsync(1, exam);

            Assert.True(result);
        }

        [Fact]
        public async Task UpdateExamAsync_Throws_WhenDataLayerFailsToUpdate()
        {
            var exam = TestDataBuilders.ValidUpdateExamRequest(
                examDate: DateOnly.FromDateTime(DateTime.Today));

            SetupUpdateHappyPath(1);

            _examDataMock
                .Setup(d => d.UpdateExamAsync(1, exam))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.UpdateExamAsync(1, exam));
        }

        #endregion

        #region Delete
        [Fact]
        public async Task DeleteExamAsync_Throws_WhenExamDoesNotExist()
        {
            _examDataMock
                .Setup(d => d.IsExamExistAsync(1))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.DeleteExamAsync(1));
        }

        [Fact]
        public async Task DeleteExamAsync_ReturnsTrue_WhenExamIsDeleted()
        {
            _examDataMock
                .Setup(d => d.IsExamExistAsync(1))
                .ReturnsAsync(true);

            _examDataMock
                .Setup(d => d.DeleteExamAsync(1))
                .ReturnsAsync(true);

            var result = await _sut.DeleteExamAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteExamAsync_Throws_WhenDataLayerFailsToDelete()
        {
            _examDataMock
                .Setup(d => d.IsExamExistAsync(1))
                .ReturnsAsync(true);

            _examDataMock
                .Setup(d => d.DeleteExamAsync(1))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.DeleteExamAsync(1));
        }

        #endregion
    }
}