using School.DTO.PersonDTOs.Requests;
using School.DTO.PersonDTOs.Responses;

namespace School.BLL.Interfaces
{
    public interface IPersonService
    {
        Task<int> AddPersonAsync(CreatePersonRequest person);
        Task<bool> DeletePersonAsync(int personId);
        Task<List<PersonResponse>> GetAllPeopleAsync();
        Task<PersonResponse?> GetPersonByIdAsync(int personId);
        Task<PersonResponse?> GetPersonByNationalIDAsync(string nationalId);
        Task<bool> IsPersonExistAsync(int personId);
        Task<bool> UpdatePersonAsync(int personId, UpdatePersonRequest person);
    }
}