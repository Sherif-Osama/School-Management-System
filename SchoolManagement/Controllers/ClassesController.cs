using Microsoft.AspNetCore.Mvc;
using School.BLL.Interfaces;
using School.DTO.ClassesDTOs;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassesController : ControllerBase
    {
        private readonly IClassService _classService;

        public ClassesController(IClassService classService)
        {
            _classService = classService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ClassDetailsDTO>>> GetAllClasses()
        {
            return Ok(await _classService.GetAllClassesAsync());
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClassDetailsDTO>> GetClassById(int id)
        {
            ClassDetailsDTO? schoolClass =
                await _classService.GetClassByIdAsync(id);

            if (schoolClass == null)
                return NotFound();

            return Ok(schoolClass);
        }

        [HttpGet("Search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClassDetailsDTO>> GetClassByDetails(byte gradeId, string className, string academicYear)
        {
            ClassDetailsDTO? schoolClass =
                await _classService.GetClassByDetailsAsync(
                    gradeId,
                    className,
                    academicYear);

            if (schoolClass == null)
                return NotFound();

            return Ok(schoolClass);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<int>> AddClass(ClassDTO classDTO)
        {
            int classId =
                await _classService.AddClassAsync(classDTO);

            return CreatedAtAction(
                nameof(GetClassById),
                new { id = classId },
                classId);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateClass(ClassDTO classDTO)
        {
            await _classService.UpdateClassAsync(classDTO);

            return Ok();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteClass(int id)
        {
            await _classService.DeleteClassAsync(id);

            return NoContent();
        }
    }
}