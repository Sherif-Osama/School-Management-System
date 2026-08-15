using Moq;
using School.BLL;
using School.DAL.Interfaces;
using School.DTO.ClassroomDTOs.Requests;
using School.DTO.ClassroomDTOs.Responses;
using Xunit;
namespace School.Tests.Services
{
    public class ClassroomServiceTests
    {
        private readonly Mock<IClassroomData> _classroomDataMock = new();

        private readonly ClassroomService _sut;

        public ClassroomServiceTests()
        {
            _sut = new ClassroomService(_classroomDataMock.Object);
        }

        #region Helpers
        private static CreateClassroomRequest ValidCreateRequest(string roomName = "Room 101", int capacity = 30) => new()
        {
            RoomName = roomName,
            Capacity = capacity
        };

        private static UpdateClassroomRequest ValidUpdateRequest(string roomName = "Room 101", int capacity = 30) => new()
        {
            RoomName = roomName,
            Capacity = capacity
        };

        private static ClassroomResponse ValidResponse(int classroomId = 1, string roomName = "Room 101", int capacity = 30) => new()
        {
            ClassroomID = classroomId,
            RoomName = roomName,
            Capacity = capacity
        };
        #endregion

        #region Get
        [Fact]
        public async Task GetClassroomByIdAsync_Throws_WhenClassroomIdIsInvalid()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.GetClassroomByIdAsync(0));
        }

        [Fact]
        public async Task GetClassroomByIdAsync_Throws_WhenNotFound()
        {
            _classroomDataMock.Setup(d => d.GetClassroomByIdAsync(1)).ReturnsAsync((ClassroomResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetClassroomByIdAsync(1));
        }

        [Fact]
        public async Task GetClassroomByIdAsync_ReturnsClassroom_WhenFound()
        {
            _classroomDataMock.Setup(d => d.GetClassroomByIdAsync(1)).ReturnsAsync(ValidResponse(1));

            var result = await _sut.GetClassroomByIdAsync(1);

            Assert.Equal(1, result.ClassroomID);
        }

        [Fact]
        public async Task GetClassroomByRoomNameAsync_Throws_WhenRoomNameIsInvalid()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.GetClassroomByRoomNameAsync(""));
        }

        [Fact]
        public async Task GetClassroomByRoomNameAsync_Throws_WhenNotFound()
        {
            _classroomDataMock.Setup(d => d.GetClassroomByRoomNameAsync("Room 101")).ReturnsAsync((ClassroomResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetClassroomByRoomNameAsync("Room 101"));
        }

        [Fact]
        public async Task GetClassroomByRoomNameAsync_ReturnsClassroom_WhenFound()
        {
            _classroomDataMock.Setup(d => d.GetClassroomByRoomNameAsync("Room 101")).ReturnsAsync(ValidResponse(roomName: "Room 101"));

            var result = await _sut.GetClassroomByRoomNameAsync("Room 101");

            Assert.Equal("Room 101", result.RoomName);
        }
        #endregion

        #region Add
        [Fact]
        public async Task AddClassroomAsync_Throws_WhenClassroomIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.AddClassroomAsync(null!));
        }

        [Theory]
        [InlineData("")]
        [InlineData("A")]
        public async Task AddClassroomAsync_Throws_WhenRoomNameIsInvalid(string roomName)
        {
            var request = ValidCreateRequest(roomName: roomName);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.AddClassroomAsync(request));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task AddClassroomAsync_Throws_WhenCapacityIsNotPositive(int capacity)
        {
            var request = ValidCreateRequest(capacity: capacity);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _sut.AddClassroomAsync(request));
        }

        [Fact]
        public async Task AddClassroomAsync_Throws_WhenCapacityExceedsMaximum()
        {
            var request = ValidCreateRequest(capacity: 101);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _sut.AddClassroomAsync(request));
        }

        [Fact]
        public async Task AddClassroomAsync_Throws_WhenRoomNameAlreadyExists()
        {
            var request = ValidCreateRequest(roomName: "Room 101");

            _classroomDataMock.Setup(d => d.GetClassroomByRoomNameAsync("Room 101")).ReturnsAsync(ValidResponse(5, "Room 101"));

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AddClassroomAsync(request));
        }

        [Fact]
        public async Task AddClassroomAsync_Throws_WhenDataLayerFailsToInsert()
        {
            var request = ValidCreateRequest(roomName: "Room 101");

            _classroomDataMock.Setup(d => d.GetClassroomByRoomNameAsync("Room 101")).ReturnsAsync((ClassroomResponse?)null);
            _classroomDataMock.Setup(d => d.AddClassroomAsync(request)).ReturnsAsync(0);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AddClassroomAsync(request));
        }

        [Fact]
        public async Task AddClassroomAsync_ReturnsNewId_WhenClassroomIsAdded()
        {
            var request = ValidCreateRequest(roomName: "Room 101");

            _classroomDataMock.Setup(d => d.GetClassroomByRoomNameAsync("Room 101")).ReturnsAsync((ClassroomResponse?)null);
            _classroomDataMock.Setup(d => d.AddClassroomAsync(request)).ReturnsAsync(3);

            int result = await _sut.AddClassroomAsync(request);

            Assert.Equal(3, result);
        }
        #endregion

        #region Update
        [Fact]
        public async Task UpdateClassroomAsync_Throws_WhenClassroomIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.UpdateClassroomAsync(1, null!));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task UpdateClassroomAsync_Throws_WhenCapacityIsNotPositive(int capacity)
        {
            var request = ValidUpdateRequest(capacity: capacity);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _sut.UpdateClassroomAsync(1, request));
        }

        [Fact]
        public async Task UpdateClassroomAsync_Throws_WhenClassroomIdIsInvalid()
        {
            var request = ValidUpdateRequest();

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.UpdateClassroomAsync(0, request));
        }

        [Fact]
        public async Task UpdateClassroomAsync_Throws_WhenClassroomDoesNotExist()
        {
            var request = ValidUpdateRequest();

            _classroomDataMock.Setup(d => d.IsClassroomExistAsync(1)).ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.UpdateClassroomAsync(1, request));
        }

        [Fact]
        public async Task UpdateClassroomAsync_Throws_WhenRoomNameBelongsToAnotherClassroom()
        {
            var request = ValidUpdateRequest(roomName: "Room 101");

            _classroomDataMock.Setup(d => d.IsClassroomExistAsync(1)).ReturnsAsync(true);
            _classroomDataMock.Setup(d => d.GetClassroomByRoomNameAsync("Room 101")).ReturnsAsync(ValidResponse(2, "Room 101"));

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.UpdateClassroomAsync(1, request));
        }

        [Fact]
        public async Task UpdateClassroomAsync_AllowsKeepingItsOwnRoomName_WhenExcludedFromUniquenessCheck()
        {
            var request = ValidUpdateRequest(roomName: "Room 101");

            _classroomDataMock.Setup(d => d.IsClassroomExistAsync(1)).ReturnsAsync(true);
            _classroomDataMock.Setup(d => d.GetClassroomByRoomNameAsync("Room 101")).ReturnsAsync(ValidResponse(1, "Room 101"));
            _classroomDataMock.Setup(d => d.UpdateClassroomAsync(1, request)).ReturnsAsync(true);

            bool result = await _sut.UpdateClassroomAsync(1, request);

            Assert.True(result);
        }

        [Fact]
        public async Task UpdateClassroomAsync_Throws_WhenDataLayerFailsToUpdate()
        {
            var request = ValidUpdateRequest(roomName: "Room 101");

            _classroomDataMock.Setup(d => d.IsClassroomExistAsync(1)).ReturnsAsync(true);
            _classroomDataMock.Setup(d => d.GetClassroomByRoomNameAsync("Room 101")).ReturnsAsync((ClassroomResponse?)null);
            _classroomDataMock.Setup(d => d.UpdateClassroomAsync(1, request)).ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.UpdateClassroomAsync(1, request));
        }
        #endregion

        #region Delete
        [Fact]
        public async Task DeleteClassroomAsync_Throws_WhenClassroomIdIsInvalid()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.DeleteClassroomAsync(0));
        }

        [Fact]
        public async Task DeleteClassroomAsync_Throws_WhenClassroomDoesNotExist()
        {
            _classroomDataMock.Setup(d => d.IsClassroomExistAsync(1)).ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.DeleteClassroomAsync(1));
        }

        [Fact]
        public async Task DeleteClassroomAsync_Throws_WhenDataLayerFailsToDelete()
        {
            _classroomDataMock.Setup(d => d.IsClassroomExistAsync(1)).ReturnsAsync(true);
            _classroomDataMock.Setup(d => d.DeleteClassroomAsync(1)).ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.DeleteClassroomAsync(1));
        }

        [Fact]
        public async Task DeleteClassroomAsync_ReturnsTrue_WhenClassroomIsDeleted()
        {
            _classroomDataMock.Setup(d => d.IsClassroomExistAsync(1)).ReturnsAsync(true);
            _classroomDataMock.Setup(d => d.DeleteClassroomAsync(1)).ReturnsAsync(true);

            bool result = await _sut.DeleteClassroomAsync(1);

            Assert.True(result);
        }
        #endregion

        #region IsClassroomExistAsync
        [Fact]
        public async Task IsClassroomExistAsync_Throws_WhenClassroomIdIsInvalid()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.IsClassroomExistAsync(0));
        }

        [Fact]
        public async Task IsClassroomExistAsync_ReturnsValueFromDataLayer()
        {
            _classroomDataMock.Setup(d => d.IsClassroomExistAsync(1)).ReturnsAsync(true);

            bool result = await _sut.IsClassroomExistAsync(1);

            Assert.True(result);
        }
        #endregion
    }
}