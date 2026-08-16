using Moq;
using School.BLL;
using School.DAL.Interfaces;
using School.DTO.PersonDTOs.Responses;
using School.Tests.TestHelpers;
using Xunit;

namespace School.Tests.Services
{
    public class PersonServiceTests
    {
        private readonly Mock<IPersonData> _personDataMock = new();

        private readonly PersonService _sut;

        public PersonServiceTests()
        {
            _sut = new PersonService(_personDataMock.Object);
        }

        #region Get

        [Fact]
        public async Task GetPersonByIdAsync_ReturnsPerson_WhenFound()
        {
            var person = TestDataBuilders.ValidPerson(personId: 3);

            _personDataMock
                .Setup(d => d.GetPersonByIdAsync(3))
                .ReturnsAsync(person);

            var result = await _sut.GetPersonByIdAsync(3);

            Assert.Equal(3, result.PersonID);
        }

        [Fact]
        public async Task GetPersonByIdAsync_Throws_WhenNotFound()
        {
            _personDataMock
                .Setup(d => d.GetPersonByIdAsync(1))
                .ReturnsAsync((PersonResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.GetPersonByIdAsync(1));
        }

        [Fact]
        public async Task GetPersonByNationalIDAsync_ReturnsPerson_WhenFound()
        {
            var person = TestDataBuilders.ValidPerson();

            _personDataMock
                .Setup(d => d.GetPersonByNationalIDAsync("12345678901234"))
                .ReturnsAsync(person);

            var result =
                await _sut.GetPersonByNationalIDAsync("12345678901234");

            Assert.Equal("12345678901234", result.NationalID);
        }

        [Fact]
        public async Task GetPersonByNationalIDAsync_Throws_WhenNotFound()
        {
            _personDataMock
                .Setup(d => d.GetPersonByNationalIDAsync("12345678901234"))
                .ReturnsAsync((PersonResponse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.GetPersonByNationalIDAsync("12345678901234"));
        }

        #endregion

        #region Add

        [Fact]
        public async Task AddPersonAsync_Throws_WhenPersonIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _sut.AddPersonAsync(null!));
        }

        [Fact]
        public async Task AddPersonAsync_Throws_WhenDateOfBirthIsDefault()
        {
            var person =
                TestDataBuilders.ValidCreatePersonRequest(
                    dateOfBirth: default(DateTime));

            await Assert.ThrowsAsync<ArgumentException>(
                () => _sut.AddPersonAsync(person));
        }

        [Fact]
        public async Task AddPersonAsync_Throws_WhenDateOfBirthIsInFuture()
        {
            var person =
                TestDataBuilders.ValidCreatePersonRequest(
                    dateOfBirth: DateTime.Today.AddDays(1));

            await Assert.ThrowsAsync<ArgumentException>(
                () => _sut.AddPersonAsync(person));
        }

        [Fact]
        public async Task AddPersonAsync_Throws_WhenEmailIsInvalid()
        {
            var person =
                TestDataBuilders.ValidCreatePersonRequest(
                    email: "invalid-email");

            await Assert.ThrowsAsync<ArgumentException>(
                () => _sut.AddPersonAsync(person));
        }

        [Fact]
        public async Task AddPersonAsync_Throws_WhenPersonAlreadyExists()
        {
            var person =
                TestDataBuilders.ValidCreatePersonRequest();

            _personDataMock
                .Setup(d => d.GetPersonByNationalIDAsync(
                    person.NationalID))
                .ReturnsAsync(TestDataBuilders.ValidPerson());

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.AddPersonAsync(person));
        }

        [Fact]
        public async Task AddPersonAsync_ReturnsNewId_WhenPersonIsAdded()
        {
            var person =
                TestDataBuilders.ValidCreatePersonRequest();

            _personDataMock
                .Setup(d => d.GetPersonByNationalIDAsync(
                    person.NationalID))
                .ReturnsAsync((PersonResponse?)null);

            _personDataMock
                .Setup(d => d.AddPersonAsync(person))
                .ReturnsAsync(10);

            var result = await _sut.AddPersonAsync(person);

            Assert.Equal(10, result);
        }

