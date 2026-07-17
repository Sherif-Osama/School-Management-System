using School.DTO.PersonDTOs;

namespace School.DAL.Interfaces
{
    public interface IPersonData
    {
        Task<int> AddPersonAsync(PersonDTO person);
        Task<bool> DeletePersonAsync(int personId);
        Task<List<PersonDTO>> GetAllPeopleAsync();
        Task<PersonDTO?> GetPersonByIdAsync(int personId);
        Task<PersonDTO?> GetPersonByNationalIDAsync(string nationalId);
        Task<bool> IsPersonExistAsync(int personId);
        Task<bool> UpdatePersonAsync(PersonDTO person);
    }
}