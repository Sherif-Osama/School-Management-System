using Moq;
using School.BLL;
using School.DAL.Interfaces;
using School.DTO.AssociationsDTOs.TeacherSubjectDTOs.Responses;
using School.Tests.TestHelpers;
using Xunit;

namespace School.Tests.Services
{
    public class TeacherSubjectServiceTests
    {
        private readonly Mock<ITeacherSubjectData> _teacherSubjectDataMock = new();
        private readonly Mock<ITeacherData> _teacherDataMock = new();
        private readonly Mock<ISubjectData> _subjectDataMock = new();
        private readonly TeacherSubjectService _sut;

        public TeacherSubjectServiceTests()
        {
            _sut = new TeacherSubjectService(
                _teacherSubjectDataMock.Object,
                _teacherDataMock.Object,
                _subjectDataMock.Object);
        }

        #region GetSubjectsByTeacherIdAsync
        [Fact]
        public async Task GetSubjectsByTeacherIdAsync_Throws_WhenTeacherDoesNotExist()
        {
            _teacherDataMock.Setup(d => d.IsTeacherExistAsync(1)).ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetSubjectsByTeacherIdAsync(1));
        }

        [Fact]
        public async Task GetSubjectsByTeacherIdAsync_ReturnsSubjects_WhenTeacherExists()
        {
            var subjects = new List<TeacherSubjectResponse> { TestDataBuilders.ValidTeacherSubject(teacherId: 1) };
            _teacherDataMock.Setup(d => d.IsTeacherExistAsync(1)).ReturnsAsync(true);
            _teacherSubjectDataMock.Setup(d => d.GetSubjectsByTeacherIdAsync(1)).ReturnsAsync(subjects);

            var result = await _sut.GetSubjectsByTeacherIdAsync(1);

            Assert.Single(result);
        }
        #endregion

