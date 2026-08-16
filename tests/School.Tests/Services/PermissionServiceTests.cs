using Moq;
using School.BLL;
using School.DAL.Interfaces;
using School.DTO.PermissionDTOs.Responses;
using School.Tests.TestHelpers;
using Xunit;

namespace School.Tests.Services
{
    public class PermissionServiceTests
    {
        private readonly Mock<IPermissionData> _permissionDataMock = new();

        private readonly PermissionService _sut;

        public PermissionServiceTests()
        {
            _sut = new PermissionService(_permissionDataMock.Object);
        }

        #region Get

        [Fact]
        public async Task GetPermissionByIdAsync_ReturnsPermission_WhenFound()
        {
            var permission = TestDataBuilders.ValidPermission(3);

            _permissionDataMock
                .Setup(d => d.GetPermissionByIdAsync(3))
                .ReturnsAsync(permission);

            var result = await _sut.GetPermissionByIdAsync(3);

            Assert.Equal(3, result.PermissionID);
        }

        [Fact]
        public async Task GetPermissionByIdAsync_Throws_WhenNotFound()
        {
            _permissionDataMock
                .Setup(d => d.GetPermissionByIdAsync(1))
                .ReturnsAsync((PermissionResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.GetPermissionByIdAsync(1));
        }

        [Fact]
        public async Task GetPermissionByNameAsync_ReturnsPermission_WhenFound()
        {
            var permission = TestDataBuilders.ValidPermission();

            _permissionDataMock
                .Setup(d => d.GetPermissionByNameAsync("Students.View"))
                .ReturnsAsync(permission);

            var result =
                await _sut.GetPermissionByNameAsync("Students.View");

            Assert.Equal("Students.View", result.PermissionName);
        }

        [Fact]
        public async Task GetPermissionByNameAsync_Throws_WhenNotFound()
        {
            _permissionDataMock
                .Setup(d => d.GetPermissionByNameAsync("Students.View"))
                .ReturnsAsync((PermissionResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.GetPermissionByNameAsync("Students.View"));
        }

        #endregion

        #region Add

        [Fact]
        public async Task AddPermissionAsync_Throws_WhenPermissionIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _sut.AddPermissionAsync(null!));
        }

        [Fact]
        public async Task AddPermissionAsync_Throws_WhenDescriptionExceedsMaxLength()
        {
            var description = new string('a', 256);

            var permission =
                TestDataBuilders.ValidCreatePermissionRequest(
                    description: description);

            await Assert.ThrowsAsync<ArgumentException>(
                () => _sut.AddPermissionAsync(permission));
        }

        [Fact]
        public async Task AddPermissionAsync_Throws_WhenPermissionAlreadyExists()
        {
            var permission =
                TestDataBuilders.ValidCreatePermissionRequest();

            _permissionDataMock
                .Setup(d => d.GetPermissionByNameAsync(
                    permission.PermissionName))
                .ReturnsAsync(TestDataBuilders.ValidPermission());

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.AddPermissionAsync(permission));
        }

        [Fact]
        public async Task AddPermissionAsync_ReturnsNewId_WhenPermissionIsAdded()
        {
            var permission =
                TestDataBuilders.ValidCreatePermissionRequest();

            _permissionDataMock
                .Setup(d => d.GetPermissionByNameAsync(
                    permission.PermissionName))
                .ReturnsAsync((PermissionResponse?)null);

            _permissionDataMock
                .Setup(d => d.AddPermissionAsync(permission))
                .ReturnsAsync(10);

            var result =
                await _sut.AddPermissionAsync(permission);

            Assert.Equal(10, result);
        }

