using Moq;
using School.BLL;
using School.DAL.Interfaces;
using School.DTO.AttendanceStatusDTOs.Requests;
using School.DTO.AttendanceStatusDTOs.Responses;
using Xunit;
namespace School.Tests.Services
{
    public class AttendanceStatusServiceTests
    {
        private readonly Mock<IAttendanceStatusData> _attendanceStatusDataMock = new();

        private readonly AttendanceStatusService _sut;

        public AttendanceStatusServiceTests()
        {
            _sut = new AttendanceStatusService(_attendanceStatusDataMock.Object);
        }

        #region Helpers
        private static AttendanceStatusRequest ValidRequest(string statusName = "Present") => new()
        {
            StatusName = statusName
        };

        private static AttendanceStatusResponse ValidResponse(int statusId = 1, string statusName = "Present") => new()
        {
            StatusID = statusId,
            StatusName = statusName
        };
        #endregion

        #region Get
        [Fact]
        public async Task GetAttendanceStatusByIdAsync_Throws_WhenStatusIdIsInvalid()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.GetAttendanceStatusByIdAsync(0));
        }

        [Fact]
        public async Task GetAttendanceStatusByIdAsync_Throws_WhenNotFound()
        {
            _attendanceStatusDataMock.Setup(d => d.GetAttendanceStatusByIdAsync(1)).ReturnsAsync((AttendanceStatusResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetAttendanceStatusByIdAsync(1));
        }

        [Fact]
        public async Task GetAttendanceStatusByIdAsync_ReturnsStatus_WhenFound()
        {
            _attendanceStatusDataMock.Setup(d => d.GetAttendanceStatusByIdAsync(1)).ReturnsAsync(ValidResponse(1));

            var result = await _sut.GetAttendanceStatusByIdAsync(1);

            Assert.Equal(1, result.StatusID);
        }

        [Fact]
        public async Task GetAttendanceStatusByNameAsync_Throws_WhenNameIsEmpty()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.GetAttendanceStatusByNameAsync(""));
        }

        [Fact]
        public async Task GetAttendanceStatusByNameAsync_Throws_WhenNotFound()
        {
            _attendanceStatusDataMock.Setup(d => d.GetAttendanceStatusByNameAsync("Present")).ReturnsAsync((AttendanceStatusResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetAttendanceStatusByNameAsync("Present"));
        }

        [Fact]
        public async Task GetAttendanceStatusByNameAsync_ReturnsStatus_WhenFound()
        {
            _attendanceStatusDataMock.Setup(d => d.GetAttendanceStatusByNameAsync("Present")).ReturnsAsync(ValidResponse(statusName: "Present"));

            var result = await _sut.GetAttendanceStatusByNameAsync("Present");

            Assert.Equal("Present", result.StatusName);
        }
        #endregion

        #region Add
        [Fact]
        public async Task AddAttendanceStatusAsync_Throws_WhenStatusIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.AddAttendanceStatusAsync(null!));
        }

        [Theory]
        [InlineData("")]
        [InlineData("A")]
        public async Task AddAttendanceStatusAsync_Throws_WhenStatusNameIsInvalid(string statusName)
        {
            var request = ValidRequest(statusName);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.AddAttendanceStatusAsync(request));
        }

        [Fact]
        public async Task AddAttendanceStatusAsync_Throws_WhenStatusNameAlreadyExists()
        {
            var request = ValidRequest("Present");

            _attendanceStatusDataMock.Setup(d => d.GetAttendanceStatusByNameAsync("Present")).ReturnsAsync(ValidResponse(5, "Present"));

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AddAttendanceStatusAsync(request));
        }

        [Fact]
        public async Task AddAttendanceStatusAsync_Throws_WhenDataLayerFailsToInsert()
        {
            var request = ValidRequest("Present");

            _attendanceStatusDataMock.Setup(d => d.GetAttendanceStatusByNameAsync("Present")).ReturnsAsync((AttendanceStatusResponse?)null);
            _attendanceStatusDataMock.Setup(d => d.AddAttendanceStatusAsync(request)).ReturnsAsync(0);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AddAttendanceStatusAsync(request));
        }

        [Fact]
        public async Task AddAttendanceStatusAsync_ReturnsNewId_WhenStatusIsAdded()
        {
            var request = ValidRequest("Present");

            _attendanceStatusDataMock.Setup(d => d.GetAttendanceStatusByNameAsync("Present")).ReturnsAsync((AttendanceStatusResponse?)null);
            _attendanceStatusDataMock.Setup(d => d.AddAttendanceStatusAsync(request)).ReturnsAsync(3);

            int result = await _sut.AddAttendanceStatusAsync(request);

            Assert.Equal(3, result);
        }
        #endregion

        #region Update
        [Fact]
        public async Task UpdateAttendanceStatusAsync_Throws_WhenStatusIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.UpdateAttendanceStatusAsync(1, null!));
        }

        [Fact]
        public async Task UpdateAttendanceStatusAsync_Throws_WhenStatusIdIsInvalid()
        {
            var request = ValidRequest();

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.UpdateAttendanceStatusAsync(0, request));
        }

        [Fact]
        public async Task UpdateAttendanceStatusAsync_Throws_WhenStatusDoesNotExist()
        {
            var request = ValidRequest();

            _attendanceStatusDataMock.Setup(d => d.IsAttendanceStatusExistAsync(1)).ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.UpdateAttendanceStatusAsync(1, request));
        }

        [Fact]
        public async Task UpdateAttendanceStatusAsync_Throws_WhenNameBelongsToAnotherStatus()
        {
            var request = ValidRequest("Present");

            _attendanceStatusDataMock.Setup(d => d.IsAttendanceStatusExistAsync(1)).ReturnsAsync(true);
            _attendanceStatusDataMock.Setup(d => d.GetAttendanceStatusByNameAsync("Present")).ReturnsAsync(ValidResponse(2, "Present"));

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.UpdateAttendanceStatusAsync(1, request));
        }

        [Fact]
        public async Task UpdateAttendanceStatusAsync_AllowsKeepingItsOwnName_WhenExcludedFromUniquenessCheck()
        {
            var request = ValidRequest("Present");

            _attendanceStatusDataMock.Setup(d => d.IsAttendanceStatusExistAsync(1)).ReturnsAsync(true);
            _attendanceStatusDataMock.Setup(d => d.GetAttendanceStatusByNameAsync("Present")).ReturnsAsync(ValidResponse(1, "Present"));
            _attendanceStatusDataMock.Setup(d => d.UpdateAttendanceStatusAsync(1, request)).ReturnsAsync(true);

            bool result = await _sut.UpdateAttendanceStatusAsync(1, request);

            Assert.True(result);
        }

        [Fact]
        public async Task UpdateAttendanceStatusAsync_Throws_WhenDataLayerFailsToUpdate()
        {
            var request = ValidRequest("Present");

            _attendanceStatusDataMock.Setup(d => d.IsAttendanceStatusExistAsync(1)).ReturnsAsync(true);
            _attendanceStatusDataMock.Setup(d => d.GetAttendanceStatusByNameAsync("Present")).ReturnsAsync((AttendanceStatusResponse?)null);
            _attendanceStatusDataMock.Setup(d => d.UpdateAttendanceStatusAsync(1, request)).ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.UpdateAttendanceStatusAsync(1, request));
        }
        #endregion

        #region Delete
        [Fact]
        public async Task DeleteAttendanceStatusAsync_Throws_WhenStatusIdIsInvalid()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.DeleteAttendanceStatusAsync(0));
        }

        [Fact]
        public async Task DeleteAttendanceStatusAsync_Throws_WhenStatusDoesNotExist()
        {
            _attendanceStatusDataMock.Setup(d => d.IsAttendanceStatusExistAsync(1)).ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.DeleteAttendanceStatusAsync(1));
        }

        [Fact]
        public async Task DeleteAttendanceStatusAsync_Throws_WhenDataLayerFailsToDelete()
        {
            _attendanceStatusDataMock.Setup(d => d.IsAttendanceStatusExistAsync(1)).ReturnsAsync(true);
            _attendanceStatusDataMock.Setup(d => d.DeleteAttendanceStatusAsync(1)).ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.DeleteAttendanceStatusAsync(1));
        }

        [Fact]
        public async Task DeleteAttendanceStatusAsync_ReturnsTrue_WhenStatusIsDeleted()
        {
            _attendanceStatusDataMock.Setup(d => d.IsAttendanceStatusExistAsync(1)).ReturnsAsync(true);
            _attendanceStatusDataMock.Setup(d => d.DeleteAttendanceStatusAsync(1)).ReturnsAsync(true);

            bool result = await _sut.DeleteAttendanceStatusAsync(1);

            Assert.True(result);
        }
        #endregion
    }
}