using Moq;
using School.BLL;
using School.BLL.Enums;
using School.DAL.Interfaces;
using School.DTO.AttendanceDTOs.Responses;
using School.DTO.ClassesDTOs.Responses;
using School.DTO.StudentsDTOs.Responses;
using School.Tests.TestHelpers;
using Xunit;
namespace School.Tests.Services
{
    public class AttendanceServiceTests
    {
        private readonly Mock<IAttendanceData> _attendanceDataMock = new();
        private readonly Mock<IStudentData> _studentDataMock = new();
        private readonly Mock<IClassData> _classDataMock = new();
        private readonly Mock<IAttendanceStatusData> _attendanceStatusDataMock = new();

        private readonly AttendanceService _sut;

        private static readonly DateOnly ReferenceDate = new(2026, 3, 15);

        public AttendanceServiceTests()
        {
            _sut = new AttendanceService(_attendanceDataMock.Object, _studentDataMock.Object,
                _classDataMock.Object, _attendanceStatusDataMock.Object);
        }

        #region Helpers

        private void SetupHappyPath(StudentResponse student)
        {
            _studentDataMock
                .Setup(d => d.GetStudentByIdAsync(student.StudentID))
                .ReturnsAsync(student);

            _attendanceStatusDataMock
                .Setup(d => d.IsAttendanceStatusExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _classDataMock
                .Setup(d => d.GetClassByIdAsync(student.ClassID))
                .ReturnsAsync(TestDataBuilders.ValidClass());

            _attendanceDataMock
                .Setup(d => d.IsStudentAttendanceExistsAsync(
                    It.IsAny<int>(),
                    It.IsAny<DateOnly>(),
                    It.IsAny<int?>()))
                .ReturnsAsync(false);
        }

        private void SetupUpdateHappyPath(StudentResponse student)
        {
            _attendanceDataMock
                .Setup(d => d.IsAttendanceExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _studentDataMock
                .Setup(d => d.GetStudentByIdAsync(student.StudentID))
                .ReturnsAsync(student);

            _attendanceStatusDataMock
                .Setup(d => d.IsAttendanceStatusExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _classDataMock
                .Setup(d => d.GetClassByIdAsync(student.ClassID))
                .ReturnsAsync(TestDataBuilders.ValidClass());

            _attendanceDataMock
                .Setup(d => d.IsStudentAttendanceExistsAsync(
                    It.IsAny<int>(),
                    It.IsAny<DateOnly>(),
                    It.IsAny<int?>()))
                .ReturnsAsync(false);
        }
        #endregion

        #region Get
        [Fact]
        public async Task GetAttendanceByIdAsync_ReturnsAttendance_WhenFound()
        {
            _attendanceDataMock.Setup(d => d.GetAttendanceByIdAsync(3)).ReturnsAsync(TestDataBuilders.ValidAttendance(3));

            var result = await _sut.GetAttendanceByIdAsync(3);

            Assert.Equal(3, result.AttendanceID);
        }

        [Fact]
        public async Task GetAttendanceByIdAsync_Throws_WhenNotFound()
        {
            _attendanceDataMock.Setup(d => d.GetAttendanceByIdAsync(1)).ReturnsAsync((AttendanceResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetAttendanceByIdAsync(1));
        }
        #endregion

        #region Add
        [Fact]
        public async Task AddAttendanceAsync_Throws_WhenAttendanceIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.AddAttendanceAsync(null!));
        }

        [Fact]
        public async Task AddAttendanceAsync_Throws_WhenAttendanceDateIsDefault()
        {
            var CreateAttendance = TestDataBuilders.ValidCreateAttendanceRequest(attendanceDate: DateOnly.MinValue);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.AddAttendanceAsync(CreateAttendance));
        }

        [Fact]
        public async Task AddAttendanceAsync_Throws_WhenAttendanceDateIsInFuture()
        {
            var CreateAttendance = TestDataBuilders.ValidCreateAttendanceRequest(
                attendanceDate: DateOnly.FromDateTime(DateTime.Today.AddDays(1)));

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.AddAttendanceAsync(CreateAttendance));
        }

        [Fact]
        public async Task AddAttendanceAsync_Throws_WhenStudentDoesNotExist()
        {
            _studentDataMock.Setup(s => s.GetStudentByIdAsync(It.IsAny<int>())).ReturnsAsync((StudentResponse?)null);

            var attendance = TestDataBuilders.ValidCreateAttendanceRequest();

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.AddAttendanceAsync(attendance));
        }

