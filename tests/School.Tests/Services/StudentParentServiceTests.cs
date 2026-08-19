using Moq;
using School.BLL;
using School.DAL.Interfaces;
using School.DTO.AssociationsDTOs.StudentParentDTOs;
using School.Tests.TestHelpers;
using Xunit;

namespace School.Tests.Services
{
    public class StudentParentServiceTests
    {
        private readonly Mock<IStudentParentData> _studentParentDataMock = new();
        private readonly Mock<IStudentData> _studentDataMock = new();
        private readonly Mock<IParentData> _parentDataMock = new();

        private readonly StudentParentService _sut;

        public StudentParentServiceTests()
        {
            _sut = new StudentParentService(
                _studentParentDataMock.Object,
                _studentDataMock.Object,
                _parentDataMock.Object);
        }

        #region GetParentsByStudentIdAsync
        [Fact]
        public async Task GetParentsByStudentIdAsync_Throws_WhenStudentDoesNotExist()
        {
            _studentDataMock.Setup(d => d.IsStudentExistAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetParentsByStudentIdAsync(1));
        }

        [Fact]
        public async Task GetParentsByStudentIdAsync_ReturnsParents_WhenStudentExists()
        {
            var relations = new List<StudentParentResponse>
            {
                TestDataBuilders.ValidStudentParent( studentId: 1,parentId: 10),

                TestDataBuilders.ValidStudentParent(studentId: 1,parentId: 20)
            };

            _studentDataMock.Setup(d => d.IsStudentExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _studentParentDataMock.Setup(d => d.GetParentsByStudentIdAsync(It.IsAny<int>()))
                .ReturnsAsync(relations);

            var result = await _sut.GetParentsByStudentIdAsync(1);

            Assert.Equal(2, result.Count);
            Assert.All(result, x => Assert.Equal(1, x.StudentID));
        }

        #endregion

        #region GetStudentsByParentIdAsync

        [Fact]
        public async Task GetStudentsByParentIdAsync_Throws_WhenParentDoesNotExist()
        {
            _parentDataMock.Setup(d => d.IsParentExistAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetStudentsByParentIdAsync(10));
        }

        [Fact]
        public async Task GetStudentsByParentIdAsync_ReturnsStudents_WhenParentExists()
        {
            var relations = new List<StudentParentResponse>
            {
                TestDataBuilders.ValidStudentParent( studentId: 1,parentId: 10),

                TestDataBuilders.ValidStudentParent(studentId: 2, parentId: 10)
            };

            _parentDataMock.Setup(d => d.IsParentExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _studentParentDataMock.Setup(d => d.GetStudentsByParentIdAsync(It.IsAny<int>()))
                .ReturnsAsync(relations);

            var result = await _sut.GetStudentsByParentIdAsync(10);

            Assert.Equal(2, result.Count);
            Assert.All(result, x => Assert.Equal(10, x.ParentID));
        }

        #endregion

        #region AddStudentParentAsync

        [Fact]
        public async Task AddStudentParentAsync_Throws_WhenRelationIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.AddStudentParentAsync(null!));
        }

        [Fact]
        public async Task AddStudentParentAsync_Throws_WhenStudentDoesNotExist()
        {
            var relation = TestDataBuilders.ValidStudentParentRequest(studentId: 1, parentId: 10);

            _studentDataMock.Setup(d => d.IsStudentExistAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.AddStudentParentAsync(relation));
        }

