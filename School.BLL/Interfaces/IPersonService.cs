using School.DTO.PersonDTOs;

namespace School.BLL.Interfaces
{
    public interface IPersonService
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