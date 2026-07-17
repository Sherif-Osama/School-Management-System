using Microsoft.AspNetCore.Mvc;
using School.BLL.Interfaces;
using School.DTO.GradesDTOs;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GradesController : ControllerBase
    {
        private readonly IGradeService _gradeService;

        public GradesController(IGradeService gradeService)
        {
            _gradeService = gradeService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<GradeDTO>>> GetAllGrades()
        {
            return Ok(await _gradeService.GetAllGradesAsync());
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GradeDTO>> GetGradeById(byte id)
        {
            GradeDTO? grade = await _gradeService.GetGradeByIdAsync(id);

            if (grade == null)
                return NotFound();

            return Ok(grade);
        }

        [HttpGet("Name/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GradeDTO>> GetGradeByName(string name)
        {
            GradeDTO? grade = await _gradeService.GetGradeByNameAsync(name);

            if (grade == null)
                return NotFound();

            return Ok(grade);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<int>> AddGrade(GradeDTO gradeDTO)
        {
            int gradeId = await _gradeService.AddGradeAsync(gradeDTO);

            return CreatedAtAction(
                nameof(GetGradeById),
                new { id = gradeId },
                gradeId);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateGrade(GradeDTO gradeDTO)
        {
            await _gradeService.UpdateGradeAsync(gradeDTO);

            return Ok();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteGrade(byte id)
        {
            await _gradeService.DeleteGradeAsync(id);

            return NoContent();
        }
    }
}