using Moq;
using School.BLL;
using School.DAL.Interfaces;
using School.DTO.StudentStatusDTOs.Responses;
using School.Tests.TestHelpers;
using Xunit;

namespace School.Tests.Services
{
    public class StudentStatusServiceTests
    {
        private readonly Mock<IStudentStatusData> _studentStatusDataMock = new();

        private readonly StudentStatusService _sut;

        public StudentStatusServiceTests()
        {
            _sut = new StudentStatusService(
                _studentStatusDataMock.Object);
        }

        #region Get
        [Fact]
        public async Task GetStudentStatusByIdAsync_ReturnsStatus_WhenFound()
        {
            var status =
                TestDataBuilders.ValidStudentStatus(
                    statusId: 1);

            _studentStatusDataMock
                .Setup(d => d.GetStudentStatusByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(status);

            var result =
                await _sut.GetStudentStatusByIdAsync(1);

            Assert.Equal(status, result);
        }

        [Fact]
        public async Task GetStudentStatusByIdAsync_Throws_WhenNotFound()
        {
            _studentStatusDataMock
                .Setup(d => d.GetStudentStatusByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((StudentStatusResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.GetStudentStatusByIdAsync(1));
        }

        [Fact]
        public async Task GetStudentStatusByNameAsync_ReturnsStatus_WhenFound()
        {
            var status =
                TestDataBuilders.ValidStudentStatus();

            _studentStatusDataMock
                .Setup(d => d.GetStudentStatusByNameAsync("Active"))
                .ReturnsAsync(status);

            var result =
                await _sut.GetStudentStatusByNameAsync("Active");

            Assert.Equal(status, result);
        }

        [Fact]
        public async Task GetStudentStatusByNameAsync_Throws_WhenNotFound()
        {
            _studentStatusDataMock.Setup(d => d.GetStudentStatusByNameAsync("Active"))
                .ReturnsAsync((StudentStatusResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetStudentStatusByNameAsync("Active"));
        }

        #endregion

        #region Add

        [Fact]
        public async Task AddStudentStatusAsync_Throws_WhenStatusIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.AddStudentStatusAsync(null!));
        }

        [Fact]
        public async Task AddStudentStatusAsync_Throws_WhenStatusAlreadyExists()
        {
            var status = TestDataBuilders.ValidCreateStudentStatusRequest();

            _studentStatusDataMock.Setup(d => d.GetStudentStatusByNameAsync(status.StatusName))
                .ReturnsAsync(TestDataBuilders.ValidStudentStatus(statusName: status.StatusName));

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AddStudentStatusAsync(status));
        }

        [Fact]
        public async Task AddStudentStatusAsync_ReturnsNewId_WhenStatusIsAdded()
        {
            var status =
                TestDataBuilders.ValidCreateStudentStatusRequest();

            _studentStatusDataMock.Setup(d => d.GetStudentStatusByNameAsync(status.StatusName))
                .ReturnsAsync((StudentStatusResponse?)null);

            _studentStatusDataMock.Setup(d => d.AddStudentStatusAsync(status))
                .ReturnsAsync(10);

            var result = await _sut.AddStudentStatusAsync(status);

            Assert.Equal(10, result);
        }

        [Fact]
        public async Task AddStudentStatusAsync_Throws_WhenDataLayerFailsToInsert()
        {
            var status = TestDataBuilders.ValidCreateStudentStatusRequest();

            _studentStatusDataMock.Setup(d => d.GetStudentStatusByNameAsync(status.StatusName))
                .ReturnsAsync((StudentStatusResponse?)null);

            _studentStatusDataMock.Setup(d => d.AddStudentStatusAsync(status))
                .ReturnsAsync(0);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AddStudentStatusAsync(status));
        }

        #endregion

        #region Update