        [Fact]
        public async Task AddStudentParentAsync_Throws_WhenParentDoesNotExist()
        {
            var relation = TestDataBuilders.ValidStudentParentRequest(studentId: 1, parentId: 10);

            _studentDataMock.Setup(d => d.IsStudentExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _parentDataMock.Setup(d => d.IsParentExistAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.AddStudentParentAsync(relation));
        }

        [Fact]
        public async Task AddStudentParentAsync_Throws_WhenRelationAlreadyExists()
        {
            var relation = TestDataBuilders.ValidStudentParentRequest(studentId: 1, parentId: 10);

            _studentDataMock.Setup(d => d.IsStudentExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _parentDataMock.Setup(d => d.IsParentExistAsync(10))
                .ReturnsAsync(true);

            _studentParentDataMock.Setup(d => d.IsStudentParentExistAsync(relation))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AddStudentParentAsync(relation));
        }

        [Fact]
        public async Task AddStudentParentAsync_ReturnsTrue_WhenRelationIsAdded()
        {
            var relation = TestDataBuilders.ValidStudentParentRequest(
                studentId: 1,
                parentId: 10);

            _studentDataMock.Setup(d => d.IsStudentExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _parentDataMock.Setup(d => d.IsParentExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _studentParentDataMock.Setup(d => d.IsStudentParentExistAsync(relation))
                .ReturnsAsync(false);

            _studentParentDataMock.Setup(d => d.AddStudentParentAsync(relation))
                .ReturnsAsync(true);

            var result = await _sut.AddStudentParentAsync(relation);

            Assert.True(result);
        }

        [Fact]
        public async Task AddStudentParentAsync_Throws_WhenDataLayerFailsToAddRelation()
        {
            var relation = TestDataBuilders.ValidStudentParentRequest(studentId: 1, parentId: 10);

            _studentDataMock.Setup(d => d.IsStudentExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _parentDataMock.Setup(d => d.IsParentExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _studentParentDataMock.Setup(d => d.IsStudentParentExistAsync(relation))
                .ReturnsAsync(false);

            _studentParentDataMock.Setup(d => d.AddStudentParentAsync(relation))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AddStudentParentAsync(relation));
        }

        #endregion

        #region DeleteStudentParentAsync

        [Fact]
        public async Task DeleteStudentParentAsync_Throws_WhenRelationIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.DeleteStudentParentAsync(null!));
        }

        [Fact]
        public async Task DeleteStudentParentAsync_Throws_WhenStudentDoesNotExist()
        {
            var relation = TestDataBuilders.ValidStudentParentRequest(studentId: 1, parentId: 10);

            _studentDataMock.Setup(d => d.IsStudentExistAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.DeleteStudentParentAsync(relation));
        }

        [Fact]
        public async Task DeleteStudentParentAsync_Throws_WhenParentDoesNotExist()
        {
            var relation = TestDataBuilders.ValidStudentParentRequest(studentId: 1, parentId: 10);

            _studentDataMock.Setup(d => d.IsStudentExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _parentDataMock.Setup(d => d.IsParentExistAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.DeleteStudentParentAsync(relation));
        }

        [Fact]
        public async Task DeleteStudentParentAsync_Throws_WhenRelationDoesNotExist()
        {
            var relation = TestDataBuilders.ValidStudentParentRequest(studentId: 1, parentId: 10);

            _studentDataMock.Setup(d => d.IsStudentExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _parentDataMock.Setup(d => d.IsParentExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _studentParentDataMock.Setup(d => d.IsStudentParentExistAsync(relation))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.DeleteStudentParentAsync(relation));
        }

        [Fact]
        public async Task DeleteStudentParentAsync_ReturnsTrue_WhenRelationIsDeleted()
        {
            var relation = TestDataBuilders.ValidStudentParentRequest(studentId: 1, parentId: 10);

            _studentDataMock.Setup(d => d.IsStudentExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _parentDataMock.Setup(d => d.IsParentExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _studentParentDataMock.Setup(d => d.IsStudentParentExistAsync(relation))
                .ReturnsAsync(true);

            _studentParentDataMock
                .Setup(d => d.DeleteStudentParentAsync(relation))
                .ReturnsAsync(true);

            var result = await _sut.DeleteStudentParentAsync(relation);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteStudentParentAsync_Throws_WhenDataLayerFailsToDeleteRelation()
        {
            var relation = TestDataBuilders.ValidStudentParentRequest(studentId: 1, parentId: 10);

            _studentDataMock.Setup(d => d.IsStudentExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _parentDataMock.Setup(d => d.IsParentExistAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _studentParentDataMock.Setup(d => d.IsStudentParentExistAsync(relation))
                .ReturnsAsync(true);

            _studentParentDataMock.Setup(d => d.DeleteStudentParentAsync(relation))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.DeleteStudentParentAsync(relation));
        }

        #endregion
    }
}