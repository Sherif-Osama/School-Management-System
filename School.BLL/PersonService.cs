using School.BLL.Common;
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
        private static int minNationalIdLength => 14;
        private static int maxNationalIdLength => 20;

        #region Private Helpers

        private static void ValidatePerson(PersonDTO person)
        {
            ArgumentNullException.ThrowIfNull(person);

            person.NationalID = ValidationHelper.ValidateString(person.NationalID, nameof(person.NationalID), minNationalIdLength, maxNationalIdLength);

            person.FirstName = ValidationHelper.ValidateString(person.FirstName, nameof(person.FirstName));

            person.SecondName = ValidationHelper.ValidateString(person.SecondName, nameof(person.SecondName));
            person.ThirdName = ValidationHelper.ValidateString(person.ThirdName, nameof(person.ThirdName));

            person.Phone = ValidationHelper.ValidateString(person.Phone, nameof(person.Phone));

            ValidationHelper.ValidateId(person.CityID);

            if (person.DateOfBirth == default || person.DateOfBirth > DateTime.Today)
                throw new ArgumentException("Date of birth is invalid.", nameof(person.DateOfBirth));

            if (!string.IsNullOrWhiteSpace(person.Email) && !person.Email.Contains('@'))
                throw new ArgumentException("Email format is invalid.", nameof(person.Email));
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
            ValidationHelper.ValidateId(personId);

            PersonDTO? personDTO = await _personData.GetPersonByIdAsync(personId);

            if (personDTO == null)
                throw new KeyNotFoundException($"Person with ID {personId} does not exist.");

            return personDTO;
        }

        public async Task<PersonDTO?> GetPersonByNationalIDAsync(string nationalId)
        {
            nationalId = ValidationHelper.ValidateString(nationalId, nameof(nationalId), minNationalIdLength, maxNationalIdLength);

            PersonDTO? person = await _personData.GetPersonByNationalIDAsync(nationalId);

            if (person == null)
                throw new KeyNotFoundException("Person not found.");

            return person;
        }

        public async Task<int> AddPersonAsync(PersonDTO person)
        {
            ValidatePerson(person);

            await EnsureNationalIdIsUniqueAsync(person.NationalID);

            int newPersonId = await _personData.AddPersonAsync(person);

            if (newPersonId <= 0)
                throw new InvalidOperationException("Failed to add person.");

            return newPersonId;
        }

        public async Task<bool> UpdatePersonAsync(PersonDTO person)
        {
            ValidatePerson(person);

            ValidationHelper.ValidateId(person.PersonID);

            await EnsurePersonExistsAsync(person.PersonID);

            await EnsureNationalIdIsUniqueAsync(person.NationalID, person.PersonID);

            bool isUpdated = await _personData.UpdatePersonAsync(person);

            if (!isUpdated)
                throw new InvalidOperationException("Failed to update person.");

            return isUpdated;
        }

        public async Task<bool> DeletePersonAsync(int personId)
        {
            ValidationHelper.ValidateId(personId);

            await EnsurePersonExistsAsync(personId);

            bool isDeleted = await _personData.DeletePersonAsync(personId);

            if (!isDeleted)
                throw new InvalidOperationException("Failed to delete person.");

            return isDeleted;
        }

        public async Task<bool> IsPersonExistAsync(int personId)
        {
            ValidationHelper.ValidateId(personId);

            return await _personData.IsPersonExistAsync(personId);
        }
        #endregion
    }
}