using Microsoft.AspNetCore.Mvc;
using School.BLL;
using School.DTO.TeachersDTOs;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachersController : ControllerBase
    {
        private readonly TeacherService _teacherService;

        public TeachersController(TeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<TeacherDetailsDTO>>> GetAllTeachers()
        {
            return Ok(await _teacherService.GetAllTeachersAsync());
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TeacherDetailsDTO>> GetTeacherById(int id)
        {
            TeacherDetailsDTO? teacher = await _teacherService.GetTeacherByIdAsync(id);

            if (teacher == null)
                return NotFound();

            return Ok(teacher);
        }

        [HttpGet("Person/{personId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TeacherDetailsDTO>> GetTeacherByPersonId(int personId)
        {
            TeacherDetailsDTO? teacher = await _teacherService.GetTeacherByPersonIdAsync(personId);

            if (teacher == null)
                return NotFound();

            return Ok(teacher);
        }

        [HttpGet("NationalID/{nationalId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TeacherDetailsDTO>> GetTeacherByNationalId(string nationalId)
        {
            TeacherDetailsDTO? teacher = await _teacherService.GetTeacherByNationalIdAsync(nationalId);

            if (teacher == null)
                return NotFound();

            return Ok(teacher);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<int>> AddTeacher(TeacherDTO teacherDTO)
        {
            int teacherId = await _teacherService.AddTeacherAsync(teacherDTO);

            return CreatedAtAction(
                nameof(GetTeacherById),
                new { id = teacherId },
                teacherId);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateTeacher(TeacherDTO teacherDTO)
        {
            await _teacherService.UpdateTeacherAsync(teacherDTO);

            return Ok();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            await _teacherService.DeleteTeacherAsync(id);

            return NoContent();
        }
    }
}