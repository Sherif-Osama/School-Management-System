using Moq;
using School.BLL.Authentication;
using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.AuthDTOs;
using School.DTO.UserDTOs.Responses;
using School.Tests.TestHelpers;
using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace School.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserData> _userDataMock = new();
        private readonly Mock<IJwtService> _jwtServiceMock = new();
        private readonly Mock<IUserRoleData> _userRoleDataMock = new();
        private readonly Mock<IRolePermissionData> _rolePermissionDataMock = new();
        private readonly Mock<IRefreshTokenData> _refreshTokenDataMock = new();
        private readonly JwtSettings _jwtSettings = new()
        {
            Key = "unit-test-signing-key",
            Issuer = "SchoolAPI.Tests",
            Audience = "SchoolAPI.Tests",
            ExpireMinutes = 30,
            RefreshTokenExpireDays = 7
        };
        private readonly AuthService _sut;

        public AuthServiceTests()
        {
            _sut = new AuthService(
                _userDataMock.Object,
                _jwtServiceMock.Object,
                _userRoleDataMock.Object,
                _rolePermissionDataMock.Object,
                _refreshTokenDataMock.Object,
                _jwtSettings);

            _jwtServiceMock.Setup(j => j.GenerateToken(It.IsAny<UserAuth>()))
                .Returns(new LoginResponse
                {
                    AccessToken = "fake-access-token",
                    ExpiresAt = DateTime.Now.AddMinutes(30),
                    RefreshToken = string.Empty,
                    RefreshTokenExpiresAt = DateTime.Now
                });

            _jwtServiceMock.Setup(j => j.GenerateRefreshToken()).Returns("fake-refresh-token");
        }

        #region LoginAsync — Input validation
        [Fact]
        public async Task LoginAsync_Throws_WhenUsernameIsEmpty()
        {
            var request = new LoginRequest { Username = "", Password = "password123" };

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.LoginAsync(request));
        }

        [Fact]
        public async Task LoginAsync_Throws_WhenPasswordIsEmpty()
        {
            var request = new LoginRequest { Username = "ahmed", Password = "" };

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.LoginAsync(request));
        }
        #endregion

        #region LoginAsync — Business rules
        [Fact]
        public async Task LoginAsync_Throws_WhenUserDoesNotExist()
        {
            var request = new LoginRequest { Username = "None", Password = "password123" };
            _userDataMock.Setup(d => d.GetUserForAuthenticationAsync("None")).ReturnsAsync((UserAuth?)null);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _sut.LoginAsync(request));
        }

        [Fact]
        public async Task LoginAsync_Throws_WhenUserIsInactive()
        {
            var user = TestDataBuilders.MakeUserAuth("ahmed", "password123", isActive: false);
            var request = new LoginRequest { Username = "ahmed", Password = "password123" };
            _userDataMock.Setup(d => d.GetUserForAuthenticationAsync("ahmed")).ReturnsAsync(user);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _sut.LoginAsync(request));
        }

        [Fact]
        public async Task LoginAsync_Throws_WhenPasswordIsIncorrect()
        {
            var user = TestDataBuilders.MakeUserAuth("ahmed", "correct-password");
            var request = new LoginRequest { Username = "ahmed", Password = "wrong-password" };
            _userDataMock.Setup(d => d.GetUserForAuthenticationAsync("ahmed")).ReturnsAsync(user);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _sut.LoginAsync(request));
        }

        [Fact]
        public async Task LoginAsync_ReturnsTokens_WhenCredentialsAreValid()
        {
            var user = TestDataBuilders.MakeUserAuth("ahmed", "correct-password");
            var request = new LoginRequest { Username = "ahmed", Password = "correct-password" };
            _userDataMock.Setup(d => d.GetUserForAuthenticationAsync("ahmed")).ReturnsAsync(user);
            _userRoleDataMock.Setup(d => d.GetRoleNamesByUserIdAsync(user.UserID)).ReturnsAsync(["Admin"]);
            _rolePermissionDataMock.Setup(d => d.GetPermissionNamesByUserIdAsync(user.UserID)).ReturnsAsync(["Students.View.All"]);

            var response = await _sut.LoginAsync(request);

            Assert.Equal("fake-access-token", response.AccessToken);
            Assert.Equal("fake-refresh-token", response.RefreshToken);
        }

        [Fact]
        public async Task LoginAsync_PersistsTheIssuedRefreshToken()
        {
            var user = TestDataBuilders.MakeUserAuth("ahmed", "correct-password");

            var request = new LoginRequest
            {
                Username = "ahmed",
                Password = "correct-password"
            };

            _userDataMock.Setup(d => d.GetUserForAuthenticationAsync("ahmed"))
                .ReturnsAsync(user);

            _userRoleDataMock.Setup(d => d.GetRoleNamesByUserIdAsync(user.UserID))
                .ReturnsAsync(["Admin"]);

            _rolePermissionDataMock.Setup(d => d.GetPermissionNamesByUserIdAsync(user.UserID))
                .ReturnsAsync(["Students.View.All"]);

            await _sut.LoginAsync(request);

            string expectedHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes("fake-refresh-token")));

            _refreshTokenDataMock.Verify(d => d.AddRefreshTokenAsync(user.UserID, expectedHash, It.IsAny<DateTime>()), Times.Once);
        }
        #endregion

        #region RefreshTokenAsync
        [Fact]
        public async Task RefreshTokenAsync_Throws_WhenTokenDoesNotExist()
        {
            var request = new RefreshTokenRequest { RefreshToken = "unknown-token" };
            _refreshTokenDataMock.Setup(d => d.GetRefreshTokenByTokenAsync("unknown-token")).ReturnsAsync((RefreshToken?)null);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _sut.RefreshTokenAsync(request));
        }

        [Fact]
        public async Task RefreshTokenAsync_Throws_WhenTokenIsExpired()
        {
            var request = new RefreshTokenRequest { RefreshToken = "expired-token" };
            var storedToken = new RefreshToken
            {
                RefreshTokenID = 1,
                UserID = 1,
                Token = "expired-token",
                ExpiresAt = DateTime.Now.AddDays(-1),
                CreatedAt = DateTime.Now.AddDays(-8)
            };
            _refreshTokenDataMock.Setup(d => d.GetRefreshTokenByTokenAsync("expired-token")).ReturnsAsync(storedToken);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _sut.RefreshTokenAsync(request));
        }

        [Fact]
        public async Task RefreshTokenAsync_Throws_WhenTokenIsAlreadyRevoked()
        {
            var request = new RefreshTokenRequest { RefreshToken = "revoked-token" };
            var storedToken = new RefreshToken
            {
                RefreshTokenID = 1,
                UserID = 1,
                Token = "revoked-token",
                ExpiresAt = DateTime.Now.AddDays(5),
                CreatedAt = DateTime.Now.AddDays(-2),
                RevokedAt = DateTime.Now.AddDays(-1)
            };
            _refreshTokenDataMock.Setup(d => d.GetRefreshTokenByTokenAsync("revoked-token")).ReturnsAsync(storedToken);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _sut.RefreshTokenAsync(request));
        }

        [Fact]
        public async Task RefreshTokenAsync_RevokesTheOldToken_BeforeIssuingANewOne()
        {
            var request = new RefreshTokenRequest
            {
                RefreshToken = "valid-token"
            };

            var storedToken = new RefreshToken
            {
                RefreshTokenID = 1,
                UserID = 1,
                Token = "valid-token",
                ExpiresAt = DateTime.Now.AddDays(5),
                CreatedAt = DateTime.Now.AddDays(-1)
            };

            string tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes("valid-token")));

            _refreshTokenDataMock.Setup(d => d.GetRefreshTokenByTokenAsync(tokenHash))
                .ReturnsAsync(storedToken);

            _userDataMock.Setup(d => d.GetUserByIdAsync(1))
                .ReturnsAsync(new UserResponse
                {
                    UserID = 1,
                    PersonID = 100,
                    NationalID = "12345678901234",
                    FirstName = "Ahmed",
                    SecondName = "Mohamed",
                    ThirdName = "Ali",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    Gender = 1,
                    Phone = "01000000000",
                    CityID = 1,
                    Username = "ahmed",
                    IsActive = true
                });

            _userRoleDataMock.Setup(d => d.GetRoleNamesByUserIdAsync(1))
                .ReturnsAsync(["Admin"]);

            _rolePermissionDataMock.Setup(d => d.GetPermissionNamesByUserIdAsync(1))
                .ReturnsAsync(["Students.View.All"]);

            await _sut.RefreshTokenAsync(request);

            _refreshTokenDataMock.Verify(d => d.RevokeRefreshTokenAsync(tokenHash), Times.Once);
        }

        [Fact]
        public async Task RefreshTokenAsync_Throws_WhenTheOwningUserIsNoLongerActive()
        {
            var request = new RefreshTokenRequest { RefreshToken = "valid-token" };
            var storedToken = new RefreshToken
            {
                RefreshTokenID = 1,
                UserID = 1,
                Token = "valid-token",
                ExpiresAt = DateTime.Now.AddDays(5),
                CreatedAt = DateTime.Now.AddDays(-1)
            };
            _refreshTokenDataMock.Setup(d => d.GetRefreshTokenByTokenAsync("valid-token")).ReturnsAsync(storedToken);
            _userDataMock.Setup(d => d.GetUserByIdAsync(1)).ReturnsAsync(new UserResponse
            {
                UserID = 1,
                PersonID = 100,
                NationalID = "12345678901234",
                FirstName = "Ahmed",
                SecondName = "Mohamed",
                ThirdName = "Ali",
                DateOfBirth = new DateTime(1990, 1, 1),
                Gender = 1,
                Phone = "01000000000",
                CityID = 1,
                Username = "ahmed",
                IsActive = false
            });

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _sut.RefreshTokenAsync(request));
        }
        #endregion

        #region RevokeTokenAsync
        [Fact]
        public async Task RevokeTokenAsync_CallsDataLayer_WithTheGivenToken()
        {
            var request = new RefreshTokenRequest
            {
                RefreshToken = "some-token"
            };

            string tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes("some-token")));

            await _sut.RevokeTokenAsync(request);

            _refreshTokenDataMock.Verify(d => d.RevokeRefreshTokenAsync(tokenHash), Times.Once);
        }
        #endregion
    }
}
