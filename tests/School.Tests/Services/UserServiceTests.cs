using Moq;
using School.BLL;
using School.BLL.Common;
using School.DAL.Interfaces;
using School.DTO.UserDTOs.Requests;
using School.DTO.UserDTOs.Responses;
using School.Tests.TestHelpers;
using Xunit;

namespace School.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserData> _userDataMock = new();
        private readonly Mock<IPersonData> _personDataMock = new();
        private readonly Mock<IRefreshTokenData> _refreshTokenDataMock = new();
        private readonly UserService _sut;

        public UserServiceTests()
        {
            _sut = new UserService(
                _userDataMock.Object,
                _personDataMock.Object,
                _refreshTokenDataMock.Object);
        }

        #region GetUserByIdAsync
        [Fact]
        public async Task GetUserByIdAsync_ReturnsUser_WhenFound()
        {
            var user = TestDataBuilders.ValidUser(userId: 3);
            _userDataMock.Setup(d => d.GetUserByIdAsync(It.IsAny<int>())).ReturnsAsync(user);

            var result = await _sut.GetUserByIdAsync(3);

            Assert.Equal(3, result.UserID);
        }

        [Fact]
        public async Task GetUserByIdAsync_Throws_WhenNotFound()
        {
            _userDataMock.Setup(d => d.GetUserByIdAsync(It.IsAny<int>())).ReturnsAsync((UserResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetUserByIdAsync(1));
        }
        #endregion

        #region GetUserByUsernameAsync
        [Fact]
        public async Task GetUserByUsernameAsync_Throws_WhenNotFound()
        {
            _userDataMock.Setup(d => d.GetUserByUsernameAsync(It.IsAny<string>())).ReturnsAsync((UserResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetUserByUsernameAsync("ahmed123"));
        }

        [Fact]
        public async Task GetUserByUsernameAsync_ReturnsUser_WhenFound()
        {
            var user = TestDataBuilders.ValidUser(username: "ahmed123");
            _userDataMock.Setup(d => d.GetUserByUsernameAsync("ahmed123")).ReturnsAsync(user);

            var result = await _sut.GetUserByUsernameAsync("ahmed123");

            Assert.Equal("ahmed123", result.Username);
        }
        #endregion

        #region GetUserByPersonIdAsync
        [Fact]
        public async Task GetUserByPersonIdAsync_Throws_WhenPersonDoesNotExist()
        {
            _personDataMock.Setup(d => d.IsPersonExistAsync(It.IsAny<int>())).ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetUserByPersonIdAsync(1));
        }

        [Fact]
        public async Task GetUserByPersonIdAsync_Throws_WhenUserDoesNotExistForPerson()
        {
            _personDataMock.Setup(d => d.IsPersonExistAsync(It.IsAny<int>())).ReturnsAsync(true);
            _userDataMock.Setup(d => d.GetUserByPersonIdAsync(It.IsAny<int>())).ReturnsAsync((UserResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetUserByPersonIdAsync(1));
        }

        [Fact]
        public async Task GetUserByPersonIdAsync_ReturnsUser_WhenFound()
        {
            _personDataMock.Setup(d => d.IsPersonExistAsync(It.IsAny<int>())).ReturnsAsync(true);
            _userDataMock.Setup(d => d.GetUserByPersonIdAsync(It.IsAny<int>())).ReturnsAsync(TestDataBuilders.ValidUser(personId: 1));

            var result = await _sut.GetUserByPersonIdAsync(1);

            Assert.Equal(1, result.PersonID);
        }
        #endregion

        #region AddUserAsync — Validation
        [Fact]
        public async Task AddUserAsync_Throws_WhenUserIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.AddUserAsync(null!));
        }

        [Fact]
        public async Task AddUserAsync_Throws_WhenUsernameIsTooShort()
        {
            var request = TestDataBuilders.ValidCreateUserRequest(username: "ab");

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.AddUserAsync(request));
        }

        [Fact]
        public async Task AddUserAsync_Throws_WhenPasswordIsTooShort()
        {
            var request = TestDataBuilders.ValidCreateUserRequest(password: "123");

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.AddUserAsync(request));
        }
        #endregion

        #region AddUserAsync — Business rules
        [Fact]
        public async Task AddUserAsync_Throws_WhenPersonDoesNotExist()
        {
            var request = TestDataBuilders.ValidCreateUserRequest(personId: 1);
            _personDataMock.Setup(d => d.IsPersonExistAsync(It.IsAny<int>())).ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.AddUserAsync(request));
        }

        [Fact]
        public async Task AddUserAsync_Throws_WhenPersonAlreadyHasAUser()
        {
            var request = TestDataBuilders.ValidCreateUserRequest(personId: 1);
            _personDataMock.Setup(d => d.IsPersonExistAsync(It.IsAny<int>())).ReturnsAsync(true);
            _userDataMock.Setup(d => d.GetUserByPersonIdAsync(It.IsAny<int>())).ReturnsAsync(TestDataBuilders.ValidUser(personId: 1));

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AddUserAsync(request));
        }

        [Fact]
        public async Task AddUserAsync_Throws_WhenUsernameIsAlreadyTaken()
        {
            var request = TestDataBuilders.ValidCreateUserRequest(personId: 1, username: "ahmed123");
            _personDataMock.Setup(d => d.IsPersonExistAsync(It.IsAny<int>())).ReturnsAsync(true);
            _userDataMock.Setup(d => d.GetUserByPersonIdAsync(It.IsAny<int>())).ReturnsAsync((UserResponse?)null);
            _userDataMock.Setup(d => d.GetUserByUsernameAsync("ahmed123")).ReturnsAsync(TestDataBuilders.ValidUser(userId: 9));

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AddUserAsync(request));
        }

        [Fact]
        public async Task AddUserAsync_HashesPassword_BeforeSendingToDataLayer()
        {
            var request = TestDataBuilders.ValidCreateUserRequest(personId: 1, password: "PlainTextP@ss");
            _personDataMock.Setup(d => d.IsPersonExistAsync(It.IsAny<int>())).ReturnsAsync(true);
            _userDataMock.Setup(d => d.GetUserByPersonIdAsync(It.IsAny<int>())).ReturnsAsync((UserResponse?)null);
            _userDataMock.Setup(d => d.GetUserByUsernameAsync(request.Username)).ReturnsAsync((UserResponse?)null);

            string? capturedPasswordSentToDataLayer = null;
            _userDataMock
                .Setup(d => d.AddUserAsync(It.IsAny<CreateUserRequest>()))
                .Callback<CreateUserRequest>(r => capturedPasswordSentToDataLayer = r.Password)
                .ReturnsAsync(1);

            await _sut.AddUserAsync(request);

            Assert.NotNull(capturedPasswordSentToDataLayer);
            Assert.NotEqual("PlainTextP@ss", capturedPasswordSentToDataLayer);
            Assert.True(PasswordHasher.Verify("PlainTextP@ss", capturedPasswordSentToDataLayer!));
        }

        [Fact]
        public async Task AddUserAsync_ReturnsNewId_WhenUserIsAdded()
        {
            var request = TestDataBuilders.ValidCreateUserRequest(personId: 1);
            _personDataMock.Setup(d => d.IsPersonExistAsync(It.IsAny<int>())).ReturnsAsync(true);
            _userDataMock.Setup(d => d.GetUserByPersonIdAsync(It.IsAny<int>())).ReturnsAsync((UserResponse?)null);
            _userDataMock.Setup(d => d.GetUserByUsernameAsync(request.Username)).ReturnsAsync((UserResponse?)null);
            _userDataMock.Setup(d => d.AddUserAsync(It.IsAny<CreateUserRequest>())).ReturnsAsync(7);

            var result = await _sut.AddUserAsync(request);

            Assert.Equal(7, result);
        }

        [Fact]
        public async Task AddUserAsync_Throws_WhenDataLayerFailsToInsert()
        {
            var request = TestDataBuilders.ValidCreateUserRequest(personId: 1);
            _personDataMock.Setup(d => d.IsPersonExistAsync(It.IsAny<int>())).ReturnsAsync(true);
            _userDataMock.Setup(d => d.GetUserByPersonIdAsync(It.IsAny<int>())).ReturnsAsync((UserResponse?)null);
            _userDataMock.Setup(d => d.GetUserByUsernameAsync(request.Username)).ReturnsAsync((UserResponse?)null);
            _userDataMock.Setup(d => d.AddUserAsync(It.IsAny<CreateUserRequest>())).ReturnsAsync(0);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AddUserAsync(request));
        }
        #endregion

        #region UpdateUserAsync
        [Fact]
        public async Task UpdateUserAsync_Throws_WhenUserIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.UpdateUserAsync(1, null!));
        }

        [Fact]
        public async Task UpdateUserAsync_Throws_WhenUsernameIsTooShort()
        {
            var request = TestDataBuilders.ValidUpdateUserRequest(username: "ab");

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.UpdateUserAsync(1, request));
        }

        [Fact]
        public async Task UpdateUserAsync_Throws_WhenUserDoesNotExist()
        {
            var request = TestDataBuilders.ValidUpdateUserRequest();
            _userDataMock.Setup(d => d.IsUserExistAsync(It.IsAny<int>())).ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.UpdateUserAsync(1, request));
        }

        [Fact]
        public async Task UpdateUserAsync_Throws_WhenUsernameBelongsToAnotherUser()
        {
            var request = TestDataBuilders.ValidUpdateUserRequest(username: "ahmed123");
            _userDataMock.Setup(d => d.IsUserExistAsync(It.IsAny<int>())).ReturnsAsync(true);
            _userDataMock.Setup(d => d.GetUserByUsernameAsync("ahmed123")).ReturnsAsync(TestDataBuilders.ValidUser(userId: 2));

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.UpdateUserAsync(1, request));
        }

        [Fact]
        public async Task UpdateUserAsync_ReturnsTrue_WhenUsernameBelongsToTheSameUser()
        {
            var request = TestDataBuilders.ValidUpdateUserRequest(username: "ahmed123");
            _userDataMock.Setup(d => d.IsUserExistAsync(It.IsAny<int>())).ReturnsAsync(true);
            _userDataMock.Setup(d => d.GetUserByUsernameAsync("ahmed123")).ReturnsAsync(TestDataBuilders.ValidUser(userId: 1));
            _userDataMock.Setup(d => d.UpdateUserAsync(It.IsAny<int>(), request)).ReturnsAsync(true);

            var result = await _sut.UpdateUserAsync(1, request);

            Assert.True(result);
        }

        [Fact]
        public async Task UpdateUserAsync_ReturnsTrue_WhenUpdateSucceeds()
        {
            var request = TestDataBuilders.ValidUpdateUserRequest(username: "newname");
            _userDataMock.Setup(d => d.IsUserExistAsync(It.IsAny<int>())).ReturnsAsync(true);
            _userDataMock.Setup(d => d.GetUserByUsernameAsync("newname")).ReturnsAsync((UserResponse?)null);
            _userDataMock.Setup(d => d.UpdateUserAsync(It.IsAny<int>(), request)).ReturnsAsync(true);

            var result = await _sut.UpdateUserAsync(1, request);

            Assert.True(result);
        }

        [Fact]
        public async Task UpdateUserAsync_Throws_WhenDataLayerFailsToUpdate()
        {
            var request = TestDataBuilders.ValidUpdateUserRequest(username: "newname");
            _userDataMock.Setup(d => d.IsUserExistAsync(It.IsAny<int>())).ReturnsAsync(true);
            _userDataMock.Setup(d => d.GetUserByUsernameAsync("newname")).ReturnsAsync((UserResponse?)null);
            _userDataMock.Setup(d => d.UpdateUserAsync(It.IsAny<int>(), request)).ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.UpdateUserAsync(1, request));
        }
        #endregion

        #region ChangePasswordAsync — Validation
        [Fact]
        public async Task ChangePasswordAsync_Throws_WhenNewPasswordEqualsCurrentPassword()
        {
            var request = TestDataBuilders.ValidUpdatePasswordRequest(currentPassword: "SameP@ss1", newPassword: "SameP@ss1", confirmPassword: "SameP@ss1");

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.ChangePasswordAsync(1, request));
        }

        [Fact]
        public async Task ChangePasswordAsync_Throws_WhenConfirmPasswordDoesNotMatchNewPassword()
        {
            var request = TestDataBuilders.ValidUpdatePasswordRequest(newPassword: "NewP@ss1", confirmPassword: "Different1");

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.ChangePasswordAsync(1, request));
        }
        #endregion

        #region ChangePasswordAsync — Business rules
        [Fact]
        public async Task ChangePasswordAsync_Throws_WhenUserDoesNotExist()
        {
            var request = TestDataBuilders.ValidUpdatePasswordRequest();
            _userDataMock.Setup(d => d.IsUserExistAsync(It.IsAny<int>())).ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.ChangePasswordAsync(1, request));
        }

        [Fact]
        public async Task ChangePasswordAsync_Throws_WhenPasswordHashNotFound()
        {
            var request = TestDataBuilders.ValidUpdatePasswordRequest();
            _userDataMock.Setup(d => d.IsUserExistAsync(It.IsAny<int>())).ReturnsAsync(true);
            _userDataMock.Setup(d => d.GetPasswordHashByUserIdAsync(It.IsAny<int>())).ReturnsAsync((string?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.ChangePasswordAsync(1, request));
        }

        [Fact]
        public async Task ChangePasswordAsync_Throws_WhenCurrentPasswordIsIncorrect()
        {
            var request = TestDataBuilders.ValidUpdatePasswordRequest(currentPassword: "WrongP@ss1");
            var storedHash = PasswordHasher.Hash("ActualP@ss1");

            _userDataMock.Setup(d => d.IsUserExistAsync(It.IsAny<int>())).ReturnsAsync(true);
            _userDataMock.Setup(d => d.GetPasswordHashByUserIdAsync(It.IsAny<int>())).ReturnsAsync(storedHash);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _sut.ChangePasswordAsync(1, request));
        }

        [Fact]
        public async Task ChangePasswordAsync_Throws_WhenDataLayerFailsToUpdatePassword()
        {
            var request = TestDataBuilders.ValidUpdatePasswordRequest(currentPassword: "CorrectP@ss1", newPassword: "NewP@ss1", confirmPassword: "NewP@ss1");
            var storedHash = PasswordHasher.Hash("CorrectP@ss1");

            _userDataMock.Setup(d => d.IsUserExistAsync(It.IsAny<int>())).ReturnsAsync(true);
            _userDataMock.Setup(d => d.GetPasswordHashByUserIdAsync(It.IsAny<int>())).ReturnsAsync(storedHash);
            _userDataMock.Setup(d => d.UpdatePasswordAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.ChangePasswordAsync(1, request));
        }

        [Fact]
        public async Task ChangePasswordAsync_ReturnsTrue_AndRevokesAllRefreshTokens_WhenSucceeds()
        {
            var request = TestDataBuilders.ValidUpdatePasswordRequest(currentPassword: "CorrectP@ss1", newPassword: "NewP@ss1", confirmPassword: "NewP@ss1");
            var storedHash = PasswordHasher.Hash("CorrectP@ss1");

            _userDataMock.Setup(d => d.IsUserExistAsync(It.IsAny<int>())).ReturnsAsync(true);
            _userDataMock.Setup(d => d.GetPasswordHashByUserIdAsync(It.IsAny<int>())).ReturnsAsync(storedHash);
            _userDataMock.Setup(d => d.UpdatePasswordAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(true);

            var result = await _sut.ChangePasswordAsync(1, request);

            Assert.True(result);
            _refreshTokenDataMock.Verify(d => d.RevokeAllRefreshTokensByUserIdAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task ChangePasswordAsync_DoesNotRevokeRefreshTokens_WhenCurrentPasswordIsWrong()
        {
            var request = TestDataBuilders.ValidUpdatePasswordRequest(currentPassword: "WrongP@ss1");
            var storedHash = PasswordHasher.Hash("ActualP@ss1");

            _userDataMock.Setup(d => d.IsUserExistAsync(It.IsAny<int>())).ReturnsAsync(true);
            _userDataMock.Setup(d => d.GetPasswordHashByUserIdAsync(It.IsAny<int>())).ReturnsAsync(storedHash);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _sut.ChangePasswordAsync(1, request));

            _refreshTokenDataMock.Verify(d => d.RevokeAllRefreshTokensByUserIdAsync(It.IsAny<int>()), Times.Never);
        }
        #endregion

        #region DeleteUserAsync
        [Fact]
        public async Task DeleteUserAsync_Throws_WhenUserDoesNotExist()
        {
            _userDataMock.Setup(d => d.IsUserExistAsync(It.IsAny<int>())).ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.DeleteUserAsync(1));
        }

        [Fact]
        public async Task DeleteUserAsync_ReturnsTrue_WhenDeletionSucceeds()
        {
            _userDataMock.Setup(d => d.IsUserExistAsync(It.IsAny<int>())).ReturnsAsync(true);
            _userDataMock.Setup(d => d.DeleteUserAsync(It.IsAny<int>())).ReturnsAsync(true);

            var result = await _sut.DeleteUserAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteUserAsync_Throws_WhenDataLayerFailsToDelete()
        {
            _userDataMock.Setup(d => d.IsUserExistAsync(It.IsAny<int>())).ReturnsAsync(true);
            _userDataMock.Setup(d => d.DeleteUserAsync(It.IsAny<int>())).ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.DeleteUserAsync(1));
        }
        #endregion
    }
}