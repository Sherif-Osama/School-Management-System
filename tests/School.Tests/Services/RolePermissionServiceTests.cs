using Moq;
using School.BLL;
using School.DAL.Interfaces;
using School.DTO.AssociationsDTOs.RolePermissionDTOs;
using School.Tests.TestHelpers;
using Xunit;

namespace School.Tests.Services
{
    public class RolePermissionServiceTests
    {
        private readonly Mock<IRolePermissionData> _rolePermissionDataMock = new();
        private readonly Mock<IRoleData> _roleDataMock = new();
        private readonly Mock<IPermissionData> _permissionDataMock = new();

        private readonly RolePermissionService _sut;

        public RolePermissionServiceTests()
        {
            _sut = new RolePermissionService(
                _rolePermissionDataMock.Object,
                _roleDataMock.Object,
                _permissionDataMock.Object);
        }

        #region GetAllRolePermissionsAsync

        [Fact]
        public async Task GetAllRolePermissionsAsync_ReturnsRolePermissions()
        {
            var rolePermissions = new List<RolePermissionResponse>
            {
                TestDataBuilders.ValidRolePermission(
                    roleId: 1,
                    permissionId: 1),

                TestDataBuilders.ValidRolePermission(
                    roleId: 2,
                    permissionId: 2)
            };

            _rolePermissionDataMock
                .Setup(d => d.GetAllRolePermissionsAsync())
                .ReturnsAsync(rolePermissions);

            var result = await _sut.GetAllRolePermissionsAsync();

            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].RoleID);
            Assert.Equal(2, result[1].RoleID);
        }

        #endregion

        #region GetRolePermissionAsync

        [Fact]
        public async Task GetRolePermissionAsync_Throws_WhenNotFound()
        {
            _rolePermissionDataMock
                .Setup(d => d.GetRolePermissionAsync(1, 2))
                .ReturnsAsync((RolePermissionResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.GetRolePermissionAsync(1, 2));
        }

        [Fact]
        public async Task GetRolePermissionAsync_ReturnsRolePermission_WhenFound()
        {
            var rolePermission = TestDataBuilders.ValidRolePermission(
                roleId: 1,
                permissionId: 2);

            _rolePermissionDataMock
                .Setup(d => d.GetRolePermissionAsync(1, 2))
                .ReturnsAsync(rolePermission);

            var result = await _sut.GetRolePermissionAsync(1, 2);

            Assert.Equal(1, result.RoleID);
            Assert.Equal(2, result.PermissionID);
            Assert.Equal(rolePermission.RoleName, result.RoleName);
            Assert.Equal(rolePermission.PermissionName, result.PermissionName);
        }

        #endregion

        #region GetPermissionsByRoleIdAsync

        [Fact]
        public async Task GetPermissionsByRoleIdAsync_Throws_WhenRoleDoesNotExist()
        {
            _roleDataMock
                .Setup(d => d.IsRoleExistAsync(1))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.GetPermissionsByRoleIdAsync(1));
        }

        [Fact]
        public async Task GetPermissionsByRoleIdAsync_ReturnsPermissions_WhenRoleExists()
        {
            var permissions = new List<RolePermissionResponse>
            {
                TestDataBuilders.ValidRolePermission(
                    roleId: 1,
                    permissionId: 1),

                TestDataBuilders.ValidRolePermission(
                    roleId: 1,
                    permissionId: 2)
            };

            _roleDataMock
                .Setup(d => d.IsRoleExistAsync(1))
                .ReturnsAsync(true);

            _rolePermissionDataMock
                .Setup(d => d.GetPermissionsByRoleIdAsync(1))
                .ReturnsAsync(permissions);

            var result = await _sut.GetPermissionsByRoleIdAsync(1);

            Assert.Equal(2, result.Count);
            Assert.All(result, permission => Assert.Equal(1, permission.RoleID));
        }

        #endregion

        #region AddRolePermissionAsync

        [Fact]
        public async Task AddRolePermissionAsync_Throws_WhenRolePermissionIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _sut.AddRolePermissionAsync(null!));
        }

        [Fact]
        public async Task AddRolePermissionAsync_Throws_WhenRoleDoesNotExist()
        {
            var request = TestDataBuilders.ValidRolePermissionRequest(
                roleId: 1,
                permissionId: 2);

            _roleDataMock
                .Setup(d => d.IsRoleExistAsync(1))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.AddRolePermissionAsync(request));
        }

        [Fact]
        public async Task AddRolePermissionAsync_Throws_WhenPermissionDoesNotExist()
        {
            var request = TestDataBuilders.ValidRolePermissionRequest(
                roleId: 1,
                permissionId: 2);

            _roleDataMock
                .Setup(d => d.IsRoleExistAsync(1))
                .ReturnsAsync(true);

            _permissionDataMock
                .Setup(d => d.IsPermissionExistAsync(2))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.AddRolePermissionAsync(request));
        }

        [Fact]
        public async Task AddRolePermissionAsync_Throws_WhenPermissionIsAlreadyAssigned()
        {
            var request = TestDataBuilders.ValidRolePermissionRequest(
                roleId: 1,
                permissionId: 2);

            _roleDataMock
                .Setup(d => d.IsRoleExistAsync(1))
                .ReturnsAsync(true);

            _permissionDataMock
                .Setup(d => d.IsPermissionExistAsync(2))
                .ReturnsAsync(true);

            _rolePermissionDataMock
                .Setup(d => d.IsRolePermissionExistAsync(1, 2))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.AddRolePermissionAsync(request));
        }

        [Fact]
        public async Task AddRolePermissionAsync_ReturnsTrue_WhenAssignedSuccessfully()
        {
            var request = TestDataBuilders.ValidRolePermissionRequest(
                roleId: 1,
                permissionId: 2);

            _roleDataMock
                .Setup(d => d.IsRoleExistAsync(1))
                .ReturnsAsync(true);

            _permissionDataMock
                .Setup(d => d.IsPermissionExistAsync(2))
                .ReturnsAsync(true);

            _rolePermissionDataMock
                .Setup(d => d.IsRolePermissionExistAsync(1, 2))
                .ReturnsAsync(false);

            _rolePermissionDataMock
                .Setup(d => d.AddRolePermissionAsync(request))
                .ReturnsAsync(true);

            var result = await _sut.AddRolePermissionAsync(request);

            Assert.True(result);
        }

        [Fact]
        public async Task AddRolePermissionAsync_Throws_WhenDataLayerFailsToAssign()
        {
            var request = TestDataBuilders.ValidRolePermissionRequest(
                roleId: 1,
                permissionId: 2);

            _roleDataMock
                .Setup(d => d.IsRoleExistAsync(1))
                .ReturnsAsync(true);

            _permissionDataMock
                .Setup(d => d.IsPermissionExistAsync(2))
                .ReturnsAsync(true);

            _rolePermissionDataMock
                .Setup(d => d.IsRolePermissionExistAsync(1, 2))
                .ReturnsAsync(false);

            _rolePermissionDataMock
                .Setup(d => d.AddRolePermissionAsync(request))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.AddRolePermissionAsync(request));
        }

        #endregion

        #region DeleteRolePermissionAsync

        [Fact]
        public async Task DeleteRolePermissionAsync_Throws_WhenRelationshipDoesNotExist()
        {
            _rolePermissionDataMock
                .Setup(d => d.IsRolePermissionExistAsync(1, 2))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.DeleteRolePermissionAsync(1, 2));
        }

        [Fact]
        public async Task DeleteRolePermissionAsync_ReturnsTrue_WhenDeletionSucceeds()
        {
            _rolePermissionDataMock
                .Setup(d => d.IsRolePermissionExistAsync(1, 2))
                .ReturnsAsync(true);

            _rolePermissionDataMock
                .Setup(d => d.DeleteRolePermissionAsync(1, 2))
                .ReturnsAsync(true);

            var result = await _sut.DeleteRolePermissionAsync(1, 2);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteRolePermissionAsync_Throws_WhenDataLayerFailsToDelete()
        {
            _rolePermissionDataMock
                .Setup(d => d.IsRolePermissionExistAsync(1, 2))
                .ReturnsAsync(true);

            _rolePermissionDataMock
                .Setup(d => d.DeleteRolePermissionAsync(1, 2))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.DeleteRolePermissionAsync(1, 2));
        }

        #endregion
    }
}