using Microsoft.AspNetCore.Mvc;
using School.BLL.Interfaces;
using School.DTO.CityDTOs;

namespace SchoolManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitiesController : ControllerBase
    {
        private readonly ICityService _cityService;

        public CitiesController(ICityService cityService)
        {
            _cityService = cityService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<CityDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<CityDTO>>> GetAllCities()
        {
            List<CityDTO> cities = await _cityService.GetAllCitiesAsync();
            return Ok(cities);
        }

        [HttpGet("{cityId:int}")]
        [ProducesResponseType(typeof(CityDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CityDTO>> GetCityById(int cityId)
        {
            CityDTO? city = await _cityService.GetCityByIdAsync(cityId);
            return Ok(city);
        }

        [HttpGet("by-name/{cityName}")]
        [ProducesResponseType(typeof(CityDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CityDTO>> GetCityByName(string cityName)
        {
            CityDTO? city = await _cityService.GetCityByNameAsync(cityName);
            return Ok(city);
        }
    }
}