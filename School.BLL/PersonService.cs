using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.PersonDTOs;

namespace School.BLL
{
    public class PersonService : IPersonService
    {
        private readonly IPersonData _personData;

        public PersonService(IPersonData personData)
        {
            _personData = personData;
        }

        #region Private Helpers

        private static void ValidatePerson(PersonDTO person)
        {
            ArgumentNullException.ThrowIfNull(person);

            if (string.IsNullOrWhiteSpace(person.NationalID))
                throw new ArgumentException("National ID is required.", nameof(person.NationalID));

            if (person.NationalID.Length != 14)
                throw new ArgumentException("National ID must contain 14 digits.", nameof(person.NationalID));

            if (string.IsNullOrWhiteSpace(person.FirstName))
                throw new ArgumentException("First name is required.", nameof(person.FirstName));

            if (string.IsNullOrWhiteSpace(person.SecondName))
                throw new ArgumentException("Second name is required.", nameof(person.SecondName));

            if (string.IsNullOrWhiteSpace(person.ThirdName))
                throw new ArgumentException("Third name is required.", nameof(person.ThirdName));

            if (string.IsNullOrWhiteSpace(person.Phone))
                throw new ArgumentException("Phone is required.", nameof(person.Phone));

            if (person.DateOfBirth == default || person.DateOfBirth > DateTime.Today)
                throw new ArgumentException("Date of birth is invalid.", nameof(person.DateOfBirth));

            if (person.CityID <= 0)
                throw new ArgumentException("A valid city must be selected.", nameof(person.CityID));

            if (!string.IsNullOrWhiteSpace(person.Email) && !person.Email.Contains('@'))
                throw new ArgumentException("Email format is invalid.", nameof(person.Email));
        }

        private static void ValidatePersonId(int personId)
        {
            if (personId <= 0)
                throw new ArgumentOutOfRangeException(nameof(personId), "Person ID must be greater than zero.");
        }

        private async Task EnsurePersonExistsAsync(int personId)
        {
            if (!await _personData.IsPersonExistAsync(personId))
                throw new KeyNotFoundException($"Person with ID {personId} does not exist.");
        }

        private async Task EnsureNationalIdIsUniqueAsync(string nationalId, int? currentPersonId = null)
        {
            PersonDTO? person = await _personData.GetPersonByNationalIDAsync(nationalId);

            if (person == null)
                return;

            if (currentPersonId.HasValue && person.PersonID == currentPersonId.Value)
                return;

            throw new InvalidOperationException($"National ID '{nationalId}' is already used.");
        }

        #endregion

        #region Public Methods

        public async Task<List<PersonDTO>> GetAllPeopleAsync()
        {
            return await _personData.GetAllPeopleAsync();
        }

        public async Task<PersonDTO?> GetPersonByIdAsync(int personId)
        {
            ValidatePersonId(personId);

            PersonDTO? personDTO = await _personData.GetPersonByIdAsync(personId);

            if (personDTO == null)
                throw new KeyNotFoundException($"Person with ID {personId} does not exist.");

            return personDTO;
        }

        public async Task<PersonDTO?> GetPersonByNationalIDAsync(string nationalId)
        {
            if (string.IsNullOrWhiteSpace(nationalId))
                throw new ArgumentException("National ID is required.", nameof(nationalId));

            nationalId = nationalId.Trim();

            PersonDTO? person = await _personData.GetPersonByNationalIDAsync(nationalId);

            if (person == null)
                throw new KeyNotFoundException("Person not found.");

            return person;
        }

        public async Task<int> AddPersonAsync(PersonDTO person)
        {
            ValidatePerson(person);

            person.NationalID = person.NationalID.Trim();

            await EnsureNationalIdIsUniqueAsync(person.NationalID);

            int newPersonId = await _personData.AddPersonAsync(person);

            if (newPersonId <= 0)
                throw new InvalidOperationException("Failed to add person."); ;

            return newPersonId;
        }

        public async Task<bool> UpdatePersonAsync(PersonDTO person)
        {
            ValidatePerson(person);

            ValidatePersonId(person.PersonID);

            await EnsurePersonExistsAsync(person.PersonID);

            await EnsureNationalIdIsUniqueAsync(person.NationalID, person.PersonID);

            bool isUpdated = await _personData.UpdatePersonAsync(person);

            if (!isUpdated)
                throw new InvalidOperationException("Failed to update person.");

            return isUpdated;
        }

        public async Task<bool> DeletePersonAsync(int personId)
        {
            ValidatePersonId(personId);

            await EnsurePersonExistsAsync(personId);

            bool isDeleted = await _personData.DeletePersonAsync(personId);

            if (!isDeleted)
                throw new InvalidOperationException("Failed to delete person.");

            return isDeleted;
        }

        public async Task<bool> IsPersonExistAsync(int personId)
        {
            ValidatePersonId(personId);

            return await _personData.IsPersonExistAsync(personId);
        }
        #endregion
    }
}