using Moq;
using School.BLL;
using School.DAL.Interfaces;
using School.DTO.ExamTypeDTOs.Responses;
using School.Tests.TestHelpers;
using Xunit;

namespace School.Tests.Services
{
    public class ExamTypeServiceTests
    {
        private readonly Mock<IExamTypeData> _examTypeDataMock = new();

        private readonly ExamTypeService _sut;

        public ExamTypeServiceTests()
        {
            _sut = new ExamTypeService(_examTypeDataMock.Object);
        }

        #region Get

        [Fact]
        public async Task GetExamTypeByIdAsync_ReturnsExamType_WhenFound()
        {
            var examType = TestDataBuilders.ValidExamType(examTypeId: 3);

            _examTypeDataMock
                .Setup(d => d.GetExamTypeByIdAsync(3))
                .ReturnsAsync(examType);

            var result = await _sut.GetExamTypeByIdAsync(3);

            Assert.Equal(3, result.ExamTypeID);
        }

        [Fact]
        public async Task GetExamTypeByIdAsync_Throws_WhenNotFound()
        {
            _examTypeDataMock
                .Setup(d => d.GetExamTypeByIdAsync(1))
                .ReturnsAsync((ExamTypeResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.GetExamTypeByIdAsync(1));
        }

        [Fact]
        public async Task GetExamTypeByNameAsync_ReturnsExamType_WhenFound()
        {
            var examType = TestDataBuilders.ValidExamType();

            _examTypeDataMock
                .Setup(d => d.GetExamTypeByNameAsync("Midterm"))
                .ReturnsAsync(examType);

            var result = await _sut.GetExamTypeByNameAsync("Midterm");

            Assert.Equal("Midterm", result.ExamName);
        }

        [Fact]
        public async Task GetExamTypeByNameAsync_Throws_WhenNotFound()
        {
            _examTypeDataMock
                .Setup(d => d.GetExamTypeByNameAsync("Midterm"))
                .ReturnsAsync((ExamTypeResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.GetExamTypeByNameAsync("Midterm"));
        }

        #endregion

        #region Add

        [Fact]
        public async Task AddExamTypeAsync_Throws_WhenExamTypeIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _sut.AddExamTypeAsync(null!));
        }

        [Theory]
        [InlineData("ab")]
        [InlineData("")]
        [InlineData("   ")]
        public async Task AddExamTypeAsync_Throws_WhenExamNameIsInvalid(
            string examName)
        {
            var examType =
                TestDataBuilders.ValidCreateExamTypeRequest(examName);

            await Assert.ThrowsAsync<ArgumentException>(
                () => _sut.AddExamTypeAsync(examType));
        }

        [Fact]
        public async Task AddExamTypeAsync_Throws_WhenExamTypeAlreadyExists()
        {
            var examType =
                TestDataBuilders.ValidCreateExamTypeRequest();

            _examTypeDataMock
                .Setup(d => d.GetExamTypeByNameAsync(examType.ExamName))
                .ReturnsAsync(TestDataBuilders.ValidExamType());

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.AddExamTypeAsync(examType));
        }

        [Fact]
        public async Task AddExamTypeAsync_ReturnsNewId_WhenExamTypeIsAdded()
        {
            var examType =
                TestDataBuilders.ValidCreateExamTypeRequest();

            _examTypeDataMock
                .Setup(d => d.GetExamTypeByNameAsync(examType.ExamName))
                .ReturnsAsync((ExamTypeResponse?)null);

            _examTypeDataMock
                .Setup(d => d.AddExamTypeAsync(examType))
                .ReturnsAsync(10);

            var result = await _sut.AddExamTypeAsync(examType);

            Assert.Equal(10, result);
        }

