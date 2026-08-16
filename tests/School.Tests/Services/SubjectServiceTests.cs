using Moq;
using School.BLL;
using School.DAL.Interfaces;
using School.DTO.SubjectDTOs.Responses;
using School.Tests.TestHelpers;
using Xunit;

namespace School.Tests.Services
{
    public class SubjectServiceTests
    {
        private readonly Mock<ISubjectData> _subjectDataMock = new();

        private readonly SubjectService _sut;

        public SubjectServiceTests()
        {
            _sut = new SubjectService(_subjectDataMock.Object);
        }

        [Fact]
        public async Task GetSubjectByIdAsync_ReturnsSubject_WhenFound()
        {
            var subject = TestDataBuilders.ValidSubject(subjectId: 3);

            _subjectDataMock.Setup(d => d.GetSubjectByIdAsync(It.IsAny<int>())).ReturnsAsync(subject);

            var result = await _sut.GetSubjectByIdAsync(3);

            Assert.Equal(3, result.SubjectID);
        }

        [Fact]
        public async Task GetSubjectByIdAsync_Throws_WhenNotFound()
        {
            _subjectDataMock.Setup(d => d.GetSubjectByIdAsync(It.IsAny<int>())).ReturnsAsync((SubjectResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetSubjectByIdAsync(1));
        }

        [Fact]
        public async Task GetSubjectByNameAsync_ReturnsSubject_WhenFound()
        {
            var subject = TestDataBuilders.ValidSubject();

            _subjectDataMock.Setup(d => d.GetSubjectByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(subject);

            var result =
                await _sut.GetSubjectByNameAsync("Mathematics");

            Assert.Equal("Mathematics", result.SubjectName);
        }

        [Fact]
        public async Task GetSubjectByNameAsync_Throws_WhenNotFound()
        {
            _subjectDataMock
                .Setup(d => d.GetSubjectByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((SubjectResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.GetSubjectByNameAsync("Mathematics"));
        }

        #region Add

        [Fact]
        public async Task AddSubjectAsync_Throws_WhenSubjectIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _sut.AddSubjectAsync(null!));
        }

        [Fact]
        public async Task AddSubjectAsync_Throws_WhenSubjectAlreadyExists()
        {
            var subject = TestDataBuilders.ValidCreateSubjectRequest();

            _subjectDataMock.Setup(d => d.GetSubjectByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(TestDataBuilders.ValidSubject(subjectName: subject.SubjectName));

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AddSubjectAsync(subject));
        }

        [Fact]
        public async Task AddSubjectAsync_ReturnsNewId_WhenSubjectIsAdded()
        {
            var subject = TestDataBuilders.ValidCreateSubjectRequest();

            _subjectDataMock.Setup(d => d.GetSubjectByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((SubjectResponse?)null);

            _subjectDataMock.Setup(d => d.AddSubjectAsync(subject))
                .ReturnsAsync(10);

            var result = await _sut.AddSubjectAsync(subject);

            Assert.Equal(10, result);
        }

        [Fact]
        public async Task AddSubjectAsync_Throws_WhenDataLayerFailsToInsert()
        {
            var subject = TestDataBuilders.ValidCreateSubjectRequest();

            _subjectDataMock.Setup(d => d.GetSubjectByNameAsync(It.IsAny<string>())).ReturnsAsync((SubjectResponse?)null);

            _subjectDataMock.Setup(d => d.AddSubjectAsync(subject))
                .ReturnsAsync(0);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.AddSubjectAsync(subject));
        }

        #endregion

        #region Update
        [Fact]
        public async Task UpdateSubjectAsync_Throws_WhenSubjectIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _sut.UpdateSubjectAsync(1, null!));
        }

        [Fact]
        public async Task UpdateSubjectAsync_Throws_WhenSubjectDoesNotExist()
        {
            var subject =
                TestDataBuilders.ValidUpdateSubjectRequest();

            _subjectDataMock
                .Setup(d => d.IsSubjectExistAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.UpdateSubjectAsync(1, subject));
        }

