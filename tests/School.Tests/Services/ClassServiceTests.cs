using Moq;
using School.BLL;
using School.DAL.Interfaces;
using School.DTO.ClassesDTOs.Responses;
using School.Tests.TestHelpers;
using Xunit;

namespace School.Tests.Services
{
    public class ClassServiceTests
    {
        private readonly Mock<IClassData> _classDataMock = new();
        private readonly Mock<IGradeData> _gradeDataMock = new();

        private readonly ClassService _sut;

        public ClassServiceTests()
        {
            _sut = new ClassService(
                _classDataMock.Object,
                _gradeDataMock.Object);
        }

        #region Helpers

        private void SetupAddHappyPath()
        {
            _gradeDataMock
                .Setup(d => d.IsGradeExistAsync(It.IsAny<byte>()))
                .ReturnsAsync(true);

            _classDataMock
                .Setup(d => d.GetClassByDetailsAsync(
                    It.IsAny<byte>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync((ClassResponse?)null);
        }

        private void SetupUpdateHappyPath(int classId)
        {
            _classDataMock
                .Setup(d => d.IsClassExistAsync(classId))
                .ReturnsAsync(true);

            _gradeDataMock
                .Setup(d => d.IsGradeExistAsync(It.IsAny<byte>()))
                .ReturnsAsync(true);

            _classDataMock
                .Setup(d => d.GetClassByDetailsAsync(
                    It.IsAny<byte>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync((ClassResponse?)null);
        }

        #endregion

        #region Get

        [Fact]
        public async Task GetClassByIdAsync_ReturnsClass_WhenFound()
        {
            _classDataMock
                .Setup(d => d.GetClassByIdAsync(3))
                .ReturnsAsync(TestDataBuilders.ValidClass(3));

            var result = await _sut.GetClassByIdAsync(3);

            Assert.Equal(3, result.ClassID);
        }

        [Fact]
        public async Task GetClassByIdAsync_Throws_WhenNotFound()
        {
            _classDataMock
                .Setup(d => d.GetClassByIdAsync(1))
                .ReturnsAsync((ClassResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.GetClassByIdAsync(1));
        }
        [Fact]
        public async Task GetClassByDetailsAsync_ReturnsClass_WhenFound()
        {
            var classResponse = TestDataBuilders.ValidClass();

            _classDataMock
                .Setup(d => d.GetClassByDetailsAsync(
                    1,
                    "Class A",
                    "2025-2026"))
                .ReturnsAsync(classResponse);

            var result = await _sut.GetClassByDetailsAsync(
                1,
                "Class A",
                "2025-2026");

            Assert.Equal(classResponse.ClassID, result.ClassID);
        }

        [Fact]
        public async Task GetClassByDetailsAsync_Throws_WhenNotFound()
        {
            _classDataMock
                .Setup(d => d.GetClassByDetailsAsync(
                    1,
                    "Class A",
                    "2025-2026"))
                .ReturnsAsync((ClassResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.GetClassByDetailsAsync(
                    1,
                    "Class A",
                    "2025-2026"));
        }

        #endregion

        #region Add

        [Fact]
        public async Task AddClassAsync_Throws_WhenClassIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _sut.AddClassAsync(null!));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(101)]
        public async Task AddClassAsync_Throws_WhenCapacityIsOutOfRange(int capacity)
        {
            var request = TestDataBuilders.ValidCreateClassRequest(
                capacity: capacity);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                () => _sut.AddClassAsync(request));
        }

        [Fact]
        public async Task AddClassAsync_Throws_WhenGradeDoesNotExist()
        {
            var request = TestDataBuilders.ValidCreateClassRequest();

            _gradeDataMock
                .Setup(d => d.IsGradeExistAsync(request.GradeID))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.AddClassAsync(request));
        }

        [Fact]
        public async Task AddClassAsync_Throws_WhenClassAlreadyExists()
        {
            var request = TestDataBuilders.ValidCreateClassRequest();

            _gradeDataMock
                .Setup(d => d.IsGradeExistAsync(request.GradeID))
                .ReturnsAsync(true);

            _classDataMock
                .Setup(d => d.GetClassByDetailsAsync(
                    request.GradeID,
                    request.ClassName,
                    request.AcademicYear))
                .ReturnsAsync(TestDataBuilders.ValidClass());

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.AddClassAsync(request));
        }

        [Fact]
        public async Task AddClassAsync_ReturnsNewId_WhenClassIsAdded()
        {
            var request = TestDataBuilders.ValidCreateClassRequest();

            SetupAddHappyPath();

            _classDataMock
                .Setup(d => d.AddClassAsync(request))
                .ReturnsAsync(10);

            var result = await _sut.AddClassAsync(request);

            Assert.Equal(10, result);
        }

        [Fact]
        public async Task AddClassAsync_Throws_WhenDataLayerFailsToInsert()
        {
            var request = TestDataBuilders.ValidCreateClassRequest();

            SetupAddHappyPath();

            _classDataMock
                .Setup(d => d.AddClassAsync(request))
                .ReturnsAsync(0);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.AddClassAsync(request));
        }

