using Moq;
using School.BLL;
using School.DAL.Interfaces;
using School.DTO.AssociationsDTOs.ClassSubjectDTOs.Responses;
using School.Tests.TestHelpers;
using Xunit;

namespace School.Tests.Services
{
    public class ScheduleServiceTests
    {
        private readonly Mock<IScheduleData> _scheduleDataMock = new();
        private readonly Mock<IClassSubjectData> _classSubjectDataMock = new();
        private readonly Mock<IClassroomData> _classroomDataMock = new();
        private readonly Mock<ITeacherData> _teacherDataMock = new();
        private readonly ScheduleService _sut;

        public ScheduleServiceTests()
        {
            _sut = new ScheduleService(
                _scheduleDataMock.Object,
                _classSubjectDataMock.Object,
                _classroomDataMock.Object,
                _teacherDataMock.Object);
        }

        #region Helpers
        private void SetupHappyPath(ClassSubjectResponse classSubject)
        {
            _classSubjectDataMock
                .Setup(d => d.GetClassSubjectByIdAsync(classSubject.ClassSubjectID))
                .ReturnsAsync(classSubject);

            _classroomDataMock.Setup(d => d.IsClassroomExistAsync(It.IsAny<int>())).ReturnsAsync(true);

            _scheduleDataMock
                .Setup(d => d.IsClassroomAvailableAsync(It.IsAny<int>(), It.IsAny<byte>(), It.IsAny<TimeOnly>(), It.IsAny<TimeOnly>(), It.IsAny<int?>()))
                .ReturnsAsync(true);

            _scheduleDataMock
                .Setup(d => d.IsTeacherAvailableAsync(It.IsAny<int>(), It.IsAny<byte>(), It.IsAny<TimeOnly>(), It.IsAny<TimeOnly>(), It.IsAny<int?>()))
                .ReturnsAsync(true);

            _scheduleDataMock
                .Setup(d => d.IsClassAvailableAsync(It.IsAny<int>(), It.IsAny<byte>(), It.IsAny<TimeOnly>(), It.IsAny<TimeOnly>(), It.IsAny<int?>()))
                .ReturnsAsync(true);
        }
        #endregion

        #region AddScheduleAsync
        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        [InlineData(255)]
        public async Task AddScheduleAsync_Throws_WhenDayOfWeekIsOutOfRange(byte dayOfWeek)
        {
            var request = TestDataBuilders.ValidCreateScheduleRequest(dayOfWeek: dayOfWeek);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _sut.AddScheduleAsync(request));
        }

