using Microsoft.AspNetCore.Mvc;
using School.BLL.Interfaces;
using School.DTO.CountriesDTOs;

namespace SchoolManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountriesController : ControllerBase
    {
        private readonly ICountryService _countryService;

        public CountriesController(ICountryService countryService)
        {
            _countryService = countryService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<CountryDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<CountryDTO>>> GetAllCountries()
        {
            List<CountryDTO> countries = await _countryService.GetAllCountriesAsync();
            return Ok(countries);
        }

        [HttpGet("{countryId:int}")]
        [ProducesResponseType(typeof(CountryDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CountryDTO>> GetCountryById(int countryId)
        {
            CountryDTO? country = await _countryService.GetCountryByIdAsync(countryId);
            return Ok(country);
        }

        [HttpGet("by-name/{countryName}")]
        [ProducesResponseType(typeof(CountryDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CountryDTO>> GetCountryByName(string countryName)
        {
            CountryDTO? country = await _countryService.GetCountryByNameAsync(countryName);
            return Ok(country);
        }
    }
}