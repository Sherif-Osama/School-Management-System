using Moq;
using School.BLL;
using School.DAL.Interfaces;
using School.DTO.ParentsDTOs.Responses;
using School.DTO.StudentsDTOs.Responses;
using School.DTO.TeachersDTOs.Responses;
using School.Tests.TestHelpers;
using Xunit;

namespace School.Tests.Services
{
    public class StudentServiceTests
    {
        private readonly Mock<IStudentData> _studentDataMock = new();
        private readonly Mock<IPersonData> _personDataMock = new();
        private readonly Mock<IClassData> _classDataMock = new();
        private readonly Mock<ITeacherData> _teacherDataMock = new();
        private readonly Mock<IParentData> _parentDataMock = new();

        private readonly StudentService _sut;

        public StudentServiceTests()
        {
            _sut = new StudentService(
                _studentDataMock.Object,
                _personDataMock.Object,
                _classDataMock.Object,
                _teacherDataMock.Object,
                _parentDataMock.Object);
        }

        #region GetStudentByIdAsync
        [Fact]
        public async Task GetStudentByIdAsync_ReturnsStudent_WhenFound()
        {
            var student = TestDataBuilders.ValidStudent(studentId: 5);

            _studentDataMock.Setup(d => d.GetStudentByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(student);

            var result = await _sut.GetStudentByIdAsync(5);

            Assert.Equal(5, result.StudentID);
            Assert.Equal(student.PersonID, result.PersonID);
            Assert.Equal(student.ClassID, result.ClassID);
        }

        [Fact]
        public async Task GetStudentByIdAsync_Throws_WhenStudentDoesNotExist()
        {
            _studentDataMock.Setup(d => d.GetStudentByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((StudentResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetStudentByIdAsync(5));
        }

        #endregion

        #region GetStudentByPersonIdAsync

        [Fact]
        public async Task GetStudentByPersonIdAsync_ReturnsStudent_WhenFound()
        {
            var student = TestDataBuilders.ValidStudent(studentId: 5);

            _studentDataMock.Setup(d => d.GetStudentByPersonIdAsync(It.IsAny<int>()))
                .ReturnsAsync(student);

            var result = await _sut.GetStudentByPersonIdAsync(100);

            Assert.Equal(5, result.StudentID);
            Assert.Equal(100, result.PersonID);
        }

        [Fact]
        public async Task GetStudentByPersonIdAsync_Throws_WhenStudentDoesNotExist()
        {
            _studentDataMock.Setup(d => d.GetStudentByPersonIdAsync(It.IsAny<int>()))
                .ReturnsAsync((StudentResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetStudentByPersonIdAsync(100));
        }

        #endregion

        #region AddStudentAsync

        [Fact]
        public async Task AddStudentAsync_Throws_WhenEnrollmentDateIsDefault()
        {
            var request = TestDataBuilders.ValidCreateStudentRequest(enrollmentDate: default(DateTime));

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.AddStudentAsync(request));
        }

        [Fact]
        public async Task AddStudentAsync_Throws_WhenEnrollmentDateIsInFuture()
        {
            var request = TestDataBuilders.ValidCreateStudentRequest(enrollmentDate: DateTime.Today.AddDays(1));

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.AddStudentAsync(request));
        }

        [Fact]
        public async Task AddStudentAsync_Throws_WhenPersonIsAlreadyTeacher()
        {
            var request = TestDataBuilders.ValidCreateStudentRequest(
                personId: 100);

            _personDataMock.Setup(d => d.IsPersonExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _classDataMock.Setup(d => d.IsClassExistAsync(It.IsAny<int>())).ReturnsAsync(true);

            _studentDataMock.Setup(d => d.GetStudentByPersonIdAsync(It.IsAny<int>()))
                .ReturnsAsync((StudentResponse?)null);

            _teacherDataMock.Setup(d => d.GetTeacherByPersonIdAsync(It.IsAny<int>()))
                .ReturnsAsync(TestDataBuilders.ValidTeacher());

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AddStudentAsync(request));
        }

        [Fact]
        public async Task AddStudentAsync_Throws_WhenPersonIsAlreadyParent()
        {
            var request = TestDataBuilders.ValidCreateStudentRequest(personId: 100);

            _personDataMock.Setup(d => d.IsPersonExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _classDataMock.Setup(d => d.IsClassExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _studentDataMock.Setup(d => d.GetStudentByPersonIdAsync(It.IsAny<int>()))
                .ReturnsAsync((StudentResponse?)null);

            _teacherDataMock.Setup(d => d.GetTeacherByPersonIdAsync(It.IsAny<int>()))
                .ReturnsAsync((TeacherResponse?)null);

            _parentDataMock.Setup(d => d.GetParentByPersonIdAsync(It.IsAny<int>()))
                .ReturnsAsync(TestDataBuilders.ValidParent());

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AddStudentAsync(request));
        }

        [Fact]
        public async Task AddStudentAsync_Throws_WhenClassHasNoAvailableCapacity()
        {
            var request = TestDataBuilders.ValidCreateStudentRequest(classId: 10);

            _personDataMock.Setup(d => d.IsPersonExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _classDataMock.Setup(d => d.IsClassExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _studentDataMock.Setup(d => d.GetStudentByPersonIdAsync(It.IsAny<int>()))
                .ReturnsAsync((StudentResponse?)null);

            _teacherDataMock.Setup(d => d.GetTeacherByPersonIdAsync(It.IsAny<int>()))
                .ReturnsAsync((TeacherResponse?)null);

            _parentDataMock.Setup(d => d.GetParentByPersonIdAsync(It.IsAny<int>()))
                .ReturnsAsync((ParentResponse?)null);

            _classDataMock.Setup(d => d.HasClassAvailableCapacityAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AddStudentAsync(request));
        }

        [Fact]
        public async Task AddStudentAsync_ReturnsNewStudentId_WhenStudentIsAdded()
        {
            var request = TestDataBuilders.ValidCreateStudentRequest(personId: 100, classId: 10);

            _personDataMock.Setup(d => d.IsPersonExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _classDataMock.Setup(d => d.IsClassExistAsync(10))
                .ReturnsAsync(true);

            _studentDataMock.Setup(d => d.GetStudentByPersonIdAsync(It.IsAny<int>()))
                .ReturnsAsync((StudentResponse?)null);

            _teacherDataMock.Setup(d => d.GetTeacherByPersonIdAsync(It.IsAny<int>()))
                .ReturnsAsync((TeacherResponse?)null);

            _parentDataMock.Setup(d => d.GetParentByPersonIdAsync(It.IsAny<int>()))
                .ReturnsAsync((ParentResponse?)null);

            _classDataMock.Setup(d => d.HasClassAvailableCapacityAsync(10))
                .ReturnsAsync(true);

            _studentDataMock.Setup(d => d.AddStudentAsync(request))
                .ReturnsAsync(25);

            var result = await _sut.AddStudentAsync(request);

            Assert.Equal(25, result);
        }

        [Fact]
        public async Task AddStudentAsync_Throws_WhenDataLayerFailsToAddStudent()
        {
            var request = TestDataBuilders.ValidCreateStudentRequest(personId: 100, classId: 10);

            _personDataMock.Setup(d => d.IsPersonExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _classDataMock.Setup(d => d.IsClassExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _studentDataMock.Setup(d => d.GetStudentByPersonIdAsync(It.IsAny<int>()))
                .ReturnsAsync((StudentResponse?)null);

            _teacherDataMock.Setup(d => d.GetTeacherByPersonIdAsync(It.IsAny<int>()))
                .ReturnsAsync((TeacherResponse?)null);

            _parentDataMock.Setup(d => d.GetParentByPersonIdAsync(It.IsAny<int>()))
                .ReturnsAsync((ParentResponse?)null);

            _classDataMock.Setup(d => d.HasClassAvailableCapacityAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _studentDataMock.Setup(d => d.AddStudentAsync(request))
                .ReturnsAsync(0);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AddStudentAsync(request));
        }

        #endregion

        #region UpdateStudentAsync

        [Fact]
        public async Task UpdateStudentAsync_Throws_WhenEnrollmentDateIsDefault()
        {
            var request = TestDataBuilders.ValidUpdateStudentRequest(enrollmentDate: default(DateTime));

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.UpdateStudentAsync(1, request));
        }

        [Fact]
        public async Task UpdateStudentAsync_Throws_WhenEnrollmentDateIsInFuture()
        {
            var request = TestDataBuilders.ValidUpdateStudentRequest(enrollmentDate: DateTime.Today.AddDays(1));

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.UpdateStudentAsync(1, request));
        }

        [Fact]
        public async Task UpdateStudentAsync_Throws_WhenStudentDoesNotExist()
        {
            var request = TestDataBuilders.ValidUpdateStudentRequest();

            _studentDataMock.Setup(d => d.GetStudentByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((StudentResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.UpdateStudentAsync(1, request));
        }

        [Fact]
        public async Task UpdateStudentAsync_ReturnsTrue_WhenClassDoesNotChange()
        {
            var currentStudent = TestDataBuilders.ValidStudent(studentId: 1, classId: 10);

            var request = TestDataBuilders.ValidUpdateStudentRequest(classId: 10);

            _studentDataMock.Setup(d => d.GetStudentByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(currentStudent);

            _classDataMock.Setup(d => d.IsClassExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _studentDataMock.Setup(d => d.UpdateStudentAsync(1, request))
                .ReturnsAsync(true);

            var result = await _sut.UpdateStudentAsync(1, request);

            Assert.True(result);

            _classDataMock.Verify(d => d.HasClassAvailableCapacityAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task UpdateStudentAsync_Throws_WhenNewClassHasNoAvailableCapacity()
        {
            var currentStudent = TestDataBuilders.ValidStudent(studentId: 1, classId: 10);

            var request = TestDataBuilders.ValidUpdateStudentRequest(classId: 20);

            _studentDataMock.Setup(d => d.GetStudentByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(currentStudent);

            _classDataMock.Setup(d => d.IsClassExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _classDataMock.Setup(d => d.HasClassAvailableCapacityAsync(20))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.UpdateStudentAsync(1, request));
        }

        [Fact]
        public async Task UpdateStudentAsync_ReturnsTrue_WhenStudentIsUpdated()
        {
            var currentStudent = TestDataBuilders.ValidStudent(studentId: 1, classId: 10);

            var request = TestDataBuilders.ValidUpdateStudentRequest(classId: 20);

            _studentDataMock.Setup(d => d.GetStudentByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(currentStudent);

            _classDataMock.Setup(d => d.IsClassExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _classDataMock.Setup(d => d.HasClassAvailableCapacityAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _studentDataMock.Setup(d => d.UpdateStudentAsync(It.IsAny<int>(), request))
                .ReturnsAsync(true);

            var result = await _sut.UpdateStudentAsync(1, request);

            Assert.True(result);
        }

        [Fact]
        public async Task UpdateStudentAsync_Throws_WhenDataLayerFailsToUpdate()
        {
            var currentStudent = TestDataBuilders.ValidStudent(studentId: 1, classId: 10);

            var request = TestDataBuilders.ValidUpdateStudentRequest(classId: 20);

            _studentDataMock.Setup(d => d.GetStudentByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(currentStudent);

            _classDataMock.Setup(d => d.IsClassExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _classDataMock.Setup(d => d.HasClassAvailableCapacityAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _studentDataMock.Setup(d => d.UpdateStudentAsync(1, request))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.UpdateStudentAsync(1, request));
        }

        #endregion

        #region DeleteStudentAsync

        [Fact]
        public async Task DeleteStudentAsync_Throws_WhenStudentDoesNotExist()
        {
            _studentDataMock.Setup(d => d.GetStudentByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((StudentResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.DeleteStudentAsync(1));
        }

        [Fact]
        public async Task DeleteStudentAsync_ReturnsTrue_WhenStudentIsDeleted()
        {
            var student = TestDataBuilders.ValidStudent(studentId: 1);

            _studentDataMock.Setup(d => d.GetStudentByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(student);

            _studentDataMock.Setup(d => d.DeleteStudentAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            var result = await _sut.DeleteStudentAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteStudentAsync_Throws_WhenDataLayerFailsToDelete()
        {
            var student = TestDataBuilders.ValidStudent(studentId: 1);

            _studentDataMock.Setup(d => d.GetStudentByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(student);

            _studentDataMock.Setup(d => d.DeleteStudentAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.DeleteStudentAsync(1));
        }

        #endregion
    }
}