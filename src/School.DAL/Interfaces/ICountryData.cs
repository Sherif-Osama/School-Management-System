using School.DTO.CountriesDTOs;

namespace School.DAL.Interfaces
{
    public interface ICountryData
    {
        Task<List<CountryDTO>> GetAllCountriesAsync();
        Task<CountryDTO?> GetCountryByIdAsync(int countryId);
        Task<CountryDTO?> GetCountryByNameAsync(string countryName);
    }
}