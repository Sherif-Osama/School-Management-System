using Moq;
using School.BLL;
using School.DAL.Interfaces;
using School.DTO.GradesDTOs.Responses;
using School.Tests.TestHelpers;
using Xunit;

namespace School.Tests.Services
{
    public class GradeServiceTests
    {
        private readonly Mock<IGradeData> _gradeDataMock = new();

        private readonly GradeService _sut;

        public GradeServiceTests()
        {
            _sut = new GradeService(_gradeDataMock.Object);
        }

        #region Get

        [Fact]
        public async Task GetGradeByIdAsync_ReturnsGrade_WhenFound()
        {
            var grade = TestDataBuilders.ValidGrade(3);

            _gradeDataMock
                .Setup(d => d.GetGradeByIdAsync(3))
                .ReturnsAsync(grade);

            var result = await _sut.GetGradeByIdAsync(3);

            Assert.Equal(3, result.GradeID);
        }

        [Fact]
        public async Task GetGradeByIdAsync_Throws_WhenNotFound()
        {
            _gradeDataMock
                .Setup(d => d.GetGradeByIdAsync(1))
                .ReturnsAsync((GradeResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.GetGradeByIdAsync(1));
        }

        [Fact]
        public async Task GetGradeByNameAsync_ReturnsGrade_WhenFound()
        {
            var grade = TestDataBuilders.ValidGrade();

            _gradeDataMock
                .Setup(d => d.GetGradeByNameAsync("Grade 1"))
                .ReturnsAsync(grade);

            var result = await _sut.GetGradeByNameAsync("Grade 1");

            Assert.Equal("Grade 1", result.GradeName);
        }

        [Fact]
        public async Task GetGradeByNameAsync_Throws_WhenNotFound()
        {
            _gradeDataMock
                .Setup(d => d.GetGradeByNameAsync("Grade 1"))
                .ReturnsAsync((GradeResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.GetGradeByNameAsync("Grade 1"));
        }

        #endregion

        #region Add

        [Fact]
        public async Task AddGradeAsync_Throws_WhenGradeIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _sut.AddGradeAsync(null!));
        }

        [Fact]
        public async Task AddGradeAsync_Throws_WhenGradeAlreadyExists()
        {
            var grade =
                TestDataBuilders.ValidCreateGradeRequest();

            _gradeDataMock
                .Setup(d => d.GetGradeByNameAsync(grade.GradeName))
                .ReturnsAsync(TestDataBuilders.ValidGrade());

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.AddGradeAsync(grade));
        }

        [Fact]
        public async Task AddGradeAsync_ReturnsNewId_WhenGradeIsAdded()
        {
            var grade =
                TestDataBuilders.ValidCreateGradeRequest();

            _gradeDataMock
                .Setup(d => d.GetGradeByNameAsync(grade.GradeName))
                .ReturnsAsync((GradeResponse?)null);

            _gradeDataMock
                .Setup(d => d.AddGradeAsync(grade))
                .ReturnsAsync(10);

            var result = await _sut.AddGradeAsync(grade);

            Assert.Equal(10, result);
        }

