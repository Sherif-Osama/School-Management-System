using Moq;
using School.BLL;
using School.DAL.Interfaces;
using School.DTO.StudentsDTOs.Responses;
using School.DTO.TeachersDTOs.Responses;
using School.Tests.TestHelpers;
using Xunit;

namespace School.Tests.Services
{
    public class TeacherServiceTests
    {
        private readonly Mock<ITeacherData> _teacherDataMock = new();
        private readonly Mock<IPersonData> _personDataMock = new();
        private readonly Mock<IStudentData> _studentDataMock = new();
        private readonly TeacherService _sut;

        public TeacherServiceTests()
        {
            _sut = new TeacherService(
                _teacherDataMock.Object,
                _personDataMock.Object,
                _studentDataMock.Object);
        }

        #region GetTeacherByIdAsync
        [Fact]
        public async Task GetTeacherByIdAsync_Throws_WhenNotFound()
        {
            _teacherDataMock.Setup(d => d.GetTeacherByIdAsync(It.IsAny<int>())).ReturnsAsync((TeacherResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetTeacherByIdAsync(1));
        }

        [Fact]
        public async Task GetTeacherByIdAsync_ReturnsTeacher_WhenFound()
        {
            var teacher = TestDataBuilders.ValidTeacher(teacherId: 3);
            _teacherDataMock.Setup(d => d.GetTeacherByIdAsync(It.IsAny<int>())).ReturnsAsync(teacher);

            var result = await _sut.GetTeacherByIdAsync(3);

            Assert.Equal(3, result.TeacherID);
        }
        #endregion

        #region GetTeacherByPersonIdAsync
        [Fact]
        public async Task GetTeacherByPersonIdAsync_Throws_WhenNotFound()
        {
            _teacherDataMock.Setup(d => d.GetTeacherByPersonIdAsync(It.IsAny<int>())).ReturnsAsync((TeacherResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetTeacherByPersonIdAsync(100));
        }

        [Fact]
        public async Task GetTeacherByPersonIdAsync_ReturnsTeacher_WhenFound()
        {
            var teacher = TestDataBuilders.ValidTeacher(personId: 100);
            _teacherDataMock.Setup(d => d.GetTeacherByPersonIdAsync(It.IsAny<int>())).ReturnsAsync(teacher);

            var result = await _sut.GetTeacherByPersonIdAsync(100);

            Assert.Equal(100, result.PersonID);
        }
        #endregion

        #region GetTeacherByNationalIdAsync
        [Fact]
        public async Task GetTeacherByNationalIdAsync_Throws_WhenNotFound()
        {
            _teacherDataMock.Setup(d => d.GetTeacherByNationalIdAsync(It.IsAny<string>())).ReturnsAsync((TeacherResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetTeacherByNationalIdAsync("12345678901234"));
        }

        [Fact]
        public async Task GetTeacherByNationalIdAsync_ReturnsTeacher_WhenFound()
        {
            var teacher = TestDataBuilders.ValidTeacher(nationalId: "12345678901234");
            _teacherDataMock.Setup(d => d.GetTeacherByNationalIdAsync(It.IsAny<string>())).ReturnsAsync(teacher);

            var result = await _sut.GetTeacherByNationalIdAsync("12345678901234");

            Assert.Equal("12345678901234", result.NationalID);
        }
        #endregion

        #region AddTeacherAsync — Validation
        [Fact]
        public async Task AddTeacherAsync_Throws_WhenTeacherIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.AddTeacherAsync(null!));
        }

        [Fact]
        public async Task AddTeacherAsync_Throws_WhenHireDateIsDefault()
        {
            var request = TestDataBuilders.ValidCreateTeacherRequest(hireDate: default(DateTime));

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.AddTeacherAsync(request));
        }

        [Fact]
        public async Task AddTeacherAsync_Throws_WhenHireDateIsInFuture()
        {
            var request = TestDataBuilders.ValidCreateTeacherRequest(hireDate: DateTime.Today.AddDays(1));

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.AddTeacherAsync(request));
        }

