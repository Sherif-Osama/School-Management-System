using Moq;
using School.BLL;
using School.DAL.Interfaces;
using School.DTO.ParentsDTOs.Responses;
using School.DTO.StudentsDTOs.Responses;
using School.Tests.TestHelpers;
using Xunit;

namespace School.Tests.Services
{
    public class ParentServiceTests
    {
        private readonly Mock<IParentData> _parentDataMock = new();
        private readonly Mock<IPersonData> _personDataMock = new();
        private readonly Mock<IStudentData> _studentDataMock = new();

        private readonly ParentService _sut;

        public ParentServiceTests()
        {
            _sut = new ParentService(
                _parentDataMock.Object,
                _personDataMock.Object,
                _studentDataMock.Object);
        }

        #region Helpers
        private void SetupAddHappyPath(int personId)
        {
            _personDataMock
                .Setup(d => d.IsPersonExistAsync(personId))
                .ReturnsAsync(true);

            _parentDataMock
                .Setup(d => d.GetParentByPersonIdAsync(personId))
                .ReturnsAsync((ParentResponse?)null);

            _studentDataMock
                .Setup(d => d.GetStudentByPersonIdAsync(personId))
                .ReturnsAsync((StudentResponse?)null);
        }

        #endregion

        #region Get

        [Fact]
        public async Task GetParentByIdAsync_Throws_WhenNotFound()
        {
            _parentDataMock
                .Setup(d => d.GetParentByIdAsync(1))
                .ReturnsAsync((ParentResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.GetParentByIdAsync(1));
        }

        [Fact]
        public async Task GetParentByPersonIdAsync_Throws_WhenNotFound()
        {
            _parentDataMock
                .Setup(d => d.GetParentByPersonIdAsync(1))
                .ReturnsAsync((ParentResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.GetParentByPersonIdAsync(1));
        }

        [Fact]
        public async Task GetParentByNationalIdAsync_Throws_WhenNotFound()
        {
            _parentDataMock
                .Setup(d => d.GetParentByNationalIdAsync("12345678901234"))
                .ReturnsAsync((ParentResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.GetParentByNationalIdAsync("12345678901234"));
        }

        #endregion

        #region Add

        [Fact]
        public async Task AddParentAsync_Throws_WhenParentIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _sut.AddParentAsync(null!));
        }

        [Fact]
        public async Task AddParentAsync_Throws_WhenPersonDoesNotExist()
        {
            var parent = TestDataBuilders.ValidCreateParentRequest();

            _personDataMock
                .Setup(d => d.IsPersonExistAsync(parent.PersonID))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.AddParentAsync(parent));
        }

        [Fact]
        public async Task AddParentAsync_Throws_WhenParentAlreadyExistsForPerson()
        {
            var parent = TestDataBuilders.ValidCreateParentRequest();

            _personDataMock
                .Setup(d => d.IsPersonExistAsync(parent.PersonID))
                .ReturnsAsync(true);

            _parentDataMock
                .Setup(d => d.GetParentByPersonIdAsync(parent.PersonID))
                .ReturnsAsync(TestDataBuilders.ValidParent());

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.AddParentAsync(parent));
        }

        [Fact]
        public async Task AddParentAsync_Throws_WhenPersonIsAlreadyStudent()
        {
            var parent = TestDataBuilders.ValidCreateParentRequest();

            _personDataMock
                .Setup(d => d.IsPersonExistAsync(parent.PersonID))
                .ReturnsAsync(true);

            _parentDataMock
                .Setup(d => d.GetParentByPersonIdAsync(parent.PersonID))
                .ReturnsAsync((ParentResponse?)null);

            _studentDataMock
                .Setup(d => d.GetStudentByPersonIdAsync(parent.PersonID))
                .ReturnsAsync(TestDataBuilders.ValidStudent());

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.AddParentAsync(parent));
        }

        [Fact]
        public async Task AddParentAsync_ReturnsNewId_WhenParentIsAdded()
        {
            var parent = TestDataBuilders.ValidCreateParentRequest();

            SetupAddHappyPath(parent.PersonID);

            _parentDataMock
                .Setup(d => d.AddParentAsync(parent))
                .ReturnsAsync(10);

            var result = await _sut.AddParentAsync(parent);

            Assert.Equal(10, result);
        }

        [Fact]
        public async Task AddParentAsync_Throws_WhenDataLayerFailsToInsert()
        {
            var parent = TestDataBuilders.ValidCreateParentRequest();

            SetupAddHappyPath(parent.PersonID);

            _parentDataMock
                .Setup(d => d.AddParentAsync(parent))
                .ReturnsAsync(0);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.AddParentAsync(parent));
        }

        #endregion

        #region Delete
        [Fact]
        public async Task DeleteParentAsync_Throws_WhenParentDoesNotExist()
        {
            _parentDataMock
                .Setup(d => d.IsParentExistAsync(1))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.DeleteParentAsync(1));
        }

        [Fact]
        public async Task DeleteParentAsync_ReturnsTrue_WhenParentIsDeleted()
        {
            _parentDataMock
                .Setup(d => d.IsParentExistAsync(1))
                .ReturnsAsync(true);

            _parentDataMock
                .Setup(d => d.DeleteParentAsync(1))
                .ReturnsAsync(true);

            var result = await _sut.DeleteParentAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteParentAsync_Throws_WhenDataLayerFailsToDelete()
        {
            _parentDataMock
                .Setup(d => d.IsParentExistAsync(1))
                .ReturnsAsync(true);

            _parentDataMock
                .Setup(d => d.DeleteParentAsync(1))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.DeleteParentAsync(1));
        }

        #endregion

        #region IsParentExist
        [Fact]
        public async Task IsParentExistAsync_ReturnsTrue_WhenParentExists()
        {
            _parentDataMock
                .Setup(d => d.IsParentExistAsync(1))
                .ReturnsAsync(true);

            var result = await _sut.IsParentExistAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task IsParentExistAsync_ReturnsFalse_WhenParentDoesNotExist()
        {
            _parentDataMock
                .Setup(d => d.IsParentExistAsync(1))
                .ReturnsAsync(false);

            var result = await _sut.IsParentExistAsync(1);

            Assert.False(result);
        }

        #endregion
    }
}