        [Fact]
        public async Task AddGradeAsync_Throws_WhenDataLayerFailsToInsert()
        {
            var grade =
                TestDataBuilders.ValidCreateGradeRequest();

            _gradeDataMock
                .Setup(d => d.GetGradeByNameAsync(grade.GradeName))
                .ReturnsAsync((GradeResponse?)null);

            _gradeDataMock
                .Setup(d => d.AddGradeAsync(grade))
                .ReturnsAsync(0);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.AddGradeAsync(grade));
        }

        #endregion

        #region Update

        [Fact]
        public async Task UpdateGradeAsync_Throws_WhenGradeIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _sut.UpdateGradeAsync(1, null!));
        }

        [Fact]
        public async Task UpdateGradeAsync_Throws_WhenGradeDoesNotExist()
        {
            var grade =
                TestDataBuilders.ValidUpdateGradeRequest();

            _gradeDataMock
                .Setup(d => d.IsGradeExistAsync(1))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.UpdateGradeAsync(1, grade));
        }

        [Fact]
        public async Task UpdateGradeAsync_Throws_WhenGradeNameAlreadyExists()
        {
            var grade =
                TestDataBuilders.ValidUpdateGradeRequest();

            _gradeDataMock
                .Setup(d => d.IsGradeExistAsync(1))
                .ReturnsAsync(true);

            _gradeDataMock
                .Setup(d => d.GetGradeByNameAsync(grade.GradeName))
                .ReturnsAsync(
                    TestDataBuilders.ValidGrade(gradeId: 2));

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.UpdateGradeAsync(1, grade));
        }

        [Fact]
        public async Task UpdateGradeAsync_ReturnsTrue_WhenGradeIsUpdated()
        {
            var grade =
                TestDataBuilders.ValidUpdateGradeRequest();

            _gradeDataMock
                .Setup(d => d.IsGradeExistAsync(1))
                .ReturnsAsync(true);

            _gradeDataMock
                .Setup(d => d.GetGradeByNameAsync(grade.GradeName))
                .ReturnsAsync((GradeResponse?)null);

            _gradeDataMock
                .Setup(d => d.UpdateGradeAsync(1, grade))
                .ReturnsAsync(true);

            var result =
                await _sut.UpdateGradeAsync(1, grade);

            Assert.True(result);
        }

        [Fact]
        public async Task UpdateGradeAsync_ReturnsTrue_WhenFoundGradeIsTheCurrentGrade()
        {
            var grade =
                TestDataBuilders.ValidUpdateGradeRequest();

            _gradeDataMock
                .Setup(d => d.IsGradeExistAsync(1))
                .ReturnsAsync(true);

            _gradeDataMock
                .Setup(d => d.GetGradeByNameAsync(grade.GradeName))
                .ReturnsAsync(
                    TestDataBuilders.ValidGrade(gradeId: 1));

            _gradeDataMock
                .Setup(d => d.UpdateGradeAsync(1, grade))
                .ReturnsAsync(true);

            var result =
                await _sut.UpdateGradeAsync(1, grade);

            Assert.True(result);
        }

        [Fact]
        public async Task UpdateGradeAsync_Throws_WhenDataLayerFailsToUpdate()
        {
            var grade =
                TestDataBuilders.ValidUpdateGradeRequest();

            _gradeDataMock
                .Setup(d => d.IsGradeExistAsync(1))
                .ReturnsAsync(true);

            _gradeDataMock
                .Setup(d => d.GetGradeByNameAsync(grade.GradeName))
                .ReturnsAsync((GradeResponse?)null);

            _gradeDataMock
                .Setup(d => d.UpdateGradeAsync(1, grade))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.UpdateGradeAsync(1, grade));
        }

        #endregion

        #region Delete
        [Fact]
        public async Task DeleteGradeAsync_Throws_WhenGradeDoesNotExist()
        {
            _gradeDataMock
                .Setup(d => d.IsGradeExistAsync(1))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.DeleteGradeAsync(1));
        }

        [Fact]
        public async Task DeleteGradeAsync_ReturnsTrue_WhenGradeIsDeleted()
        {
            _gradeDataMock
                .Setup(d => d.IsGradeExistAsync(1))
                .ReturnsAsync(true);

            _gradeDataMock
                .Setup(d => d.DeleteGradeAsync(1))
                .ReturnsAsync(true);

            var result =
                await _sut.DeleteGradeAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteGradeAsync_Throws_WhenDataLayerFailsToDelete()
        {
            _gradeDataMock
                .Setup(d => d.IsGradeExistAsync(1))
                .ReturnsAsync(true);

            _gradeDataMock
                .Setup(d => d.DeleteGradeAsync(1))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.DeleteGradeAsync(1));
        }

        #endregion

        #region IsGradeExist
        [Fact]
        public async Task IsGradeExistAsync_ReturnsTrue_WhenGradeExists()
        {
            _gradeDataMock
                .Setup(d => d.IsGradeExistAsync(1))
                .ReturnsAsync(true);

            var result = await _sut.IsGradeExistAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task IsGradeExistAsync_ReturnsFalse_WhenGradeDoesNotExist()
        {
            _gradeDataMock
                .Setup(d => d.IsGradeExistAsync(1))
                .ReturnsAsync(false);

            var result = await _sut.IsGradeExistAsync(1);

            Assert.False(result);
        }

        #endregion
    }
}