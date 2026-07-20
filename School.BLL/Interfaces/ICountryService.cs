using School.DTO.CountriesDTOs;

namespace School.BLL.Interfaces
{
    public interface ICountryService
    {
        Task<List<CountryDTO>> GetAllCountriesAsync();
        Task<CountryDTO?> GetCountryByIdAsync(int countryId);
        Task<CountryDTO?> GetCountryByNameAsync(string countryName);
    }
}