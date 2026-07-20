using School.DTO.CityDTOs;

namespace School.BLL.Interfaces
{
    public interface ICityService
    {
        Task<List<CityDTO>> GetAllCitiesAsync();
        Task<CityDTO?> GetCityByIdAsync(int cityId);
        Task<CityDTO?> GetCityByNameAsync(string cityName);
    }
}