        [Fact]
        public async Task UpdateSubjectAsync_Throws_WhenSubjectNameAlreadyExists()
        {
            var subject =
                TestDataBuilders.ValidUpdateSubjectRequest();

            _subjectDataMock
                .Setup(d => d.IsSubjectExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _subjectDataMock
                .Setup(d => d.GetSubjectByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(
                    TestDataBuilders.ValidSubject(
                        subjectId: 2,
                        subjectName: subject.SubjectName));

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.UpdateSubjectAsync(1, subject));
        }

        [Fact]
        public async Task UpdateSubjectAsync_ReturnsTrue_WhenSubjectIsUpdated()
        {
            var subject =
                TestDataBuilders.ValidUpdateSubjectRequest();

            _subjectDataMock
                .Setup(d => d.IsSubjectExistAsync(1))
                .ReturnsAsync(true);

            _subjectDataMock
                .Setup(d => d.GetSubjectByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((SubjectResponse?)null);

            _subjectDataMock
                .Setup(d => d.UpdateSubjectAsync(1, subject))
                .ReturnsAsync(true);

            var result =
                await _sut.UpdateSubjectAsync(1, subject);

            Assert.True(result);
        }

        [Fact]
        public async Task UpdateSubjectAsync_ReturnsTrue_WhenFoundSubjectIsCurrentSubject()
        {
            var subject =
                TestDataBuilders.ValidUpdateSubjectRequest();

            _subjectDataMock
                .Setup(d => d.IsSubjectExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _subjectDataMock
                .Setup(d => d.GetSubjectByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(
                    TestDataBuilders.ValidSubject(
                        subjectId: 1,
                        subjectName: subject.SubjectName));

            _subjectDataMock
                .Setup(d => d.UpdateSubjectAsync(1, subject))
                .ReturnsAsync(true);

            var result =
                await _sut.UpdateSubjectAsync(1, subject);

            Assert.True(result);
        }

        [Fact]
        public async Task UpdateSubjectAsync_Throws_WhenDataLayerFailsToUpdate()
        {
            var subject =
                TestDataBuilders.ValidUpdateSubjectRequest();

            _subjectDataMock
                .Setup(d => d.IsSubjectExistAsync(1))
                .ReturnsAsync(true);

            _subjectDataMock
                .Setup(d => d.GetSubjectByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((SubjectResponse?)null);

            _subjectDataMock
                .Setup(d => d.UpdateSubjectAsync(1, subject))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.UpdateSubjectAsync(1, subject));
        }
        #endregion

        #region Delete
        [Fact]
        public async Task DeleteSubjectAsync_Throws_WhenSubjectDoesNotExist()
        {
            _subjectDataMock
                .Setup(d => d.IsSubjectExistAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.DeleteSubjectAsync(1));
        }

        [Fact]
        public async Task DeleteSubjectAsync_ReturnsTrue_WhenSubjectIsDeleted()
        {
            _subjectDataMock
                .Setup(d => d.IsSubjectExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _subjectDataMock
                .Setup(d => d.DeleteSubjectAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            var result =
                await _sut.DeleteSubjectAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteSubjectAsync_Throws_WhenDataLayerFailsToDelete()
        {
            _subjectDataMock
                .Setup(d => d.IsSubjectExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _subjectDataMock
                .Setup(d => d.DeleteSubjectAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.DeleteSubjectAsync(1));
        }
        #endregion

        #region IsSubjectExistAsync

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task IsSubjectExistAsync_ReturnsExpectedValue(bool expected)
        {
            _subjectDataMock.Setup(d => d.IsSubjectExistAsync(It.IsAny<int>())).ReturnsAsync(expected);

            var result =
                await _sut.IsSubjectExistAsync(1);

            Assert.Equal(expected, result);
        }
        #endregion
    }
}