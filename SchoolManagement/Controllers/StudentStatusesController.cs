using Microsoft.AspNetCore.Mvc;
using School.BLL;
using School.DTO.StudentStatusDTOs;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentStatusesController : ControllerBase
    {
        private readonly StudentStatusService _studentStatusService;

        public StudentStatusesController(StudentStatusService studentStatusService)
        {
            _studentStatusService = studentStatusService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<StudentStatusDTO>>> GetAllStudentStatuses()
        {
            return Ok(await _studentStatusService.GetAllStudentStatusesAsync());
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StudentStatusDTO>> GetStudentStatusById(int id)
        {
            StudentStatusDTO? status = await _studentStatusService.GetStudentStatusByIdAsync(id);

            if (status == null)
                return NotFound();

            return Ok(status);
        }

        [HttpGet("Name/{statusName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StudentStatusDTO>> GetStudentStatusByName(string statusName)
        {
            StudentStatusDTO? status = await _studentStatusService.GetStudentStatusByNameAsync(statusName);

            if (status == null)
                return NotFound();

            return Ok(status);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<int>> AddStudentStatus(StudentStatusDTO statusDTO)
        {
            int statusId = await _studentStatusService.AddStudentStatusAsync(statusDTO);

            return CreatedAtAction(
                nameof(GetStudentStatusById),
                new { id = statusId },
                statusId);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateStudentStatus(StudentStatusDTO statusDTO)
        {
            await _studentStatusService.UpdateStudentStatusAsync(statusDTO);

            return Ok();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteStudentStatus(int id)
        {
            await _studentStatusService.DeleteStudentStatusAsync(id);

            return NoContent();
        }
    }
}