using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.BLL.Interfaces;
using School.DTO.SubjectDTOs;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectsController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [HttpGet]
        [Authorize(Policy = "Subjects.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<SubjectDTO>>> GetAllSubjects()
        {
            return Ok(await _subjectService.GetAllSubjectsAsync());
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "Subjects.View")]
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
        [Authorize(Policy = "Subjects.View")]
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
        [Authorize(Policy = "Subjects.Create")]
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
        [Authorize(Policy = "Subjects.Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateSubject(SubjectDTO subjectDTO)
        {
            await _subjectService.UpdateSubjectAsync(subjectDTO);

            return Ok();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "Subjects.Delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            await _subjectService.DeleteSubjectAsync(id);

            return NoContent();
        }
    }
}