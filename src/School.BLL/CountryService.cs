using School.BLL.Common;
using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.CountriesDTOs;

namespace School.BLL
{
    public class CountryService : ICountryService
    {
        private readonly ICountryData _countryData;
        private static int MinCountryNameLength => 2;
        private static int MaxCountryNameLength => 100;
        public CountryService(ICountryData countryData)
        {
            _countryData = countryData;
        }
        #region Public

        public Task<List<CountryDTO>> GetAllCountriesAsync()
        {
            return _countryData.GetAllCountriesAsync();
        }

        public async Task<CountryDTO> GetCountryByIdAsync(int countryId)
        {
            ValidationHelper.ValidateId(countryId);

            CountryDTO? country = await _countryData.GetCountryByIdAsync(countryId);

            if (country == null)
                throw new KeyNotFoundException($"Country with ID {countryId} does not exist.");

            return country;
        }

        public async Task<CountryDTO> GetCountryByNameAsync(string countryName)
        {
            countryName = ValidationHelper.ValidateString(countryName, nameof(countryName), MinCountryNameLength, MaxCountryNameLength);

            CountryDTO? country = await _countryData.GetCountryByNameAsync(countryName);

            if (country == null)
                throw new KeyNotFoundException($"Country with name '{countryName}' does not exist.");

            return country;
        }

        #endregion
    }
}