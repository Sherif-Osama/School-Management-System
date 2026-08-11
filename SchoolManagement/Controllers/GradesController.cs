using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.BLL.Interfaces;
using School.DTO.GradesDTOs.Requests;
using School.DTO.GradesDTOs.Responses;

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
        [Authorize(Policy = "Grades.View.All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<GradeResponse>>> GetAllGrades()
        {
            return Ok(await _gradeService.GetAllGradesAsync());
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "Grades.View.All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GradeResponse>> GetGradeById(byte id)
        {
            GradeResponse? grade = await _gradeService.GetGradeByIdAsync(id);

            if (grade == null)
                return NotFound();

            return Ok(grade);
        }

        [HttpGet("Name/{name}")]
        [Authorize(Policy = "Grades.View.All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GradeResponse>> GetGradeByName(string name)
        {
            GradeResponse? grade = await _gradeService.GetGradeByNameAsync(name);

            if (grade == null)
                return NotFound();

            return Ok(grade);
        }

        [HttpPost]
        [Authorize(Policy = "Grades.Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<int>> AddGrade(CreateGradeRequest grade)
        {
            int gradeId = await _gradeService.AddGradeAsync(grade);

            return CreatedAtAction(
                nameof(GetGradeById),
                new { id = gradeId },
                gradeId);
        }

        [HttpPut("{gradeId:int}")]
        [Authorize(Policy = "Grades.Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateGrade(byte gradeId, UpdateGradeRequest grade)
        {
            await _gradeService.UpdateGradeAsync(gradeId, grade);

            return Ok();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "Grades.Delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteGrade(byte id)
        {
            await _gradeService.DeleteGradeAsync(id);

            return NoContent();
        }
    }
}