        [Fact]
        public async Task AddExamTypeAsync_Throws_WhenDataLayerFailsToInsert()
        {
            var examType =
                TestDataBuilders.ValidCreateExamTypeRequest();

            _examTypeDataMock
                .Setup(d => d.GetExamTypeByNameAsync(examType.ExamName))
                .ReturnsAsync((ExamTypeResponse?)null);

            _examTypeDataMock
                .Setup(d => d.AddExamTypeAsync(examType))
                .ReturnsAsync(0);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.AddExamTypeAsync(examType));
        }

        #endregion

        #region Update

        [Fact]
        public async Task UpdateExamTypeAsync_Throws_WhenExamTypeIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _sut.UpdateExamTypeAsync(1, null!));
        }

        [Fact]
        public async Task UpdateExamTypeAsync_Throws_WhenExamTypeDoesNotExist()
        {
            var examType =
                TestDataBuilders.ValidUpdateExamTypeRequest();

            _examTypeDataMock
                .Setup(d => d.IsExamTypeExistAsync(1))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.UpdateExamTypeAsync(1, examType));
        }

        [Fact]
        public async Task UpdateExamTypeAsync_Throws_WhenExamNameAlreadyExists()
        {
            var examType =
                TestDataBuilders.ValidUpdateExamTypeRequest();

            _examTypeDataMock
                .Setup(d => d.IsExamTypeExistAsync(1))
                .ReturnsAsync(true);

            _examTypeDataMock
                .Setup(d => d.GetExamTypeByNameAsync(examType.ExamName))
                .ReturnsAsync(TestDataBuilders.ValidExamType(
                    examTypeId: 2));

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.UpdateExamTypeAsync(1, examType));
        }

        [Fact]
        public async Task UpdateExamTypeAsync_ReturnsTrue_WhenExamTypeIsUpdated()
        {
            var examType =
                TestDataBuilders.ValidUpdateExamTypeRequest();

            _examTypeDataMock
                .Setup(d => d.IsExamTypeExistAsync(1))
                .ReturnsAsync(true);

            _examTypeDataMock
                .Setup(d => d.GetExamTypeByNameAsync(examType.ExamName))
                .ReturnsAsync((ExamTypeResponse?)null);

            _examTypeDataMock
                .Setup(d => d.UpdateExamTypeAsync(1, examType))
                .ReturnsAsync(true);

            var result =
                await _sut.UpdateExamTypeAsync(1, examType);

            Assert.True(result);
        }

        [Fact]
        public async Task UpdateExamTypeAsync_ReturnsTrue_WhenFoundExamTypeIsTheCurrentExamType()
        {
            var examType =
                TestDataBuilders.ValidUpdateExamTypeRequest();

            _examTypeDataMock
                .Setup(d => d.IsExamTypeExistAsync(1))
                .ReturnsAsync(true);

            _examTypeDataMock
                .Setup(d => d.GetExamTypeByNameAsync(examType.ExamName))
                .ReturnsAsync(
                    TestDataBuilders.ValidExamType(
                        examTypeId: 1));

            _examTypeDataMock
                .Setup(d => d.UpdateExamTypeAsync(1, examType))
                .ReturnsAsync(true);

            var result =
                await _sut.UpdateExamTypeAsync(1, examType);

            Assert.True(result);
        }

        [Fact]
        public async Task UpdateExamTypeAsync_Throws_WhenDataLayerFailsToUpdate()
        {
            var examType =
                TestDataBuilders.ValidUpdateExamTypeRequest();

            _examTypeDataMock
                .Setup(d => d.IsExamTypeExistAsync(1))
                .ReturnsAsync(true);

            _examTypeDataMock
                .Setup(d => d.GetExamTypeByNameAsync(examType.ExamName))
                .ReturnsAsync((ExamTypeResponse?)null);

            _examTypeDataMock
                .Setup(d => d.UpdateExamTypeAsync(1, examType))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.UpdateExamTypeAsync(1, examType));
        }

        #endregion

        #region Delete
        [Fact]
        public async Task DeleteExamTypeAsync_Throws_WhenExamTypeDoesNotExist()
        {
            _examTypeDataMock
                .Setup(d => d.IsExamTypeExistAsync(1))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.DeleteExamTypeAsync(1));
        }

        [Fact]
        public async Task DeleteExamTypeAsync_ReturnsTrue_WhenExamTypeIsDeleted()
        {
            _examTypeDataMock
                .Setup(d => d.IsExamTypeExistAsync(1))
                .ReturnsAsync(true);

            _examTypeDataMock
                .Setup(d => d.DeleteExamTypeAsync(1))
                .ReturnsAsync(true);

            var result = await _sut.DeleteExamTypeAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteExamTypeAsync_Throws_WhenDataLayerFailsToDelete()
        {
            _examTypeDataMock
                .Setup(d => d.IsExamTypeExistAsync(1))
                .ReturnsAsync(true);

            _examTypeDataMock
                .Setup(d => d.DeleteExamTypeAsync(1))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.DeleteExamTypeAsync(1));
        }

        #endregion
    }
}