        #endregion

        #region Update

        [Fact]
        public async Task UpdateClassAsync_Throws_WhenClassIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _sut.UpdateClassAsync(1, null!));
        }

        [Fact]
        public async Task UpdateClassAsync_Throws_WhenClassDoesNotExist()
        {
            var request = TestDataBuilders.ValidUpdateClassRequest();

            _classDataMock
                .Setup(d => d.IsClassExistAsync(1))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.UpdateClassAsync(1, request));
        }

        [Fact]
        public async Task UpdateClassAsync_Throws_WhenGradeDoesNotExist()
        {
            var request = TestDataBuilders.ValidUpdateClassRequest();

            _classDataMock
                .Setup(d => d.IsClassExistAsync(1))
                .ReturnsAsync(true);

            _gradeDataMock
                .Setup(d => d.IsGradeExistAsync(request.GradeID))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.UpdateClassAsync(1, request));
        }

        [Fact]
        public async Task UpdateClassAsync_Throws_WhenClassAlreadyExists()
        {
            var request = TestDataBuilders.ValidUpdateClassRequest();

            _classDataMock
                .Setup(d => d.IsClassExistAsync(1))
                .ReturnsAsync(true);

            _gradeDataMock
                .Setup(d => d.IsGradeExistAsync(request.GradeID))
                .ReturnsAsync(true);

            _classDataMock
                .Setup(d => d.GetClassByDetailsAsync(
                    request.GradeID,
                    request.ClassName,
                    request.AcademicYear))
                .ReturnsAsync(TestDataBuilders.ValidClass(classId: 2));

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.UpdateClassAsync(1, request));
        }

        [Fact]
        public async Task UpdateClassAsync_DoesNotThrow_WhenFoundClassIsTheCurrentClass()
        {
            var request = TestDataBuilders.ValidUpdateClassRequest();

            _classDataMock
                .Setup(d => d.IsClassExistAsync(1))
                .ReturnsAsync(true);

            _gradeDataMock
                .Setup(d => d.IsGradeExistAsync(request.GradeID))
                .ReturnsAsync(true);

            _classDataMock
                .Setup(d => d.GetClassByDetailsAsync(
                    request.GradeID,
                    request.ClassName,
                    request.AcademicYear))
                .ReturnsAsync(TestDataBuilders.ValidClass(classId: 1));

            _classDataMock
                .Setup(d => d.UpdateClassAsync(1, request))
                .ReturnsAsync(true);

            var result = await _sut.UpdateClassAsync(1, request);

            Assert.True(result);
        }

        [Fact]
        public async Task UpdateClassAsync_ReturnsTrue_WhenClassIsUpdated()
        {
            var request = TestDataBuilders.ValidUpdateClassRequest();

            SetupUpdateHappyPath(1);

            _classDataMock
                .Setup(d => d.UpdateClassAsync(1, request))
                .ReturnsAsync(true);

            var result = await _sut.UpdateClassAsync(1, request);

            Assert.True(result);
        }

        [Fact]
        public async Task UpdateClassAsync_Throws_WhenDataLayerFailsToUpdate()
        {
            var request = TestDataBuilders.ValidUpdateClassRequest();

            SetupUpdateHappyPath(1);

            _classDataMock
                .Setup(d => d.UpdateClassAsync(1, request))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.UpdateClassAsync(1, request));
        }

        #endregion

        #region Delete
        [Fact]
        public async Task DeleteClassAsync_Throws_WhenClassDoesNotExist()
        {
            _classDataMock
                .Setup(d => d.IsClassExistAsync(1))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.DeleteClassAsync(1));
        }

        [Fact]
        public async Task DeleteClassAsync_ReturnsTrue_WhenClassIsDeleted()
        {
            _classDataMock
                .Setup(d => d.IsClassExistAsync(1))
                .ReturnsAsync(true);

            _classDataMock
                .Setup(d => d.DeleteClassAsync(1))
                .ReturnsAsync(true);

            var result = await _sut.DeleteClassAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteClassAsync_Throws_WhenDataLayerFailsToDelete()
        {
            _classDataMock
                .Setup(d => d.IsClassExistAsync(1))
                .ReturnsAsync(true);

            _classDataMock
                .Setup(d => d.DeleteClassAsync(1))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.DeleteClassAsync(1));
        }

        #endregion

        #region IsClassExist

        [Fact]
        public async Task IsClassExistAsync_ReturnsTrue_WhenClassExists()
        {
            _classDataMock
                .Setup(d => d.IsClassExistAsync(1))
                .ReturnsAsync(true);

            var result = await _sut.IsClassExistAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task IsClassExistAsync_ReturnsFalse_WhenClassDoesNotExist()
        {
            _classDataMock
                .Setup(d => d.IsClassExistAsync(1))
                .ReturnsAsync(false);

            var result = await _sut.IsClassExistAsync(1);

            Assert.False(result);
        }
        #endregion
    }
}