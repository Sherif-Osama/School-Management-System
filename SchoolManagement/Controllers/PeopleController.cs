using Microsoft.AspNetCore.Mvc;
using School.BLL;
using School.DTO.PersonDTOs;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly PersonService _personService;

        public PeopleController(PersonService personService)
        {
            _personService = personService;
        }

        [HttpGet]
        public async Task<ActionResult<List<PersonDTO>>> GetAllPeople()
        {
            var people = await _personService.GetAllPeopleAsync();

            return Ok(people);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<PersonDTO>> GetPersonById(int id)
        {
            var person = await _personService.GetPersonByIdAsync(id);

            if (person == null)
                return NotFound();

            return Ok(person);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("NationalID/{nationalId}")]
        public async Task<ActionResult<PersonDTO>> GetPersonByNationalId(string nationalId)
        {
            var person = await _personService.GetPersonByNationalIDAsync(nationalId);

            if (person == null)
                return NotFound();

            return Ok(person);
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [HttpPost]
        public async Task<ActionResult<int>> AddPersonAsync(PersonDTO newPersonDTO)
        {
            int personId = await _personService.AddPersonAsync(newPersonDTO);

            return CreatedAtAction(nameof(GetPersonById), new { id = personId },
                personId);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPut("Update")]
        public async Task<ActionResult> UpdatePersonAsync(PersonDTO updatedPersonDTO)
        {
            await _personService.UpdatePersonAsync(updatedPersonDTO);

            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePersonAsync(int id)
        {
            await _personService.DeletePersonAsync(id);

            return NoContent();
        }
    }
}
