using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.BLL.Interfaces;
using School.DTO.StudentStatusDTOs.Requests;
using School.DTO.StudentStatusDTOs.Responses;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentStatusesController : ControllerBase
    {
        private readonly IStudentStatusService _studentStatusService;

        public StudentStatusesController(IStudentStatusService studentStatusService)
        {
            _studentStatusService = studentStatusService;
        }

        [HttpGet]
        [Authorize(Policy = "Students.View.All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<StudentStatusResponse>>> GetAllStudentStatuses()
        {
            return Ok(await _studentStatusService.GetAllStudentStatusesAsync());
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "Students.View.All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StudentStatusResponse>> GetStudentStatusById(int id)
        {
            StudentStatusResponse status = await _studentStatusService.GetStudentStatusByIdAsync(id);

            return Ok(status);
        }

        [HttpGet("Name/{statusName}")]
        [Authorize(Policy = "Students.View.All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StudentStatusResponse>> GetStudentStatusByName(string statusName)
        {
            StudentStatusResponse status = await _studentStatusService.GetStudentStatusByNameAsync(statusName);

            return Ok(status);
        }

        [HttpPost]
        [Authorize(Policy = "Students.Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<int>> AddStudentStatus(CreateStudentStatusRequest status)
        {
            int statusId = await _studentStatusService.AddStudentStatusAsync(status);

            return CreatedAtAction(
                nameof(GetStudentStatusById),
                new { id = statusId },
                statusId);
        }

        [HttpPut("{statusId:int}")]
        [Authorize(Policy = "Students.Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateStudentStatus(int statusId, UpdateStudentStatusRequest status)
        {
            await _studentStatusService.UpdateStudentStatusAsync(statusId, status);

            return Ok();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "Students.Delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteStudentStatus(int id)
        {
            await _studentStatusService.DeleteStudentStatusAsync(id);

            return NoContent();
        }
    }
}