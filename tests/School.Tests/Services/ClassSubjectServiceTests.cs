using Moq;
using School.BLL;
using School.DAL.Interfaces;
using School.DTO.AssociationsDTOs.ClassSubjectDTOs.Responses;
using School.DTO.AssociationsDTOs.TeacherSubjectDTOs.Requests;
using School.Tests.TestHelpers;
using Xunit;

namespace School.Tests.Services
{
    public class ClassSubjectServiceTests
    {
        private readonly Mock<IClassSubjectData> _classSubjectDataMock = new();
        private readonly Mock<IClassData> _classDataMock = new();
        private readonly Mock<ITeacherData> _teacherDataMock = new();
        private readonly Mock<ISubjectData> _subjectDataMock = new();
        private readonly Mock<ITeacherSubjectData> _teacherSubjectDataMock = new();

        private readonly ClassSubjectService _sut;

        public ClassSubjectServiceTests()
        {
            _sut = new ClassSubjectService(
                _classSubjectDataMock.Object,
                _classDataMock.Object,
                _teacherDataMock.Object,
                _subjectDataMock.Object,
                _teacherSubjectDataMock.Object);
        }

        #region Helpers

        private void SetupAddHappyPath()
        {
            _classDataMock
                .Setup(d => d.IsClassExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _teacherDataMock
                .Setup(d => d.IsTeacherExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _subjectDataMock
                .Setup(d => d.IsSubjectExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _teacherSubjectDataMock
                .Setup(d => d.IsTeacherSubjectExistAsync(
                    It.IsAny<TeacherSubjectRequest>()))
                .ReturnsAsync(true);

            _classSubjectDataMock
                .Setup(d => d.GetClassSubjectByDetailsAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .ReturnsAsync((ClassSubjectResponse?)null);
        }

        private void SetupUpdateHappyPath(int classSubjectId)
        {
            _classSubjectDataMock
                .Setup(d => d.GetClassSubjectByIdAsync(classSubjectId))
                .ReturnsAsync(
                    TestDataBuilders.ValidClassSubject(
                        classSubjectId: classSubjectId));

            _teacherDataMock
                .Setup(d => d.IsTeacherExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _teacherSubjectDataMock
                .Setup(d => d.IsTeacherSubjectExistAsync(
                    It.IsAny<TeacherSubjectRequest>()))
                .ReturnsAsync(true);
        }

        #endregion

        #region Get

        [Fact]
        public async Task GetClassSubjectByIdAsync_ReturnsClassSubject_WhenFound()
        {
            var classSubject =
                TestDataBuilders.ValidClassSubject(classSubjectId: 3);

            _classSubjectDataMock
                .Setup(d => d.GetClassSubjectByIdAsync(3))
                .ReturnsAsync(classSubject);

            var result = await _sut.GetClassSubjectByIdAsync(3);

            Assert.Equal(3, result.ClassSubjectID);
        }

        [Fact]
        public async Task GetClassSubjectByIdAsync_Throws_WhenNotFound()
        {
            _classSubjectDataMock
                .Setup(d => d.GetClassSubjectByIdAsync(1))
                .ReturnsAsync((ClassSubjectResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.GetClassSubjectByIdAsync(1));
        }

        [Fact]
        public async Task GetClassSubjectsByClassIdAsync_Throws_WhenClassDoesNotExist()
        {
            _classDataMock
                .Setup(d => d.IsClassExistAsync(1))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.GetClassSubjectsByClassIdAsync(1));
        }

        [Fact]
        public async Task GetClassSubjectsByTeacherIdAsync_Throws_WhenTeacherDoesNotExist()
        {
            _teacherDataMock
                .Setup(d => d.IsTeacherExistAsync(1))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.GetClassSubjectsByTeacherIdAsync(1));
        }

        [Fact]
        public async Task GetClassSubjectsBySubjectIdAsync_Throws_WhenSubjectDoesNotExist()
        {
            _subjectDataMock
                .Setup(d => d.IsSubjectExistAsync(1))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.GetClassSubjectsBySubjectIdAsync(1));
        }

        #endregion

        #region Add

        [Fact]
        public async Task AddClassSubjectAsync_Throws_WhenClassSubjectIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _sut.AddClassSubjectAsync(null!));
        }

        [Fact]
        public async Task AddClassSubjectAsync_Throws_WhenClassDoesNotExist()
        {
            var request =
                TestDataBuilders.ValidCreateClassSubjectRequest();

            _classDataMock
                .Setup(d => d.IsClassExistAsync(request.ClassID))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.AddClassSubjectAsync(request));
        }

