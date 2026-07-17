using Microsoft.AspNetCore.Mvc;
using School.BLL.Interfaces;
using School.DTO.StudentsDTOs;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<StudentDetailsDTO>>> GetAllStudents()
        {
            return Ok(await _studentService.GetAllStudentsAsync());
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StudentDetailsDTO>> GetStudentById(int id)
        {
            StudentDetailsDTO? student = await _studentService.GetStudentByIdAsync(id);

            if (student == null)
                return NotFound();

            return Ok(student);
        }

        [HttpGet("Person/{personId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StudentDetailsDTO>> GetStudentByPersonId(int personId)
        {
            StudentDetailsDTO? student = await _studentService.GetStudentByPersonIdAsync(personId);

            if (student == null)
                return NotFound();

            return Ok(student);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<int>> AddStudent(StudentDTO studentDTO)
        {
            int studentId = await _studentService.AddStudentAsync(studentDTO);

            return CreatedAtAction(
                nameof(GetStudentById),
                new { id = studentId },
                studentId);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateStudent(StudentDTO studentDTO)
        {
            await _studentService.UpdateStudentAsync(studentDTO);

            return Ok();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            await _studentService.DeleteStudentAsync(id);

            return NoContent();
        }
    }
}