        [Fact]
        public async Task AddPermissionAsync_Throws_WhenDataLayerFailsToInsert()
        {
            var permission =
                TestDataBuilders.ValidCreatePermissionRequest();

            _permissionDataMock
                .Setup(d => d.GetPermissionByNameAsync(
                    permission.PermissionName))
                .ReturnsAsync((PermissionResponse?)null);

            _permissionDataMock
                .Setup(d => d.AddPermissionAsync(permission))
                .ReturnsAsync(0);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.AddPermissionAsync(permission));
        }

        #endregion

        #region Update

        [Fact]
        public async Task UpdatePermissionAsync_Throws_WhenPermissionIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _sut.UpdatePermissionAsync(1, null!));
        }

        [Fact]
        public async Task UpdatePermissionAsync_Throws_WhenPermissionDoesNotExist()
        {
            var permission =
                TestDataBuilders.ValidUpdatePermissionRequest();

            _permissionDataMock
                .Setup(d => d.IsPermissionExistAsync(1))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.UpdatePermissionAsync(1, permission));
        }

        [Fact]
        public async Task UpdatePermissionAsync_Throws_WhenPermissionNameAlreadyExists()
        {
            var permission =
                TestDataBuilders.ValidUpdatePermissionRequest();

            _permissionDataMock
                .Setup(d => d.IsPermissionExistAsync(1))
                .ReturnsAsync(true);

            _permissionDataMock
                .Setup(d => d.GetPermissionByNameAsync(
                    permission.PermissionName))
                .ReturnsAsync(
                    TestDataBuilders.ValidPermission(
                        permissionId: 2));

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.UpdatePermissionAsync(1, permission));
        }

        [Fact]
        public async Task UpdatePermissionAsync_ReturnsTrue_WhenPermissionIsUpdated()
        {
            var permission =
                TestDataBuilders.ValidUpdatePermissionRequest();

            _permissionDataMock
                .Setup(d => d.IsPermissionExistAsync(1))
                .ReturnsAsync(true);

            _permissionDataMock
                .Setup(d => d.GetPermissionByNameAsync(
                    permission.PermissionName))
                .ReturnsAsync((PermissionResponse?)null);

            _permissionDataMock
                .Setup(d => d.UpdatePermissionAsync(1, permission))
                .ReturnsAsync(true);

            var result =
                await _sut.UpdatePermissionAsync(1, permission);

            Assert.True(result);
        }

        [Fact]
        public async Task UpdatePermissionAsync_ReturnsTrue_WhenFoundPermissionIsTheCurrentPermission()
        {
            var permission =
                TestDataBuilders.ValidUpdatePermissionRequest();

            _permissionDataMock
                .Setup(d => d.IsPermissionExistAsync(1))
                .ReturnsAsync(true);

            _permissionDataMock
                .Setup(d => d.GetPermissionByNameAsync(
                    permission.PermissionName))
                .ReturnsAsync(
                    TestDataBuilders.ValidPermission(
                        permissionId: 1));

            _permissionDataMock
                .Setup(d => d.UpdatePermissionAsync(1, permission))
                .ReturnsAsync(true);

            var result =
                await _sut.UpdatePermissionAsync(1, permission);

            Assert.True(result);
        }

        [Fact]
        public async Task UpdatePermissionAsync_Throws_WhenDataLayerFailsToUpdate()
        {
            var permission =
                TestDataBuilders.ValidUpdatePermissionRequest();

            _permissionDataMock
                .Setup(d => d.IsPermissionExistAsync(1))
                .ReturnsAsync(true);

            _permissionDataMock
                .Setup(d => d.GetPermissionByNameAsync(
                    permission.PermissionName))
                .ReturnsAsync((PermissionResponse?)null);

            _permissionDataMock
                .Setup(d => d.UpdatePermissionAsync(1, permission))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.UpdatePermissionAsync(1, permission));
        }

        #endregion

        #region Delete
        [Fact]
        public async Task DeletePermissionAsync_Throws_WhenPermissionDoesNotExist()
        {
            _permissionDataMock
                .Setup(d => d.IsPermissionExistAsync(1))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.DeletePermissionAsync(1));
        }

        [Fact]
        public async Task DeletePermissionAsync_ReturnsTrue_WhenPermissionIsDeleted()
        {
            _permissionDataMock
                .Setup(d => d.IsPermissionExistAsync(1))
                .ReturnsAsync(true);

            _permissionDataMock
                .Setup(d => d.DeletePermissionAsync(1))
                .ReturnsAsync(true);

            var result =
                await _sut.DeletePermissionAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeletePermissionAsync_Throws_WhenDataLayerFailsToDelete()
        {
            _permissionDataMock
                .Setup(d => d.IsPermissionExistAsync(1))
                .ReturnsAsync(true);

            _permissionDataMock
                .Setup(d => d.DeletePermissionAsync(1))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.DeletePermissionAsync(1));
        }

        #endregion
    }
}