        [Fact]
        public async Task AddTeacherAsync_Throws_WhenSalaryIsZero()
        {
            var request = TestDataBuilders.ValidCreateTeacherRequest(salary: 0);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.AddTeacherAsync(request));
        }

        [Fact]
        public async Task AddTeacherAsync_Throws_WhenSalaryIsNegative()
        {
            var request = TestDataBuilders.ValidCreateTeacherRequest(salary: -100);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.AddTeacherAsync(request));
        }
        #endregion

        #region AddTeacherAsync — Business rules
        [Fact]
        public async Task AddTeacherAsync_Throws_WhenPersonDoesNotExist()
        {
            var request = TestDataBuilders.ValidCreateTeacherRequest(personId: 1);
            _personDataMock.Setup(d => d.IsPersonExistAsync(It.IsAny<int>())).ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.AddTeacherAsync(request));
        }

        [Fact]
        public async Task AddTeacherAsync_Throws_WhenPersonIsAlreadyATeacher()
        {
            var request = TestDataBuilders.ValidCreateTeacherRequest(personId: 1);
            _personDataMock.Setup(d => d.IsPersonExistAsync(It.IsAny<int>())).ReturnsAsync(true);
            _teacherDataMock.Setup(d => d.GetTeacherByPersonIdAsync(It.IsAny<int>())).ReturnsAsync(TestDataBuilders.ValidTeacher(personId: 1));

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AddTeacherAsync(request));
        }

        [Fact]
        public async Task AddTeacherAsync_Throws_WhenPersonIsAlreadyAStudent()
        {
            var request = TestDataBuilders.ValidCreateTeacherRequest(personId: 1);
            _personDataMock.Setup(d => d.IsPersonExistAsync(It.IsAny<int>())).ReturnsAsync(true);
            _teacherDataMock.Setup(d => d.GetTeacherByPersonIdAsync(It.IsAny<int>())).ReturnsAsync((TeacherResponse?)null);
            _studentDataMock.Setup(d => d.GetStudentByPersonIdAsync(It.IsAny<int>())).ReturnsAsync(TestDataBuilders.ValidStudent());

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AddTeacherAsync(request));
        }

        [Fact]
        public async Task AddTeacherAsync_ReturnsNewId_WhenTeacherIsAdded()
        {
            var request = TestDataBuilders.ValidCreateTeacherRequest(personId: 1);
            _personDataMock.Setup(d => d.IsPersonExistAsync(It.IsAny<int>())).ReturnsAsync(true);
            _teacherDataMock.Setup(d => d.GetTeacherByPersonIdAsync(It.IsAny<int>())).ReturnsAsync((TeacherResponse?)null);
            _studentDataMock.Setup(d => d.GetStudentByPersonIdAsync(It.IsAny<int>())).ReturnsAsync((StudentResponse?)null);
            _teacherDataMock.Setup(d => d.AddTeacherAsync(request)).ReturnsAsync(12);

            var result = await _sut.AddTeacherAsync(request);

            Assert.Equal(12, result);
        }

        [Fact]
        public async Task AddTeacherAsync_Throws_WhenDataLayerFailsToInsert()
        {
            var request = TestDataBuilders.ValidCreateTeacherRequest(personId: 1);
            _personDataMock.Setup(d => d.IsPersonExistAsync(It.IsAny<int>())).ReturnsAsync(true);
            _teacherDataMock.Setup(d => d.GetTeacherByPersonIdAsync(It.IsAny<int>())).ReturnsAsync((TeacherResponse?)null);
            _studentDataMock.Setup(d => d.GetStudentByPersonIdAsync(It.IsAny<int>())).ReturnsAsync((StudentResponse?)null);
            _teacherDataMock.Setup(d => d.AddTeacherAsync(request)).ReturnsAsync(0);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AddTeacherAsync(request));
        }
        #endregion

        #region UpdateTeacherAsync
        [Fact]
        public async Task UpdateTeacherAsync_Throws_WhenTeacherIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.UpdateTeacherAsync(1, null!));
        }

        [Fact]
        public async Task UpdateTeacherAsync_Throws_WhenHireDateIsInFuture()
        {
            var request = TestDataBuilders.ValidUpdateTeacherRequest(hireDate: DateTime.Today.AddDays(1));

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.UpdateTeacherAsync(1, request));
        }

        [Fact]
        public async Task UpdateTeacherAsync_Throws_WhenSalaryIsZero()
        {
            var request = TestDataBuilders.ValidUpdateTeacherRequest(salary: 0);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.UpdateTeacherAsync(1, request));
        }

        [Fact]
        public async Task UpdateTeacherAsync_Throws_WhenTeacherDoesNotExist()
        {
            var request = TestDataBuilders.ValidUpdateTeacherRequest();
            _teacherDataMock.Setup(d => d.IsTeacherExistAsync(It.IsAny<int>())).ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.UpdateTeacherAsync(1, request));
        }

        [Fact]
        public async Task UpdateTeacherAsync_ReturnsTrue_WhenUpdateSucceeds()
        {
            var request = TestDataBuilders.ValidUpdateTeacherRequest();
            _teacherDataMock.Setup(d => d.IsTeacherExistAsync(It.IsAny<int>())).ReturnsAsync(true);
            _teacherDataMock.Setup(d => d.UpdateTeacherAsync(It.IsAny<int>(), request)).ReturnsAsync(true);

            var result = await _sut.UpdateTeacherAsync(1, request);

            Assert.True(result);
        }

        [Fact]
        public async Task UpdateTeacherAsync_Throws_WhenDataLayerFailsToUpdate()
        {
            var request = TestDataBuilders.ValidUpdateTeacherRequest();
            _teacherDataMock.Setup(d => d.IsTeacherExistAsync(It.IsAny<int>())).ReturnsAsync(true);
            _teacherDataMock.Setup(d => d.UpdateTeacherAsync(It.IsAny<int>(), request)).ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.UpdateTeacherAsync(1, request));
        }
        #endregion

        #region DeleteTeacherAsync
        [Fact]
        public async Task DeleteTeacherAsync_Throws_WhenTeacherDoesNotExist()
        {
            _teacherDataMock.Setup(d => d.IsTeacherExistAsync(It.IsAny<int>())).ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.DeleteTeacherAsync(1));
        }

        [Fact]
        public async Task DeleteTeacherAsync_ReturnsTrue_WhenDeletionSucceeds()
        {
            _teacherDataMock.Setup(d => d.IsTeacherExistAsync(It.IsAny<int>())).ReturnsAsync(true);
            _teacherDataMock.Setup(d => d.DeleteTeacherAsync(It.IsAny<int>())).ReturnsAsync(true);

            var result = await _sut.DeleteTeacherAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteTeacherAsync_Throws_WhenDataLayerFailsToDelete()
        {
            _teacherDataMock.Setup(d => d.IsTeacherExistAsync(It.IsAny<int>())).ReturnsAsync(true);
            _teacherDataMock.Setup(d => d.DeleteTeacherAsync(It.IsAny<int>())).ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.DeleteTeacherAsync(1));
        }
        #endregion

        #region IsTeacherExistAsync
        [Fact]
        public async Task IsTeacherExistAsync_ReturnsTrue_WhenTeacherExists()
        {
            _teacherDataMock.Setup(d => d.IsTeacherExistAsync(It.IsAny<int>())).ReturnsAsync(true);

            var result = await _sut.IsTeacherExistAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task IsTeacherExistAsync_ReturnsFalse_WhenTeacherDoesNotExist()
        {
            _teacherDataMock.Setup(d => d.IsTeacherExistAsync(It.IsAny<int>())).ReturnsAsync(false);

            var result = await _sut.IsTeacherExistAsync(1);

            Assert.False(result);
        }
        #endregion
    }
}