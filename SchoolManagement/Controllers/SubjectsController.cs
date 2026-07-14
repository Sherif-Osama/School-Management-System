using Microsoft.AspNetCore.Mvc;
using School.BLL;
using School.DTO.SubjectDTO;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectsController : ControllerBase
    {
        private readonly SubjectService _subjectService;

        public SubjectsController(SubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<SubjectDTO>>> GetAllSubjects()
        {
            return Ok(await _subjectService.GetAllSubjectsAsync());
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SubjectDTO>> GetSubjectById(int id)
        {
            SubjectDTO? subject = await _subjectService.GetSubjectByIdAsync(id);

            if (subject == null)
                return NotFound();

            return Ok(subject);
        }

        [HttpGet("Name/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SubjectDTO>> GetSubjectByName(string name)
        {
            SubjectDTO? subject = await _subjectService.GetSubjectByNameAsync(name);

            if (subject == null)
                return NotFound();

            return Ok(subject);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<int>> AddSubject(SubjectDTO subjectDTO)
        {
            int subjectId = await _subjectService.AddSubjectAsync(subjectDTO);

            return CreatedAtAction(
                nameof(GetSubjectById),
                new { id = subjectId },
                subjectId);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateSubject(SubjectDTO subjectDTO)
        {
            await _subjectService.UpdateSubjectAsync(subjectDTO);

            return Ok();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            await _subjectService.DeleteSubjectAsync(id);

            return NoContent();
        }
    }
}