        #region GetTeachersBySubjectIdAsync
        [Fact]
        public async Task GetTeachersBySubjectIdAsync_Throws_WhenSubjectDoesNotExist()
        {
            _subjectDataMock.Setup(d => d.IsSubjectExistAsync(1)).ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetTeachersBySubjectIdAsync(1));
        }

        [Fact]
        public async Task GetTeachersBySubjectIdAsync_ReturnsTeachers_WhenSubjectExists()
        {
            var teachers = new List<TeacherSubjectResponse> { TestDataBuilders.ValidTeacherSubject(subjectId: 1) };
            _subjectDataMock.Setup(d => d.IsSubjectExistAsync(1)).ReturnsAsync(true);
            _teacherSubjectDataMock.Setup(d => d.GetTeachersBySubjectIdAsync(1)).ReturnsAsync(teachers);

            var result = await _sut.GetTeachersBySubjectIdAsync(1);

            Assert.Single(result);
        }
        #endregion

        #region AssignSubjectToTeacherAsync — Validation
        [Fact]
        public async Task AssignSubjectToTeacherAsync_Throws_WhenRelationIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.AssignSubjectToTeacherAsync(null!));
        }

        [Fact]
        public async Task AssignSubjectToTeacherAsync_Throws_WhenTeacherIdIsInvalid()
        {
            var request = TestDataBuilders.ValidTeacherSubjectRequest(teacherId: 0);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.AssignSubjectToTeacherAsync(request));
        }

        [Fact]
        public async Task AssignSubjectToTeacherAsync_Throws_WhenSubjectIdIsInvalid()
        {
            var request = TestDataBuilders.ValidTeacherSubjectRequest(subjectId: 0);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.AssignSubjectToTeacherAsync(request));
        }
        #endregion

        #region AssignSubjectToTeacherAsync — Business rules
        [Fact]
        public async Task AssignSubjectToTeacherAsync_Throws_WhenTeacherDoesNotExist()
        {
            var request = TestDataBuilders.ValidTeacherSubjectRequest(teacherId: 1, subjectId: 1);
            _teacherDataMock.Setup(d => d.IsTeacherExistAsync(1)).ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.AssignSubjectToTeacherAsync(request));
        }

        [Fact]
        public async Task AssignSubjectToTeacherAsync_Throws_WhenSubjectDoesNotExist()
        {
            var request = TestDataBuilders.ValidTeacherSubjectRequest(teacherId: 1, subjectId: 1);
            _teacherDataMock.Setup(d => d.IsTeacherExistAsync(1)).ReturnsAsync(true);
            _subjectDataMock.Setup(d => d.IsSubjectExistAsync(1)).ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.AssignSubjectToTeacherAsync(request));
        }

        [Fact]
        public async Task AssignSubjectToTeacherAsync_Throws_WhenSubjectIsAlreadyAssignedToTeacher()
        {
            var request = TestDataBuilders.ValidTeacherSubjectRequest(teacherId: 1, subjectId: 1);
            _teacherDataMock.Setup(d => d.IsTeacherExistAsync(1)).ReturnsAsync(true);
            _subjectDataMock.Setup(d => d.IsSubjectExistAsync(1)).ReturnsAsync(true);
            _teacherSubjectDataMock.Setup(d => d.IsTeacherSubjectExistAsync(request)).ReturnsAsync(true);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AssignSubjectToTeacherAsync(request));
        }

        [Fact]
        public async Task AssignSubjectToTeacherAsync_ReturnsTrue_WhenAssignedSuccessfully()
        {
            var request = TestDataBuilders.ValidTeacherSubjectRequest(teacherId: 1, subjectId: 1);
            _teacherDataMock.Setup(d => d.IsTeacherExistAsync(1)).ReturnsAsync(true);
            _subjectDataMock.Setup(d => d.IsSubjectExistAsync(1)).ReturnsAsync(true);
            _teacherSubjectDataMock.Setup(d => d.IsTeacherSubjectExistAsync(request)).ReturnsAsync(false);
            _teacherSubjectDataMock.Setup(d => d.AssignSubjectToTeacherAsync(request)).ReturnsAsync(true);

            var result = await _sut.AssignSubjectToTeacherAsync(request);

            Assert.True(result);
        }

        [Fact]
        public async Task AssignSubjectToTeacherAsync_Throws_WhenDataLayerFailsToAssign()
        {
            var request = TestDataBuilders.ValidTeacherSubjectRequest(teacherId: 1, subjectId: 1);
            _teacherDataMock.Setup(d => d.IsTeacherExistAsync(1)).ReturnsAsync(true);
            _subjectDataMock.Setup(d => d.IsSubjectExistAsync(1)).ReturnsAsync(true);
            _teacherSubjectDataMock.Setup(d => d.IsTeacherSubjectExistAsync(request)).ReturnsAsync(false);
            _teacherSubjectDataMock.Setup(d => d.AssignSubjectToTeacherAsync(request)).ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AssignSubjectToTeacherAsync(request));
        }
        #endregion

        #region RemoveSubjectFromTeacherAsync — Validation
        [Fact]
        public async Task RemoveSubjectFromTeacherAsync_Throws_WhenRelationIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.RemoveSubjectFromTeacherAsync(null!));
        }

        [Fact]
        public async Task RemoveSubjectFromTeacherAsync_Throws_WhenTeacherIdIsInvalid()
        {
            var request = TestDataBuilders.ValidTeacherSubjectRequest(teacherId: 0);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.RemoveSubjectFromTeacherAsync(request));
        }

        [Fact]
        public async Task RemoveSubjectFromTeacherAsync_Throws_WhenSubjectIdIsInvalid()
        {
            var request = TestDataBuilders.ValidTeacherSubjectRequest(subjectId: 0);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.RemoveSubjectFromTeacherAsync(request));
        }
        #endregion

        #region RemoveSubjectFromTeacherAsync — Business rules
        [Fact]
        public async Task RemoveSubjectFromTeacherAsync_Throws_WhenTeacherDoesNotExist()
        {
            var request = TestDataBuilders.ValidTeacherSubjectRequest(teacherId: 1, subjectId: 1);
            _teacherDataMock.Setup(d => d.IsTeacherExistAsync(1)).ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.RemoveSubjectFromTeacherAsync(request));
        }

        [Fact]
        public async Task RemoveSubjectFromTeacherAsync_Throws_WhenSubjectDoesNotExist()
        {
            var request = TestDataBuilders.ValidTeacherSubjectRequest(teacherId: 1, subjectId: 1);
            _teacherDataMock.Setup(d => d.IsTeacherExistAsync(1)).ReturnsAsync(true);
            _subjectDataMock.Setup(d => d.IsSubjectExistAsync(1)).ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.RemoveSubjectFromTeacherAsync(request));
        }

        [Fact]
        public async Task RemoveSubjectFromTeacherAsync_Throws_WhenRelationDoesNotExist()
        {
            var request = TestDataBuilders.ValidTeacherSubjectRequest(teacherId: 1, subjectId: 1);
            _teacherDataMock.Setup(d => d.IsTeacherExistAsync(1)).ReturnsAsync(true);
            _subjectDataMock.Setup(d => d.IsSubjectExistAsync(1)).ReturnsAsync(true);
            _teacherSubjectDataMock.Setup(d => d.IsTeacherSubjectExistAsync(request)).ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.RemoveSubjectFromTeacherAsync(request));
        }

        [Fact]
        public async Task RemoveSubjectFromTeacherAsync_ReturnsTrue_WhenRemovedSuccessfully()
        {
            var request = TestDataBuilders.ValidTeacherSubjectRequest(teacherId: 1, subjectId: 1);
            _teacherDataMock.Setup(d => d.IsTeacherExistAsync(1)).ReturnsAsync(true);
            _subjectDataMock.Setup(d => d.IsSubjectExistAsync(1)).ReturnsAsync(true);
            _teacherSubjectDataMock.Setup(d => d.IsTeacherSubjectExistAsync(request)).ReturnsAsync(true);
            _teacherSubjectDataMock.Setup(d => d.RemoveSubjectFromTeacherAsync(request)).ReturnsAsync(true);

            var result = await _sut.RemoveSubjectFromTeacherAsync(request);

            Assert.True(result);
        }

        [Fact]
        public async Task RemoveSubjectFromTeacherAsync_Throws_WhenDataLayerFailsToRemove()
        {
            var request = TestDataBuilders.ValidTeacherSubjectRequest(teacherId: 1, subjectId: 1);
            _teacherDataMock.Setup(d => d.IsTeacherExistAsync(1)).ReturnsAsync(true);
            _subjectDataMock.Setup(d => d.IsSubjectExistAsync(1)).ReturnsAsync(true);
            _teacherSubjectDataMock.Setup(d => d.IsTeacherSubjectExistAsync(request)).ReturnsAsync(true);
            _teacherSubjectDataMock.Setup(d => d.RemoveSubjectFromTeacherAsync(request)).ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.RemoveSubjectFromTeacherAsync(request));
        }
        #endregion
    }
}