        [Fact]
        public async Task AddClassSubjectAsync_Throws_WhenTeacherDoesNotExist()
        {
            var request =
                TestDataBuilders.ValidCreateClassSubjectRequest();

            _classDataMock
                .Setup(d => d.IsClassExistAsync(request.ClassID))
                .ReturnsAsync(true);

            _teacherDataMock
                .Setup(d => d.IsTeacherExistAsync(request.TeacherID))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.AddClassSubjectAsync(request));
        }

        [Fact]
        public async Task AddClassSubjectAsync_Throws_WhenSubjectDoesNotExist()
        {
            var request =
                TestDataBuilders.ValidCreateClassSubjectRequest();

            _classDataMock
                .Setup(d => d.IsClassExistAsync(request.ClassID))
                .ReturnsAsync(true);

            _teacherDataMock
                .Setup(d => d.IsTeacherExistAsync(request.TeacherID))
                .ReturnsAsync(true);

            _subjectDataMock
                .Setup(d => d.IsSubjectExistAsync(request.SubjectID))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.AddClassSubjectAsync(request));
        }

        [Fact]
        public async Task AddClassSubjectAsync_Throws_WhenTeacherCannotTeachSubject()
        {
            var request =
                TestDataBuilders.ValidCreateClassSubjectRequest();

            _classDataMock
                .Setup(d => d.IsClassExistAsync(request.ClassID))
                .ReturnsAsync(true);

            _teacherDataMock
                .Setup(d => d.IsTeacherExistAsync(request.TeacherID))
                .ReturnsAsync(true);

            _subjectDataMock
                .Setup(d => d.IsSubjectExistAsync(request.SubjectID))
                .ReturnsAsync(true);

            _teacherSubjectDataMock
                .Setup(d => d.IsTeacherSubjectExistAsync(
                    It.Is<TeacherSubjectRequest>(x =>
                        x.TeacherID == request.TeacherID &&
                        x.SubjectID == request.SubjectID)))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.AddClassSubjectAsync(request));
        }

        [Fact]
        public async Task AddClassSubjectAsync_Throws_WhenClassSubjectAlreadyExists()
        {
            var request =
                TestDataBuilders.ValidCreateClassSubjectRequest();

            SetupAddHappyPath();

            _classSubjectDataMock
                .Setup(d => d.GetClassSubjectByDetailsAsync(
                    request.ClassID,
                    request.TeacherID,
                    request.SubjectID))
                .ReturnsAsync(TestDataBuilders.ValidClassSubject());

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.AddClassSubjectAsync(request));
        }

        [Fact]
        public async Task AddClassSubjectAsync_ReturnsNewId_WhenClassSubjectIsAdded()
        {
            var request =
                TestDataBuilders.ValidCreateClassSubjectRequest();

            SetupAddHappyPath();

            _classSubjectDataMock
                .Setup(d => d.AddClassSubjectAsync(request))
                .ReturnsAsync(10);

            var result = await _sut.AddClassSubjectAsync(request);

            Assert.Equal(10, result);
        }

        [Fact]
        public async Task AddClassSubjectAsync_Throws_WhenDataLayerFailsToInsert()
        {
            var request =
                TestDataBuilders.ValidCreateClassSubjectRequest();

            SetupAddHappyPath();

            _classSubjectDataMock
                .Setup(d => d.AddClassSubjectAsync(request))
                .ReturnsAsync(0);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.AddClassSubjectAsync(request));
        }

        #endregion

        #region Update

        [Fact]
        public async Task UpdateClassSubjectAsync_Throws_WhenClassSubjectIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _sut.UpdateClassSubjectAsync(1, null!));
        }

        [Fact]
        public async Task UpdateClassSubjectAsync_Throws_WhenClassSubjectDoesNotExist()
        {
            var request =
                TestDataBuilders.ValidUpdateClassSubjectRequest();

            _classSubjectDataMock
                .Setup(d => d.GetClassSubjectByIdAsync(1))
                .ReturnsAsync((ClassSubjectResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.UpdateClassSubjectAsync(1, request));
        }

        [Fact]
        public async Task UpdateClassSubjectAsync_Throws_WhenTeacherDoesNotExist()
        {
            var request =
                TestDataBuilders.ValidUpdateClassSubjectRequest();

            _classSubjectDataMock
                .Setup(d => d.GetClassSubjectByIdAsync(1))
                .ReturnsAsync(
                    TestDataBuilders.ValidClassSubject(
                        classSubjectId: 1));

            _teacherDataMock
                .Setup(d => d.IsTeacherExistAsync(request.TeacherID))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.UpdateClassSubjectAsync(1, request));
        }

        [Fact]
        public async Task UpdateClassSubjectAsync_Throws_WhenTeacherCannotTeachCurrentSubject()
        {
            var request =
                TestDataBuilders.ValidUpdateClassSubjectRequest();

            _classSubjectDataMock
                .Setup(d => d.GetClassSubjectByIdAsync(1))
                .ReturnsAsync(
                    TestDataBuilders.ValidClassSubject(
                        classSubjectId: 1));

            _teacherDataMock
                .Setup(d => d.IsTeacherExistAsync(request.TeacherID))
                .ReturnsAsync(true);

            _teacherSubjectDataMock
                .Setup(d => d.IsTeacherSubjectExistAsync(
                    It.Is<TeacherSubjectRequest>(x =>
                        x.TeacherID == request.TeacherID &&
                        x.SubjectID == 1)))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.UpdateClassSubjectAsync(1, request));
        }

        [Fact]
        public async Task UpdateClassSubjectAsync_ReturnsTrue_WhenClassSubjectIsUpdated()
        {
            var request =
                TestDataBuilders.ValidUpdateClassSubjectRequest();

            SetupUpdateHappyPath(1);

            _classSubjectDataMock
                .Setup(d => d.UpdateClassSubjectAsync(1, request))
                .ReturnsAsync(true);

            var result =
                await _sut.UpdateClassSubjectAsync(1, request);

            Assert.True(result);
        }

        [Fact]
        public async Task UpdateClassSubjectAsync_Throws_WhenDataLayerFailsToUpdate()
        {
            var request =
                TestDataBuilders.ValidUpdateClassSubjectRequest();

            SetupUpdateHappyPath(1);

            _classSubjectDataMock
                .Setup(d => d.UpdateClassSubjectAsync(1, request))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.UpdateClassSubjectAsync(1, request));
        }

        #endregion

        #region Delete
        [Fact]
        public async Task DeleteClassSubjectAsync_Throws_WhenClassSubjectDoesNotExist()
        {
            _classSubjectDataMock
                .Setup(d => d.IsClassSubjectExistAsync(1))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.DeleteClassSubjectAsync(1));
        }

        [Fact]
        public async Task DeleteClassSubjectAsync_ReturnsTrue_WhenClassSubjectIsDeleted()
        {
            _classSubjectDataMock
                .Setup(d => d.IsClassSubjectExistAsync(1))
                .ReturnsAsync(true);

            _classSubjectDataMock
                .Setup(d => d.DeleteClassSubjectAsync(1))
                .ReturnsAsync(true);

            var result =
                await _sut.DeleteClassSubjectAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteClassSubjectAsync_Throws_WhenDataLayerFailsToDelete()
        {
            _classSubjectDataMock
                .Setup(d => d.IsClassSubjectExistAsync(1))
                .ReturnsAsync(true);

            _classSubjectDataMock
                .Setup(d => d.DeleteClassSubjectAsync(1))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.DeleteClassSubjectAsync(1));
        }

        #endregion

        #region IsClassSubjectExist
        [Fact]
        public async Task IsClassSubjectExistAsync_ReturnsTrue_WhenClassSubjectExists()
        {
            _classSubjectDataMock
                .Setup(d => d.IsClassSubjectExistAsync(1))
                .ReturnsAsync(true);

            var result =
                await _sut.IsClassSubjectExistAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task IsClassSubjectExistAsync_ReturnsFalse_WhenClassSubjectDoesNotExist()
        {
            _classSubjectDataMock
                .Setup(d => d.IsClassSubjectExistAsync(1))
                .ReturnsAsync(false);

            var result =
                await _sut.IsClassSubjectExistAsync(1);

            Assert.False(result);
        }

        #endregion
    }
}