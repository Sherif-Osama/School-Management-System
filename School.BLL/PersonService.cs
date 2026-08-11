using School.BLL.Common;
using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.PersonDTOs.Requests;
using School.DTO.PersonDTOs.Responses;

namespace School.BLL
{
    public class PersonService : IPersonService
    {
        private readonly IPersonData _personData;

        public PersonService(IPersonData personData)
        {
            _personData = personData;
        }
        private static int MinNationalIdLength => 14;
        private static int MaxNationalIdLength => 20;

        #region Private Helpers
        private static void ValidateDateOfBirth(DateTime dateOfBirth)
        {
            if (dateOfBirth == default || dateOfBirth > DateTime.Today)
                throw new ArgumentException("Date of birth is invalid.", nameof(dateOfBirth));
        }

        private static void ValidateEmail(string? email)
        {
            if (!string.IsNullOrWhiteSpace(email) && !email.Contains('@'))
                throw new ArgumentException("Email format is invalid.", nameof(email));
        }
        private static void ValidatePerson(CreatePersonRequest person)
        {
            ArgumentNullException.ThrowIfNull(person);

            person.NationalID = ValidationHelper.ValidateString(person.NationalID, nameof(person.NationalID), MinNationalIdLength, MaxNationalIdLength);

            person.FirstName = ValidationHelper.ValidateString(person.FirstName, nameof(person.FirstName));

            person.SecondName = ValidationHelper.ValidateString(person.SecondName, nameof(person.SecondName));
            person.ThirdName = ValidationHelper.ValidateString(person.ThirdName, nameof(person.ThirdName));

            person.Phone = ValidationHelper.ValidateString(person.Phone, nameof(person.Phone));

            ValidationHelper.ValidateId(person.CityID);

            ValidateDateOfBirth(person.DateOfBirth);
            ValidateEmail(person.Email);
        }
        private static void ValidatePerson(UpdatePersonRequest person)
        {
            ArgumentNullException.ThrowIfNull(person);

            person.NationalID = ValidationHelper.ValidateString(person.NationalID, nameof(person.NationalID), MinNationalIdLength, MaxNationalIdLength);

            person.FirstName = ValidationHelper.ValidateString(person.FirstName, nameof(person.FirstName));

            person.SecondName = ValidationHelper.ValidateString(person.SecondName, nameof(person.SecondName));
            person.ThirdName = ValidationHelper.ValidateString(person.ThirdName, nameof(person.ThirdName));

            person.Phone = ValidationHelper.ValidateString(person.Phone, nameof(person.Phone));

            ValidationHelper.ValidateId(person.CityID);

            ValidateDateOfBirth(person.DateOfBirth);
            ValidateEmail(person.Email);
        }

        #endregion

        #region Public Methods

        public async Task<List<PersonResponse>> GetAllPeopleAsync()
        {
            return await _personData.GetAllPeopleAsync();
        }

        public async Task<PersonResponse?> GetPersonByIdAsync(int personId)
        {
            ValidationHelper.ValidateId(personId);

            PersonResponse? personDTO = await _personData.GetPersonByIdAsync(personId);

            if (personDTO == null)
                throw new KeyNotFoundException($"Person with ID {personId} does not exist.");

            return personDTO;
        }

        public async Task<PersonResponse?> GetPersonByNationalIDAsync(string nationalId)
        {
            nationalId = ValidationHelper.ValidateString(nationalId, nameof(nationalId), MinNationalIdLength, MaxNationalIdLength);

            PersonResponse? person = await _personData.GetPersonByNationalIDAsync(nationalId);

            if (person == null)
                throw new KeyNotFoundException("Person not found.");

            return person;
        }

        public async Task<int> AddPersonAsync(CreatePersonRequest person)
        {
            ValidatePerson(person);

            await EnsureHelper.EnsureUniqueAsync(_personData.GetPersonByNationalIDAsync, person.NationalID);

            int newPersonId = await _personData.AddPersonAsync(person);

            if (newPersonId <= 0)
                throw new InvalidOperationException("Failed to add person.");

            return newPersonId;
        }

        public async Task<bool> UpdatePersonAsync(int personId, UpdatePersonRequest person)
        {
            ValidatePerson(person);

            ValidationHelper.ValidateId(personId);

            await EnsureHelper.EnsureExistsAsync(_personData.IsPersonExistAsync, personId, "Person");

            await EnsureHelper.EnsureUniqueAsync(_personData.GetPersonByNationalIDAsync, person.NationalID, p => p.PersonID, personId);

            bool isUpdated = await _personData.UpdatePersonAsync(personId, person);

            if (!isUpdated)
                throw new InvalidOperationException("Failed to update person.");

            return isUpdated;
        }

        public async Task<bool> DeletePersonAsync(int personId)
        {
            ValidationHelper.ValidateId(personId);

            await EnsureHelper.EnsureExistsAsync(_personData.IsPersonExistAsync, personId, "Person");

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