        [Fact]
        public async Task AddAttendanceAsync_Throws_WhenStudentIsInactive()
        {
            var StudentResponse = TestDataBuilders.ValidStudent(statusId: (int)StudentStatus.Inactive);

            _studentDataMock.Setup(s => s.GetStudentByIdAsync(It.IsAny<int>())).ReturnsAsync(StudentResponse);

            _attendanceStatusDataMock.Setup(s => s.IsAttendanceStatusExistAsync(It.IsAny<int>())).ReturnsAsync(true);

            var attendance = TestDataBuilders.ValidCreateAttendanceRequest();

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AddAttendanceAsync(attendance));
        }

        [Fact]
        public async Task AddAttendanceAsync_Throws_WhenClassDoesNotExist()
        {
            _studentDataMock.Setup(s => s.GetStudentByIdAsync(It.IsAny<int>())).ReturnsAsync(TestDataBuilders.ValidStudent());
            _attendanceStatusDataMock.Setup(s => s.IsAttendanceStatusExistAsync(It.IsAny<int>())).ReturnsAsync(true);
            _classDataMock.Setup(c => c.GetClassByIdAsync(It.IsAny<int>())).ReturnsAsync((ClassResponse?)null);

            var attendance = TestDataBuilders.ValidCreateAttendanceRequest();

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.AddAttendanceAsync(attendance));
        }

        [Fact]
        public async Task AddAttendanceAsync_Throws_WhenClassIsInactive()
        {
            _studentDataMock.Setup(s => s.GetStudentByIdAsync(It.IsAny<int>())).ReturnsAsync(TestDataBuilders.ValidStudent());
            _attendanceStatusDataMock.Setup(s => s.IsAttendanceStatusExistAsync(It.IsAny<int>())).ReturnsAsync(true);

            _classDataMock.Setup(c => c.GetClassByIdAsync(It.IsAny<int>())).ReturnsAsync(TestDataBuilders.ValidClass(isActive: false));

            var attendance = TestDataBuilders.ValidCreateAttendanceRequest();

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AddAttendanceAsync(attendance));
        }

        [Fact]
        public async Task AddAttendanceAsync_Throws_WhenAttendanceDateIsOutsideAcademicYear()
        {
            _studentDataMock.Setup(s => s.GetStudentByIdAsync(It.IsAny<int>())).ReturnsAsync(TestDataBuilders.ValidStudent());

            _attendanceStatusDataMock.Setup(s => s.IsAttendanceStatusExistAsync(It.IsAny<int>())).ReturnsAsync(true);

            _classDataMock.Setup(c => c.GetClassByIdAsync(It.IsAny<int>())).ReturnsAsync(TestDataBuilders.ValidClass());

            var attendance = TestDataBuilders.ValidCreateAttendanceRequest(attendanceDate: new DateOnly(2025, 8, 31));

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.AddAttendanceAsync(attendance));
        }

        [Fact]
        public async Task AddAttendanceAsync_Throws_WhenAttendanceDateIsBeforeEnrollment()
        {
            var attendance = TestDataBuilders.ValidCreateAttendanceRequest(attendanceDate: new DateOnly(2025, 9, 15));

            var student = TestDataBuilders.ValidStudent(enrollmentDate: new DateTime(2025, 10, 1));

            _studentDataMock.Setup(s => s.GetStudentByIdAsync(It.IsAny<int>())).ReturnsAsync(student);

            _attendanceStatusDataMock.Setup(s => s.IsAttendanceStatusExistAsync(It.IsAny<int>())).ReturnsAsync(true);

            _classDataMock.Setup(c => c.GetClassByIdAsync(It.IsAny<int>())).ReturnsAsync(TestDataBuilders.ValidClass());

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.AddAttendanceAsync(attendance));
        }

        [Fact]
        public async Task AddAttendanceAsync_Throws_WhenAttendanceAlreadyExists()
        {
            var attendance = TestDataBuilders.ValidCreateAttendanceRequest(attendanceDate: ReferenceDate);

            var student = TestDataBuilders.ValidStudent();

            SetupHappyPath(student);

            _attendanceDataMock
                .Setup(d => d.IsStudentAttendanceExistsAsync(
                    It.IsAny<int>(),
                    It.IsAny<DateOnly>(),
                    It.IsAny<int?>()))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.AddAttendanceAsync(attendance));
        }

        [Fact]
        public async Task AddAttendanceAsync_ReturnsNewId_WhenAttendanceIsAdded()
        {
            var attendance = TestDataBuilders.ValidCreateAttendanceRequest(attendanceDate: ReferenceDate);

            var student = TestDataBuilders.ValidStudent();

            SetupHappyPath(student);

            _attendanceDataMock
                .Setup(d => d.AddAttendanceAsync(attendance))
                .ReturnsAsync(10);

            int result = await _sut.AddAttendanceAsync(attendance);

            Assert.Equal(10, result);
        }

        [Fact]
        public async Task AddAttendanceAsync_Throws_WhenDataLayerFailsToInsert()
        {
            var attendance = TestDataBuilders.ValidCreateAttendanceRequest(attendanceDate: ReferenceDate);

            var student = TestDataBuilders.ValidStudent();

            SetupHappyPath(student);

            _attendanceDataMock
                .Setup(d => d.AddAttendanceAsync(attendance))
                .ReturnsAsync(0);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.AddAttendanceAsync(attendance));
        }
        #endregion

        #region Update
        [Fact]
        public async Task UpdateAttendanceAsync_Throws_WhenAttendanceIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.UpdateAttendanceAsync(1, 1, null!));
        }

        [Fact]
        public async Task UpdateAttendanceAsync_Throws_WhenAttendanceDateIsDefault()
        {
            var attendance = TestDataBuilders.ValidUpdateAttendanceRequest(
                attendanceDate: DateOnly.MinValue);

            await Assert.ThrowsAsync<ArgumentException>(
                () => _sut.UpdateAttendanceAsync(1, 1, attendance));
        }

        [Fact]
        public async Task UpdateAttendanceAsync_Throws_WhenAttendanceDateIsInFuture()
        {
            var attendance = TestDataBuilders.ValidUpdateAttendanceRequest(
                attendanceDate: DateOnly.FromDateTime(DateTime.Today.AddDays(1)));

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.UpdateAttendanceAsync(1, 1, attendance));
        }

        [Fact]
        public async Task UpdateAttendanceAsync_Throws_WhenAttendanceDoesNotExist()
        {
            _attendanceDataMock
                .Setup(d => d.IsAttendanceExistAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            var attendance = TestDataBuilders.ValidUpdateAttendanceRequest();

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.UpdateAttendanceAsync(1, 1, attendance));
        }

        [Fact]
        public async Task UpdateAttendanceAsync_Throws_WhenStudentDoesNotExist()
        {
            _attendanceDataMock
                .Setup(d => d.IsAttendanceExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _studentDataMock
                .Setup(s => s.GetStudentByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((StudentResponse?)null);

            var attendance = TestDataBuilders.ValidUpdateAttendanceRequest();

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.UpdateAttendanceAsync(1, 1, attendance));
        }

        [Fact]
        public async Task UpdateAttendanceAsync_Throws_WhenStudentIsInactive()
        {
            _attendanceDataMock
                .Setup(d => d.IsAttendanceExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            var student = TestDataBuilders.ValidStudent(
                statusId: (int)StudentStatus.Inactive,
                statusName: "Inactive");

            _studentDataMock
                .Setup(s => s.GetStudentByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(student);

            _attendanceStatusDataMock
                .Setup(s => s.IsAttendanceStatusExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            var attendance = TestDataBuilders.ValidUpdateAttendanceRequest();

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.UpdateAttendanceAsync(1, 1, attendance));
        }

        [Fact]
        public async Task UpdateAttendanceAsync_Throws_WhenAttendanceStatusDoesNotExist()
        {
            _attendanceDataMock
                .Setup(d => d.IsAttendanceExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _studentDataMock
                .Setup(s => s.GetStudentByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(TestDataBuilders.ValidStudent());

            _attendanceStatusDataMock
                .Setup(s => s.IsAttendanceStatusExistAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            var attendance = TestDataBuilders.ValidUpdateAttendanceRequest();

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.UpdateAttendanceAsync(1, 1, attendance));
        }

        [Fact]
        public async Task UpdateAttendanceAsync_Throws_WhenClassDoesNotExist()
        {
            _attendanceDataMock
                .Setup(d => d.IsAttendanceExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _studentDataMock
                .Setup(s => s.GetStudentByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(TestDataBuilders.ValidStudent());

            _attendanceStatusDataMock
                .Setup(s => s.IsAttendanceStatusExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _classDataMock
                .Setup(c => c.GetClassByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((ClassResponse?)null);

            var attendance = TestDataBuilders.ValidUpdateAttendanceRequest();

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.UpdateAttendanceAsync(1, 1, attendance));
        }

        [Fact]
        public async Task UpdateAttendanceAsync_Throws_WhenClassIsInactive()
        {
            _attendanceDataMock
                .Setup(d => d.IsAttendanceExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _studentDataMock
                .Setup(s => s.GetStudentByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(TestDataBuilders.ValidStudent());

            _attendanceStatusDataMock
                .Setup(s => s.IsAttendanceStatusExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _classDataMock
                .Setup(c => c.GetClassByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(TestDataBuilders.ValidClass(isActive: false));

            var attendance = TestDataBuilders.ValidUpdateAttendanceRequest();

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.UpdateAttendanceAsync(1, 1, attendance));
        }

        [Fact]
        public async Task UpdateAttendanceAsync_Throws_WhenAttendanceDateIsOutsideAcademicYear()
        {
            _attendanceDataMock
                .Setup(d => d.IsAttendanceExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _studentDataMock
                .Setup(s => s.GetStudentByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(TestDataBuilders.ValidStudent());

            _attendanceStatusDataMock
                .Setup(s => s.IsAttendanceStatusExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _classDataMock
                .Setup(c => c.GetClassByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(TestDataBuilders.ValidClass());

            var attendance = TestDataBuilders.ValidUpdateAttendanceRequest(
                attendanceDate: new DateOnly(2025, 8, 31));

            await Assert.ThrowsAsync<ArgumentException>(
                () => _sut.UpdateAttendanceAsync(1, 1, attendance));
        }

        [Fact]
        public async Task UpdateAttendanceAsync_Throws_WhenAttendanceDateIsBeforeEnrollment()
        {
            var attendance = TestDataBuilders.ValidUpdateAttendanceRequest(
                attendanceDate: new DateOnly(2025, 9, 15));

            var student = TestDataBuilders.ValidStudent(
                enrollmentDate: new DateTime(2025, 10, 1));

            _attendanceDataMock
                .Setup(d => d.IsAttendanceExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _studentDataMock
                .Setup(s => s.GetStudentByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(student);

            _attendanceStatusDataMock
                .Setup(s => s.IsAttendanceStatusExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _classDataMock
                .Setup(c => c.GetClassByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(TestDataBuilders.ValidClass());

            await Assert.ThrowsAsync<ArgumentException>(
                () => _sut.UpdateAttendanceAsync(1, 1, attendance));
        }

        [Fact]
        public async Task UpdateAttendanceAsync_Throws_WhenAttendanceAlreadyExists()
        {
            var attendance = TestDataBuilders.ValidUpdateAttendanceRequest(attendanceDate: ReferenceDate);

            var student = TestDataBuilders.ValidStudent();

            SetupUpdateHappyPath(student);

            _attendanceDataMock
                .Setup(d => d.IsStudentAttendanceExistsAsync(
                    It.IsAny<int>(),
                    It.IsAny<DateOnly>(),
                    It.IsAny<int?>()))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.UpdateAttendanceAsync(1, 1, attendance));
        }

        [Fact]
        public async Task UpdateAttendanceAsync_ReturnsTrue_WhenAttendanceIsUpdated()
        {
            var attendance = TestDataBuilders.ValidUpdateAttendanceRequest(attendanceDate: ReferenceDate);

            var student = TestDataBuilders.ValidStudent();

            SetupUpdateHappyPath(student);

            _attendanceDataMock
                .Setup(d => d.UpdateAttendanceAsync(1, 1, attendance))
                .ReturnsAsync(true);

            var result = await _sut.UpdateAttendanceAsync(1, 1, attendance);

            Assert.True(result);
        }

        [Fact]
        public async Task UpdateAttendanceAsync_Throws_WhenDataLayerFailsToUpdate()
        {
            var attendance = TestDataBuilders.ValidUpdateAttendanceRequest(attendanceDate: ReferenceDate);

            var student = TestDataBuilders.ValidStudent();

            SetupUpdateHappyPath(student);

            _attendanceDataMock
                .Setup(d => d.UpdateAttendanceAsync(1, 1, attendance))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.UpdateAttendanceAsync(1, 1, attendance));
        }
        #endregion
        #region Delete
        [Fact]
        public async Task DeleteAttendanceAsync_Throws_WhenAttendanceIdIsInvalid()
        {
            await Assert.ThrowsAsync<ArgumentException>(
                () => _sut.DeleteAttendanceAsync(0));
        }

        [Fact]
        public async Task DeleteAttendanceAsync_Throws_WhenAttendanceDoesNotExist()
        {
            _attendanceDataMock
                .Setup(d => d.IsAttendanceExistAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.DeleteAttendanceAsync(1));
        }

        [Fact]
        public async Task DeleteAttendanceAsync_ReturnsTrue_WhenAttendanceIsDeleted()
        {
            _attendanceDataMock
                .Setup(d => d.IsAttendanceExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _attendanceDataMock
                .Setup(d => d.DeleteAttendanceAsync(1))
                .ReturnsAsync(true);

            var result = await _sut.DeleteAttendanceAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAttendanceAsync_Throws_WhenDataLayerFailsToDelete()
        {
            _attendanceDataMock
                .Setup(d => d.IsAttendanceExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _attendanceDataMock
                .Setup(d => d.DeleteAttendanceAsync(1))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.DeleteAttendanceAsync(1));
        }
        #endregion
    }
}