        [Fact]
        public async Task UpdateStudentStatusAsync_Throws_WhenStatusIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.UpdateStudentStatusAsync(1, null!));
        }

        [Fact]
        public async Task UpdateStudentStatusAsync_Throws_WhenStatusDoesNotExist()
        {
            var status = TestDataBuilders.ValidUpdateStudentStatusRequest();

            _studentStatusDataMock.Setup(d => d.IsStudentStatusExistAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.UpdateStudentStatusAsync(1, status));
        }

        [Fact]
        public async Task UpdateStudentStatusAsync_Throws_WhenStatusNameAlreadyExists()
        {
            var status = TestDataBuilders.ValidUpdateStudentStatusRequest();

            _studentStatusDataMock.Setup(d => d.IsStudentStatusExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _studentStatusDataMock.Setup(d => d.GetStudentStatusByNameAsync(status.StatusName))
                .ReturnsAsync(TestDataBuilders.ValidStudentStatus(statusName: status.StatusName));

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.UpdateStudentStatusAsync(1, status));
        }

        [Fact]
        public async Task UpdateStudentStatusAsync_ReturnsTrue_WhenStatusIsUpdated()
        {
            var status = TestDataBuilders.ValidUpdateStudentStatusRequest();

            _studentStatusDataMock.Setup(d => d.IsStudentStatusExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _studentStatusDataMock.Setup(d => d.GetStudentStatusByNameAsync(status.StatusName))
                .ReturnsAsync((StudentStatusResponse?)null);

            _studentStatusDataMock.Setup(d => d.UpdateStudentStatusAsync(It.IsAny<int>(), status))
                .ReturnsAsync(true);

            var result = await _sut.UpdateStudentStatusAsync(1, status);

            Assert.True(result);
        }

        [Fact]
        public async Task UpdateStudentStatusAsync_ReturnsTrue_WhenFoundStatusIsCurrentStatus()
        {
            var status = TestDataBuilders.ValidUpdateStudentStatusRequest();

            _studentStatusDataMock.Setup(d => d.IsStudentStatusExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _studentStatusDataMock.Setup(d => d.GetStudentStatusByNameAsync(status.StatusName))
                .ReturnsAsync(TestDataBuilders.ValidStudentStatus(statusId: 1, statusName: status.StatusName));

            _studentStatusDataMock
                .Setup(d => d.UpdateStudentStatusAsync(It.IsAny<int>(), status))
                .ReturnsAsync(true);

            var result = await _sut.UpdateStudentStatusAsync(1, status);

            Assert.True(result);
        }

        [Fact]
        public async Task UpdateStudentStatusAsync_Throws_WhenDataLayerFailsToUpdate()
        {
            var status =
                TestDataBuilders.ValidUpdateStudentStatusRequest();

            _studentStatusDataMock.Setup(d => d.IsStudentStatusExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _studentStatusDataMock.Setup(d => d.GetStudentStatusByNameAsync(status.StatusName))
                .ReturnsAsync((StudentStatusResponse?)null);

            _studentStatusDataMock
                .Setup(d => d.UpdateStudentStatusAsync(It.IsAny<int>(), status))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.UpdateStudentStatusAsync(1, status));
        }

        #endregion

        #region Delete

        [Fact]
        public async Task DeleteStudentStatusAsync_Throws_WhenStatusDoesNotExist()
        {
            _studentStatusDataMock.Setup(d => d.IsStudentStatusExistAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.DeleteStudentStatusAsync(1));
        }

        [Fact]
        public async Task DeleteStudentStatusAsync_ReturnsTrue_WhenStatusIsDeleted()
        {
            _studentStatusDataMock.Setup(d => d.IsStudentStatusExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _studentStatusDataMock.Setup(d => d.DeleteStudentStatusAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            var result = await _sut.DeleteStudentStatusAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteStudentStatusAsync_Throws_WhenDataLayerFailsToDelete()
        {
            _studentStatusDataMock.Setup(d => d.IsStudentStatusExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _studentStatusDataMock.Setup(d => d.DeleteStudentStatusAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.DeleteStudentStatusAsync(1));
        }

        #endregion
    }
}