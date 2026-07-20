using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.CountriesDTOs;

namespace School.BLL
{
    public class CountryService : ICountryService
    {
        private readonly ICountryData _countryData;

        public CountryService(ICountryData countryData)
        {
            _countryData = countryData;
        }

        #region Validation

        private static void ValidateCountryId(int countryId)
        {
            if (countryId <= 0)
                throw new ArgumentException("Country ID must be greater than zero.", nameof(countryId));
        }

        private static string ValidateCountryName(string countryName)
        {
            if (string.IsNullOrWhiteSpace(countryName))
                throw new ArgumentException("Country name is required.", nameof(countryName));

            countryName = countryName.Trim();

            if (countryName.Length > 100)
                throw new ArgumentException("Country name cannot exceed 100 characters.", nameof(countryName));

            return countryName;
        }

        #endregion

        #region Public

        public Task<List<CountryDTO>> GetAllCountriesAsync()
        {
            return _countryData.GetAllCountriesAsync();
        }

        public async Task<CountryDTO?> GetCountryByIdAsync(int countryId)
        {
            ValidateCountryId(countryId);

            CountryDTO? country = await _countryData.GetCountryByIdAsync(countryId);

            if (country == null)
                throw new KeyNotFoundException($"Country with ID {countryId} does not exist.");

            return country;
        }

        public async Task<CountryDTO?> GetCountryByNameAsync(string countryName)
        {
            countryName = ValidateCountryName(countryName);

            CountryDTO? country = await _countryData.GetCountryByNameAsync(countryName);

            if (country == null)
                throw new KeyNotFoundException($"Country with name '{countryName}' does not exist.");

            return country;
        }

        #endregion
    }
}