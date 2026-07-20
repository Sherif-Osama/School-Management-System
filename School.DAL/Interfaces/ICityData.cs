using School.DTO.CityDTOs;

namespace School.DAL.Interfaces
{
    public interface ICityData
    {
        Task<List<CityDTO>> GetAllCitiesAsync();
        Task<CityDTO?> GetCityByIdAsync(int cityId);
        Task<CityDTO?> GetCityByNameAsync(string cityName);
    }
}