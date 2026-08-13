using School.BLL.Common;
using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.CityDTOs;

namespace School.BLL
{
    public class CityService : ICityService
    {
        private readonly ICityData _cityData;
        private static int minCityNameLength => 2;
        private static int maxCityNameLength => 50;
        public CityService(ICityData cityData)
        {
            _cityData = cityData;
        }

        #region Public
        public Task<List<CityDTO>> GetAllCitiesAsync()
        {
            return _cityData.GetAllCitiesAsync();
        }

        public async Task<CityDTO> GetCityByIdAsync(int cityId)
        {
            ValidationHelper.ValidateId(cityId);

            CityDTO? city = await _cityData.GetCityByIdAsync(cityId);

            if (city == null)
                throw new KeyNotFoundException($"City with ID {cityId} does not exist.");

            return city;
        }

        public async Task<CityDTO> GetCityByNameAsync(string cityName)
        {
            cityName = ValidationHelper.ValidateString(cityName, nameof(cityName), minCityNameLength, maxCityNameLength);

            CityDTO? city = await _cityData.GetCityByNameAsync(cityName);

            if (city == null)
                throw new KeyNotFoundException($"City with name '{cityName}' does not exist.");

            return city;
        }

        #endregion
    }
}