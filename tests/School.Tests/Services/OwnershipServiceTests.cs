using Moq;
using School.BLL;
using School.DAL.Interfaces;
using School.DTO.AssociationsDTOs.StudentParentDTOs;
using School.DTO.ParentsDTOs.Responses;
using School.Tests.TestHelpers;
using Xunit;

namespace School.Tests.Services
{
    public class OwnershipServiceTests
    {
        private readonly Mock<IStudentData> _studentDataMock = new();
        private readonly Mock<IParentData> _parentDataMock = new();
        private readonly Mock<IStudentParentData> _studentParentDataMock = new();
        private readonly OwnershipService _sut;

        public OwnershipServiceTests()
        {
            _sut = new OwnershipService(
                _studentDataMock.Object,
                _parentDataMock.Object,
                _studentParentDataMock.Object);
        }

        #region IsOwnStudentAsync
        [Fact]
        public async Task IsOwnStudentAsync_ReturnsFalse_WhenStudentDoesNotExist()
        {
            _studentDataMock
                .Setup(d => d.GetStudentByIdAsync(1))
                .ReturnsAsync((School.DTO.StudentsDTOs.Responses.StudentResponse?)null);

            bool result = await _sut.IsOwnStudentAsync(studentId: 1, currentPersonId: 100);

            Assert.False(result);
        }

        [Fact]
        public async Task IsOwnStudentAsync_ReturnsTrue_WhenCurrentPersonIsTheStudentHimself()
        {
            var student = TestDataBuilders.ValidStudent(studentId: 1);

            _studentDataMock.Setup(d => d.GetStudentByIdAsync(1)).ReturnsAsync(student);

            bool result = await _sut.IsOwnStudentAsync(studentId: 1, currentPersonId: 100);

            Assert.True(result);

            _parentDataMock.Verify(d => d.GetParentByPersonIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task IsOwnStudentAsync_ReturnsFalse_WhenCurrentPersonIsNotStudentAndHasNoParentRecord()
        {
            var student = TestDataBuilders.ValidStudent(studentId: 1);

            _studentDataMock.Setup(d => d.GetStudentByIdAsync(1)).ReturnsAsync(student);

            _parentDataMock
                .Setup(d => d.GetParentByPersonIdAsync(999))
                .ReturnsAsync((ParentResponse?)null);

            bool result = await _sut.IsOwnStudentAsync(studentId: 1, currentPersonId: 999);

            Assert.False(result);

            _studentParentDataMock.Verify(d => d.GetParentsByStudentIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task IsOwnStudentAsync_ReturnsFalse_WhenCurrentPersonIsAParentButNotLinkedToThisStudent()
        {
            var student = TestDataBuilders.ValidStudent(studentId: 1);
            _studentDataMock.Setup(d => d.GetStudentByIdAsync(1)).ReturnsAsync(student);

            var parent = TestDataBuilders.ValidParent(parentId: 5, personId: 999);
            _parentDataMock.Setup(d => d.GetParentByPersonIdAsync(999)).ReturnsAsync(parent);

            _studentParentDataMock
                .Setup(d => d.GetParentsByStudentIdAsync(1))
                .ReturnsAsync([new StudentParentResponse { StudentID = 1, ParentID = 77 }]);

            bool result = await _sut.IsOwnStudentAsync(studentId: 1, currentPersonId: 999);

            Assert.False(result);
        }

        [Fact]
        public async Task IsOwnStudentAsync_ReturnsTrue_WhenCurrentPersonIsALinkedParentOfThisStudent()
        {
            var student = TestDataBuilders.ValidStudent(studentId: 1);
            _studentDataMock.Setup(d => d.GetStudentByIdAsync(1)).ReturnsAsync(student);

            var parent = TestDataBuilders.ValidParent(parentId: 5, personId: 999);
            _parentDataMock.Setup(d => d.GetParentByPersonIdAsync(999)).ReturnsAsync(parent);

            _studentParentDataMock
                .Setup(d => d.GetParentsByStudentIdAsync(1))
                .ReturnsAsync(
                [
                    new StudentParentResponse { StudentID = 1, ParentID = 77 },
                    new StudentParentResponse { StudentID = 1, ParentID = 5 }
                ]);

            bool result = await _sut.IsOwnStudentAsync(studentId: 1, currentPersonId: 999);

            Assert.True(result);
        }
        #endregion

        #region IsOwnParentRecordAsync
        [Fact]
        public async Task IsOwnParentRecordAsync_ReturnsFalse_WhenParentDoesNotExist()
        {
            _parentDataMock
                .Setup(d => d.GetParentByIdAsync(1))
                .ReturnsAsync((School.DTO.ParentsDTOs.Responses.ParentResponse?)null);

            bool result = await _sut.IsOwnParentRecordAsync(parentId: 1, currentPersonId: 100);

            Assert.False(result);
        }

        [Fact]
        public async Task IsOwnParentRecordAsync_ReturnsTrue_WhenPersonIdMatchesParentOwner()
        {
            var parent = TestDataBuilders.ValidParent(parentId: 1, personId: 100);
            _parentDataMock.Setup(d => d.GetParentByIdAsync(1)).ReturnsAsync(parent);

            bool result = await _sut.IsOwnParentRecordAsync(parentId: 1, currentPersonId: 100);

            Assert.True(result);
        }

        [Fact]
        public async Task IsOwnParentRecordAsync_ReturnsFalse_WhenPersonIdDoesNotMatchParentOwner()
        {
            var parent = TestDataBuilders.ValidParent(parentId: 1, personId: 100);
            _parentDataMock.Setup(d => d.GetParentByIdAsync(1)).ReturnsAsync(parent);

            bool result = await _sut.IsOwnParentRecordAsync(parentId: 1, currentPersonId: 999);

            Assert.False(result);
        }
        #endregion
    }
}