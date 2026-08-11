using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.BLL.Interfaces;
using School.DTO.ClassesDTOs.Requests;
using School.DTO.ClassesDTOs.Responses;

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
        [Authorize(Policy = "Classes.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ClassResponse>>> GetAllClasses()
        {
            return Ok(await _classService.GetAllClassesAsync());
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "Classes.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClassResponse>> GetClassById(int id)
        {
            ClassResponse? schoolClass =
                await _classService.GetClassByIdAsync(id);

            if (schoolClass == null)
                return NotFound();

            return Ok(schoolClass);
        }

        [HttpGet("Search")]
        [Authorize(Policy = "Classes.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClassResponse>> GetClassByDetails(byte gradeId, string className, string academicYear)
        {
            ClassResponse? schoolClass =
                await _classService.GetClassByDetailsAsync(
                    gradeId,
                    className,
                    academicYear);

            if (schoolClass == null)
                return NotFound();

            return Ok(schoolClass);
        }

        [HttpPost]
        [Authorize(Policy = "Classes.Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<int>> AddClass(CreateClassRequest classDTO)
        {
            int classId =
                await _classService.AddClassAsync(classDTO);

            return CreatedAtAction(
                nameof(GetClassById),
                new { id = classId },
                classId);
        }

        [HttpPut("{classId:int}")]
        [Authorize(Policy = "Classes.Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateClass(int classId, UpdateClassRequest classDTO)
        {
            await _classService.UpdateClassAsync(classId, classDTO);

            return Ok();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "Classes.Delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteClass(int id)
        {
            await _classService.DeleteClassAsync(id);

            return NoContent();
        }
    }
}