        [Fact]
        public async Task AddScheduleAsync_Throws_WhenStartTimeIsAfterEndTime()
        {
            var request = TestDataBuilders.ValidCreateScheduleRequest(
                startTime: new TimeOnly(11, 0), endTime: new TimeOnly(10, 0));

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.AddScheduleAsync(request));
        }

        [Fact]
        public async Task AddScheduleAsync_Throws_WhenStartTimeEqualsEndTime()
        {
            var sameTime = new TimeOnly(10, 0);
            var request = TestDataBuilders.ValidCreateScheduleRequest(startTime: sameTime, endTime: sameTime);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.AddScheduleAsync(request));
        }
        #endregion

        #region AddScheduleAsync — Existence checks
        [Fact]
        public async Task AddScheduleAsync_Throws_WhenClassSubjectDoesNotExist()
        {
            var request = TestDataBuilders.ValidCreateScheduleRequest();
            _classSubjectDataMock
                .Setup(d => d.GetClassSubjectByIdAsync(request.ClassSubjectID))
                .ReturnsAsync((ClassSubjectResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.AddScheduleAsync(request));
        }

        [Fact]
        public async Task AddScheduleAsync_Throws_WhenClassroomDoesNotExist()
        {
            var request = TestDataBuilders.ValidCreateScheduleRequest();
            var classSubject = TestDataBuilders.ValidClassSubject(classSubjectId: request.ClassSubjectID);

            _classSubjectDataMock.Setup(d => d.GetClassSubjectByIdAsync(request.ClassSubjectID)).ReturnsAsync(classSubject);
            _classroomDataMock.Setup(d => d.IsClassroomExistAsync(request.ClassroomID)).ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.AddScheduleAsync(request));
        }
        #endregion

        #region AddScheduleAsync — Conflict detection (the core business rule)
        [Fact]
        public async Task AddScheduleAsync_Throws_WhenClassroomIsAlreadyBooked()
        {
            var request = TestDataBuilders.ValidCreateScheduleRequest();
            var classSubject = TestDataBuilders.ValidClassSubject(classSubjectId: request.ClassSubjectID);
            SetupHappyPath(classSubject);

            _scheduleDataMock
                .Setup(d => d.IsClassroomAvailableAsync(request.ClassroomID, request.DayOfWeek, request.StartTime, request.EndTime, null))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AddScheduleAsync(request));
        }

        [Fact]
        public async Task AddScheduleAsync_Throws_WhenTeacherIsAlreadyBooked()
        {
            var request = TestDataBuilders.ValidCreateScheduleRequest();
            var classSubject = TestDataBuilders.ValidClassSubject(classSubjectId: request.ClassSubjectID, teacherId: 7);
            SetupHappyPath(classSubject);

            _scheduleDataMock
                .Setup(d => d.IsTeacherAvailableAsync(7, request.DayOfWeek, request.StartTime, request.EndTime, null))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AddScheduleAsync(request));
        }

        [Fact]
        public async Task AddScheduleAsync_Throws_WhenClassIsAlreadyBooked()
        {
            var request = TestDataBuilders.ValidCreateScheduleRequest();
            var classSubject = TestDataBuilders.ValidClassSubject(classSubjectId: request.ClassSubjectID, classId: 30);
            SetupHappyPath(classSubject);

            _scheduleDataMock
                .Setup(d => d.IsClassAvailableAsync(30, request.DayOfWeek, request.StartTime, request.EndTime, null))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AddScheduleAsync(request));
        }

        [Fact]
        public async Task AddScheduleAsync_ReturnsNewId_WhenNoConflictsExist()
        {
            var request = TestDataBuilders.ValidCreateScheduleRequest();
            var classSubject = TestDataBuilders.ValidClassSubject(classSubjectId: request.ClassSubjectID);
            SetupHappyPath(classSubject);
            _scheduleDataMock.Setup(d => d.AddScheduleAsync(request)).ReturnsAsync(15);

            int newId = await _sut.AddScheduleAsync(request);

            Assert.Equal(15, newId);
        }

        [Fact]
        public async Task AddScheduleAsync_Throws_WhenDataLayerFailsToInsert()
        {
            var request = TestDataBuilders.ValidCreateScheduleRequest();
            var classSubject = TestDataBuilders.ValidClassSubject(classSubjectId: request.ClassSubjectID);
            SetupHappyPath(classSubject);
            _scheduleDataMock.Setup(d => d.AddScheduleAsync(request)).ReturnsAsync(0);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AddScheduleAsync(request));
        }
        #endregion

        #region UpdateScheduleAsync — Excludes itself from conflict checks
        [Fact]
        public async Task UpdateScheduleAsync_ExcludesOwnScheduleId_WhenCheckingClassroomAvailability()
        {
            int scheduleId = 5;
            var request = TestDataBuilders.ValidUpdateScheduleRequest();
            var classSubject = TestDataBuilders.ValidClassSubject(classSubjectId: request.ClassSubjectID);

            _scheduleDataMock.Setup(d => d.IsScheduleExistAsync(scheduleId)).ReturnsAsync(true);
            SetupHappyPath(classSubject);
            _scheduleDataMock.Setup(d => d.UpdateScheduleAsync(scheduleId, request)).ReturnsAsync(true);

            await _sut.UpdateScheduleAsync(scheduleId, request);

            _scheduleDataMock.Verify(d => d.IsClassroomAvailableAsync(
                request.ClassroomID, request.DayOfWeek, request.StartTime, request.EndTime, scheduleId), Times.Once);
        }

        [Fact]
        public async Task UpdateScheduleAsync_Throws_WhenScheduleDoesNotExist()
        {
            int scheduleId = 5;
            var request = TestDataBuilders.ValidUpdateScheduleRequest();
            _scheduleDataMock.Setup(d => d.IsScheduleExistAsync(scheduleId)).ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.UpdateScheduleAsync(scheduleId, request));
        }
        #endregion

        #region DeleteScheduleAsync
        [Fact]
        public async Task DeleteScheduleAsync_Throws_WhenScheduleDoesNotExist()
        {
            _scheduleDataMock.Setup(d => d.IsScheduleExistAsync(1)).ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.DeleteScheduleAsync(1));
        }

        [Fact]
        public async Task DeleteScheduleAsync_ReturnsTrue_WhenDeletionSucceeds()
        {
            _scheduleDataMock.Setup(d => d.IsScheduleExistAsync(1)).ReturnsAsync(true);
            _scheduleDataMock.Setup(d => d.DeleteScheduleAsync(1)).ReturnsAsync(true);

            bool result = await _sut.DeleteScheduleAsync(1);

            Assert.True(result);
        }
        #endregion

        #region GetScheduleByIdAsync
        [Fact]
        public async Task GetScheduleByIdAsync_Throws_WhenNotFound()
        {
            _scheduleDataMock.Setup(d => d.GetScheduleByIdAsync(1)).ReturnsAsync((School.DTO.ScheduleDTOs.Responses.ScheduleResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetScheduleByIdAsync(1));
        }

        [Fact]
        public async Task GetScheduleByIdAsync_ReturnsSchedule_WhenFound()
        {
            var schedule = TestDataBuilders.ValidSchedule(scheduleId: 3);
            _scheduleDataMock.Setup(d => d.GetScheduleByIdAsync(3)).ReturnsAsync(schedule);

            var result = await _sut.GetScheduleByIdAsync(3);

            Assert.Equal(3, result.ScheduleID);
        }
        #endregion
    }
}
