using Moq;
using School.BLL;
using School.DAL.Interfaces;
using School.DTO.AssociationsDTOs.UserRoleDTOs.Responses;
using School.Tests.TestHelpers;
using Xunit;

namespace School.Tests.Services
{
    public class UserRoleServiceTests
    {
        private readonly Mock<IUserRoleData> _userRoleDataMock = new();
        private readonly Mock<IUserData> _userDataMock = new();
        private readonly Mock<IRoleData> _roleDataMock = new();
        private readonly UserRoleService _sut;

        public UserRoleServiceTests()
        {
            _sut = new UserRoleService(
                _userRoleDataMock.Object,
                _userDataMock.Object,
                _roleDataMock.Object);
        }

        #region GetUserRoleAsync
        [Fact]
        public async Task GetUserRoleAsync_Throws_WhenNotFound()
        {
            _userRoleDataMock.Setup(d => d.GetUserRoleAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((UserRoleResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetUserRoleAsync(1, 1));
        }

        [Fact]
        public async Task GetUserRoleAsync_ReturnsUserRole_WhenFound()
        {
            var userRole = TestDataBuilders.ValidUserRole(userId: 1, roleId: 2);
            _userRoleDataMock.Setup(d => d.GetUserRoleAsync(It.IsAny<int>(), 2)).ReturnsAsync(userRole);

            var result = await _sut.GetUserRoleAsync(1, 2);

            Assert.Equal(1, result.UserID);
            Assert.Equal(2, result.RoleID);
        }
        #endregion

        #region GetRolesByUserIdAsync
        [Fact]
        public async Task GetRolesByUserIdAsync_Throws_WhenUserDoesNotExist()
        {
            _userDataMock.Setup(d => d.IsUserExistAsync(It.IsAny<int>())).ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetRolesByUserIdAsync(1));
        }

        [Fact]
        public async Task GetRolesByUserIdAsync_ReturnsRoles_WhenUserExists()
        {
            var roles = new List<UserRoleResponse> { TestDataBuilders.ValidUserRole(userId: 1, roleId: 1) };
            _userDataMock.Setup(d => d.IsUserExistAsync(It.IsAny<int>())).ReturnsAsync(true);
            _userRoleDataMock.Setup(d => d.GetRolesByUserIdAsync(It.IsAny<int>())).ReturnsAsync(roles);

            var result = await _sut.GetRolesByUserIdAsync(1);

            Assert.Single(result);
        }
        #endregion

        #region AddUserRoleAsync — Validation
        [Fact]
        public async Task AddUserRoleAsync_Throws_WhenUserRoleIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.AddUserRoleAsync(null!));
        }

        [Fact]
        public async Task AddUserRoleAsync_Throws_WhenUserIdIsInvalid()
        {
            var request = TestDataBuilders.ValidUserRoleRequest(userId: 0);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.AddUserRoleAsync(request));
        }

        [Fact]
        public async Task AddUserRoleAsync_Throws_WhenRoleIdIsInvalid()
        {
            var request = TestDataBuilders.ValidUserRoleRequest(roleId: 0);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.AddUserRoleAsync(request));
        }
        #endregion

        #region AddUserRoleAsync — Business rules
        [Fact]
        public async Task AddUserRoleAsync_Throws_WhenUserDoesNotExist()
        {
            var request = TestDataBuilders.ValidUserRoleRequest(userId: 1, roleId: 1);
            _userDataMock.Setup(d => d.IsUserExistAsync(It.IsAny<int>())).ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.AddUserRoleAsync(request));
        }

        [Fact]
        public async Task AddUserRoleAsync_Throws_WhenRoleDoesNotExist()
        {
            var request = TestDataBuilders.ValidUserRoleRequest(userId: 1, roleId: 1);
            _userDataMock.Setup(d => d.IsUserExistAsync(It.IsAny<int>())).ReturnsAsync(true);
            _roleDataMock.Setup(d => d.IsRoleExistAsync(It.IsAny<int>())).ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.AddUserRoleAsync(request));
        }

        [Fact]
        public async Task AddUserRoleAsync_Throws_WhenRoleIsAlreadyAssignedToUser()
        {
            var request = TestDataBuilders.ValidUserRoleRequest(userId: 1, roleId: 1);
            _userDataMock.Setup(d => d.IsUserExistAsync(It.IsAny<int>())).ReturnsAsync(true);
            _roleDataMock.Setup(d => d.IsRoleExistAsync(It.IsAny<int>())).ReturnsAsync(true);
            _userRoleDataMock.Setup(d => d.IsUserRoleExistAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AddUserRoleAsync(request));
        }

        [Fact]
        public async Task AddUserRoleAsync_ReturnsTrue_WhenAssignedSuccessfully()
        {
            var request = TestDataBuilders.ValidUserRoleRequest(userId: 1, roleId: 1);
            _userDataMock.Setup(d => d.IsUserExistAsync(It.IsAny<int>())).ReturnsAsync(true);
            _roleDataMock.Setup(d => d.IsRoleExistAsync(It.IsAny<int>())).ReturnsAsync(true);
            _userRoleDataMock.Setup(d => d.IsUserRoleExistAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(false);
            _userRoleDataMock.Setup(d => d.AddUserRoleAsync(request)).ReturnsAsync(true);

            var result = await _sut.AddUserRoleAsync(request);

            Assert.True(result);
        }

        [Fact]
        public async Task AddUserRoleAsync_Throws_WhenDataLayerFailsToAssign()
        {
            var request = TestDataBuilders.ValidUserRoleRequest(userId: 1, roleId: 1);
            _userDataMock.Setup(d => d.IsUserExistAsync(It.IsAny<int>())).ReturnsAsync(true);
            _roleDataMock.Setup(d => d.IsRoleExistAsync(It.IsAny<int>())).ReturnsAsync(true);
            _userRoleDataMock.Setup(d => d.IsUserRoleExistAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(false);
            _userRoleDataMock.Setup(d => d.AddUserRoleAsync(request)).ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AddUserRoleAsync(request));
        }
        #endregion

        #region DeleteUserRoleAsync
        [Fact]
        public async Task DeleteUserRoleAsync_Throws_WhenUserIdIsInvalid()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.DeleteUserRoleAsync(0, 1));
        }

        [Fact]
        public async Task DeleteUserRoleAsync_Throws_WhenRoleIdIsInvalid()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.DeleteUserRoleAsync(1, 0));
        }

        [Fact]
        public async Task DeleteUserRoleAsync_Throws_WhenRelationshipDoesNotExist()
        {
            _userRoleDataMock.Setup(d => d.IsUserRoleExistAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.DeleteUserRoleAsync(1, 1));
        }

        [Fact]
        public async Task DeleteUserRoleAsync_ReturnsTrue_WhenDeletionSucceeds()
        {
            _userRoleDataMock.Setup(d => d.IsUserRoleExistAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
            _userRoleDataMock.Setup(d => d.DeleteUserRoleAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);

            var result = await _sut.DeleteUserRoleAsync(1, 1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteUserRoleAsync_Throws_WhenDataLayerFailsToDelete()
        {
            _userRoleDataMock.Setup(d => d.IsUserRoleExistAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
            _userRoleDataMock.Setup(d => d.DeleteUserRoleAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.DeleteUserRoleAsync(1, 1));
        }
        #endregion
    }
}