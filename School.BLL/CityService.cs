using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.CityDTOs;

namespace School.BLL
{
    public class CityService : ICityService
    {
        private readonly ICityData _cityData;

        public CityService(ICityData cityData)
        {
            _cityData = cityData;
        }

        #region Validation

        private static void ValidateCityId(int cityId)
        {
            if (cityId <= 0)
                throw new ArgumentException("City ID must be greater than zero.", nameof(cityId));
        }

        private static string ValidateCityName(string cityName)
        {
            if (string.IsNullOrWhiteSpace(cityName))
                throw new ArgumentException("City name is required.", nameof(cityName));

            cityName = cityName.Trim();

            if (cityName.Length > 100)
                throw new ArgumentException("City name cannot exceed 100 characters.", nameof(cityName));

            return cityName;
        }

        #endregion

        #region Public

        public Task<List<CityDTO>> GetAllCitiesAsync()
        {
            return _cityData.GetAllCitiesAsync();
        }

        public async Task<CityDTO?> GetCityByIdAsync(int cityId)
        {
            ValidateCityId(cityId);

            CityDTO? city = await _cityData.GetCityByIdAsync(cityId);

            if (city == null)
                throw new KeyNotFoundException($"City with ID {cityId} does not exist.");

            return city;
        }

        public async Task<CityDTO?> GetCityByNameAsync(string cityName)
        {
            cityName = ValidateCityName(cityName);

            CityDTO? city = await _cityData.GetCityByNameAsync(cityName);

            if (city == null)
                throw new KeyNotFoundException($"City with name '{cityName}' does not exist.");

            return city;
        }

        #endregion
    }
}