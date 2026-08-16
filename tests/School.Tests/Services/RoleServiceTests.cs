using Moq;
using School.BLL;
using School.DAL.Interfaces;
using School.DTO.RoleDTOs.Responses;
using School.Tests.TestHelpers;
using Xunit;

namespace School.Tests.Services
{
    public class RoleServiceTests
    {
        private readonly Mock<IRoleData> _roleDataMock = new();

        private readonly RoleService _sut;

        public RoleServiceTests()
        {
            _sut = new RoleService(_roleDataMock.Object);
        }

        #region Get

        [Fact]
        public async Task GetRoleByIdAsync_ReturnsRole_WhenFound()
        {
            var role = TestDataBuilders.ValidRole(roleId: 5);

            _roleDataMock.Setup(d => d.GetRoleByIdAsync(5))
                .ReturnsAsync(role);

            var result = await _sut.GetRoleByIdAsync(5);

            Assert.Equal(5, result.RoleID);
            Assert.Equal(role.RoleName, result.RoleName);
        }

        [Fact]
        public async Task GetRoleByIdAsync_Throws_WhenNotFound()
        {
            _roleDataMock.Setup(d => d.GetRoleByIdAsync(5))
                .ReturnsAsync((RoleResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetRoleByIdAsync(5));
        }

        [Fact]
        public async Task GetRoleByNameAsync_ReturnsRole_WhenFound()
        {
            var role = TestDataBuilders.ValidRole(roleId: 5, roleName: "Teacher");

            _roleDataMock.Setup(d => d.GetRoleByNameAsync("Teacher"))
                .ReturnsAsync(role);

            var result = await _sut.GetRoleByNameAsync("Teacher");

            Assert.Equal(5, result.RoleID);
            Assert.Equal("Teacher", result.RoleName);
        }

        [Fact]
        public async Task GetRoleByNameAsync_Throws_WhenNotFound()
        {
            _roleDataMock.Setup(d => d.GetRoleByNameAsync(It.IsAny<string>())).ReturnsAsync((RoleResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetRoleByNameAsync("Teacher"));
        }
        #endregion

        #region Add

        [Fact]
        public async Task AddRoleAsync_Throws_WhenRoleIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.AddRoleAsync(null!));
        }

        [Fact]
        public async Task AddRoleAsync_Throws_WhenDescriptionExceedsMaximumLength()
        {
            var role = TestDataBuilders.ValidCreateRoleRequest(description: new string('a', 256));

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.AddRoleAsync(role));
        }

        [Fact]
        public async Task AddRoleAsync_Throws_WhenRoleAlreadyExists()
        {
            var role = TestDataBuilders.ValidCreateRoleRequest();

            _roleDataMock.Setup(d => d.GetRoleByNameAsync(role.RoleName))
                .ReturnsAsync(TestDataBuilders.ValidRole());

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AddRoleAsync(role));
        }

        [Fact]
        public async Task AddRoleAsync_ReturnsNewId_WhenRoleIsAdded()
        {
            var role = TestDataBuilders.ValidCreateRoleRequest();

            _roleDataMock.Setup(d => d.GetRoleByNameAsync(role.RoleName))
                .ReturnsAsync((RoleResponse?)null);

            _roleDataMock.Setup(d => d.AddRoleAsync(role))
                .ReturnsAsync(10);

            var result = await _sut.AddRoleAsync(role);

            Assert.Equal(10, result);
        }

        [Fact]
        public async Task AddRoleAsync_Throws_WhenDataLayerFailsToInsert()
        {
            var role = TestDataBuilders.ValidCreateRoleRequest();

            _roleDataMock.Setup(d => d.GetRoleByNameAsync(role.RoleName))
                .ReturnsAsync((RoleResponse?)null);

            _roleDataMock.Setup(d => d.AddRoleAsync(role))
                .ReturnsAsync(0);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.AddRoleAsync(role));
        }

        #endregion

        #region Update

        [Fact]
        public async Task UpdateRoleAsync_Throws_WhenRoleIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.UpdateRoleAsync(1, null!));
        }

        [Fact]
        public async Task UpdateRoleAsync_Throws_WhenDescriptionExceedsMaximumLength()
        {
            var role = TestDataBuilders.ValidUpdateRoleRequest(description: new string('a', 256));

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.UpdateRoleAsync(1, role));
        }

        [Fact]
        public async Task UpdateRoleAsync_Throws_WhenRoleDoesNotExist()
        {
            var role = TestDataBuilders.ValidUpdateRoleRequest();

            _roleDataMock.Setup(d => d.IsRoleExistAsync(1))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.UpdateRoleAsync(1, role));
        }

        [Fact]
        public async Task UpdateRoleAsync_Throws_WhenRoleNameAlreadyExists()
        {
            var role = TestDataBuilders.ValidUpdateRoleRequest();

            _roleDataMock.Setup(d => d.IsRoleExistAsync(1))
                .ReturnsAsync(true);

            _roleDataMock.Setup(d => d.GetRoleByNameAsync(role.RoleName))
                .ReturnsAsync(TestDataBuilders.ValidRole(roleId: 2));

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.UpdateRoleAsync(1, role));
        }

        [Fact]
        public async Task UpdateRoleAsync_ReturnsTrue_WhenRoleIsUpdated()
        {
            var role = TestDataBuilders.ValidUpdateRoleRequest();

            _roleDataMock.Setup(d => d.IsRoleExistAsync(1))
                .ReturnsAsync(true);

            _roleDataMock.Setup(d => d.GetRoleByNameAsync(role.RoleName))
                .ReturnsAsync((RoleResponse?)null);

            _roleDataMock.Setup(d => d.UpdateRoleAsync(1, role))
                .ReturnsAsync(true);

            var result = await _sut.UpdateRoleAsync(1, role);

            Assert.True(result);
        }

        [Fact]
        public async Task UpdateRoleAsync_Throws_WhenDataLayerFailsToUpdate()
        {
            var role = TestDataBuilders.ValidUpdateRoleRequest();

            _roleDataMock
                .Setup(d => d.IsRoleExistAsync(1))
                .ReturnsAsync(true);

            _roleDataMock
                .Setup(d => d.GetRoleByNameAsync(role.RoleName))
                .ReturnsAsync((RoleResponse?)null);

            _roleDataMock
                .Setup(d => d.UpdateRoleAsync(1, role))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.UpdateRoleAsync(1, role));
        }

        #endregion

        #region Delete

        [Fact]
        public async Task DeleteRoleAsync_Throws_WhenRoleDoesNotExist()
        {
            _roleDataMock
                .Setup(d => d.IsRoleExistAsync(1))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.DeleteRoleAsync(1));
        }

        [Fact]
        public async Task DeleteRoleAsync_ReturnsTrue_WhenRoleIsDeleted()
        {
            _roleDataMock
                .Setup(d => d.IsRoleExistAsync(1))
                .ReturnsAsync(true);

            _roleDataMock
                .Setup(d => d.DeleteRoleAsync(1))
                .ReturnsAsync(true);

            var result = await _sut.DeleteRoleAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteRoleAsync_Throws_WhenDataLayerFailsToDelete()
        {
            _roleDataMock
                .Setup(d => d.IsRoleExistAsync(1))
                .ReturnsAsync(true);

            _roleDataMock
                .Setup(d => d.DeleteRoleAsync(1))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.DeleteRoleAsync(1));
        }

        #endregion
    }
}