        [Fact]
        public async Task AddPersonAsync_Throws_WhenDataLayerFailsToInsert()
        {
            var person =
                TestDataBuilders.ValidCreatePersonRequest();

            _personDataMock
                .Setup(d => d.GetPersonByNationalIDAsync(
                    person.NationalID))
                .ReturnsAsync((PersonResponse?)null);

            _personDataMock
                .Setup(d => d.AddPersonAsync(person))
                .ReturnsAsync(0);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.AddPersonAsync(person));
        }

        #endregion

        #region Update

        [Fact]
        public async Task UpdatePersonAsync_Throws_WhenPersonIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _sut.UpdatePersonAsync(1, null!));
        }

        [Fact]
        public async Task UpdatePersonAsync_Throws_WhenPersonDoesNotExist()
        {
            var person =
                TestDataBuilders.ValidUpdatePersonRequest();

            _personDataMock
                .Setup(d => d.IsPersonExistAsync(1))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.UpdatePersonAsync(1, person));
        }

        [Fact]
        public async Task UpdatePersonAsync_Throws_WhenNationalIdAlreadyExists()
        {
            var person =
                TestDataBuilders.ValidUpdatePersonRequest();

            _personDataMock
                .Setup(d => d.IsPersonExistAsync(1))
                .ReturnsAsync(true);

            _personDataMock
                .Setup(d => d.GetPersonByNationalIDAsync(
                    person.NationalID))
                .ReturnsAsync(
                    TestDataBuilders.ValidPerson(personId: 2));

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.UpdatePersonAsync(1, person));
        }

        [Fact]
        public async Task UpdatePersonAsync_ReturnsTrue_WhenPersonIsUpdated()
        {
            var person =
                TestDataBuilders.ValidUpdatePersonRequest();

            _personDataMock
                .Setup(d => d.IsPersonExistAsync(1))
                .ReturnsAsync(true);

            _personDataMock
                .Setup(d => d.GetPersonByNationalIDAsync(
                    person.NationalID))
                .ReturnsAsync((PersonResponse?)null);

            _personDataMock
                .Setup(d => d.UpdatePersonAsync(1, person))
                .ReturnsAsync(true);

            var result =
                await _sut.UpdatePersonAsync(1, person);

            Assert.True(result);
        }

        [Fact]
        public async Task UpdatePersonAsync_ReturnsTrue_WhenNationalIdBelongsToCurrentPerson()
        {
            var person =
                TestDataBuilders.ValidUpdatePersonRequest();

            _personDataMock
                .Setup(d => d.IsPersonExistAsync(1))
                .ReturnsAsync(true);

            _personDataMock
                .Setup(d => d.GetPersonByNationalIDAsync(
                    person.NationalID))
                .ReturnsAsync(
                    TestDataBuilders.ValidPerson(personId: 1));

            _personDataMock
                .Setup(d => d.UpdatePersonAsync(1, person))
                .ReturnsAsync(true);

            var result =
                await _sut.UpdatePersonAsync(1, person);

            Assert.True(result);
        }

        [Fact]
        public async Task UpdatePersonAsync_Throws_WhenDataLayerFailsToUpdate()
        {
            var person =
                TestDataBuilders.ValidUpdatePersonRequest();

            _personDataMock
                .Setup(d => d.IsPersonExistAsync(1))
                .ReturnsAsync(true);

            _personDataMock
                .Setup(d => d.GetPersonByNationalIDAsync(
                    person.NationalID))
                .ReturnsAsync((PersonResponse?)null);

            _personDataMock
                .Setup(d => d.UpdatePersonAsync(1, person))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.UpdatePersonAsync(1, person));
        }

        #endregion

        #region Delete
        [Fact]
        public async Task DeletePersonAsync_Throws_WhenPersonDoesNotExist()
        {
            _personDataMock
                .Setup(d => d.IsPersonExistAsync(1))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.DeletePersonAsync(1));
        }

        [Fact]
        public async Task DeletePersonAsync_ReturnsTrue_WhenPersonIsDeleted()
        {
            _personDataMock
                .Setup(d => d.IsPersonExistAsync(1))
                .ReturnsAsync(true);

            _personDataMock
                .Setup(d => d.DeletePersonAsync(1))
                .ReturnsAsync(true);

            var result = await _sut.DeletePersonAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeletePersonAsync_Throws_WhenDataLayerFailsToDelete()
        {
            _personDataMock
                .Setup(d => d.IsPersonExistAsync(1))
                .ReturnsAsync(true);

            _personDataMock
                .Setup(d => d.DeletePersonAsync(1))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.DeletePersonAsync(1));
        }

        #endregion

        #region IsPersonExist
        [Fact]
        public async Task IsPersonExistAsync_ReturnsTrue_WhenPersonExists()
        {
            _personDataMock
                .Setup(d => d.IsPersonExistAsync(1))
                .ReturnsAsync(true);

            var result = await _sut.IsPersonExistAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task IsPersonExistAsync_ReturnsFalse_WhenPersonDoesNotExist()
        {
            _personDataMock
                .Setup(d => d.IsPersonExistAsync(1))
                .ReturnsAsync(false);

            var result = await _sut.IsPersonExistAsync(1);

            Assert.False(result);
        }

        #endregion
    }
}