using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.BLL.Interfaces;
using School.DTO.PersonDTOs.Requests;
using School.DTO.PersonDTOs.Responses;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly IPersonService _personService;

        public PeopleController(IPersonService personService)
        {
            _personService = personService;
        }

        [HttpGet]
        [Authorize(Policy = "People.View")]
        public async Task<ActionResult<List<PersonResponse>>> GetAllPeople()
        {
            var people = await _personService.GetAllPeopleAsync();

            return Ok(people);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id:int}")]
        [Authorize(Policy = "People.View")]
        public async Task<ActionResult<PersonResponse>> GetPersonById(int id)
        {
            var person = await _personService.GetPersonByIdAsync(id);

            if (person == null)
                return NotFound();

            return Ok(person);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("NationalID/{nationalId}")]
        [Authorize(Policy = "People.View")]
        public async Task<ActionResult<PersonResponse>> GetPersonByNationalId(string nationalId)
        {
            var person = await _personService.GetPersonByNationalIDAsync(nationalId);

            if (person == null)
                return NotFound();

            return Ok(person);
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [HttpPost]
        [Authorize(Policy = "People.Create")]
        public async Task<ActionResult<int>> AddPersonAsync(CreatePersonRequest Person)
        {
            int personId = await _personService.AddPersonAsync(Person);

            return CreatedAtAction(nameof(GetPersonById), new { id = personId },
                personId);
        }

        [HttpPut("{personId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Policy = "People.Update")]
        public async Task<ActionResult> UpdatePersonAsync(int personId, UpdatePersonRequest Person)
        {
            await _personService.UpdatePersonAsync(personId, Person);

            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpDelete("{id:int}")]
        [Authorize(Policy = "People.Delete")]
        public async Task<IActionResult> DeletePersonAsync(int id)
        {
            await _personService.DeletePersonAsync(id);

            return NoContent();
        }
    }
}