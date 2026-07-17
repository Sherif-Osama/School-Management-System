using Microsoft.AspNetCore.Mvc;
using School.BLL;
using School.DTO.AttendanceStatusDTOs;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceStatusesController : ControllerBase
    {
        private readonly AttendanceStatusService _attendanceStatusService;

        public AttendanceStatusesController(AttendanceStatusService attendanceStatusService)
        {
            _attendanceStatusService = attendanceStatusService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<AttendanceStatusDTO>>> GetAllAttendanceStatuses()
        {
            return Ok(await _attendanceStatusService.GetAllAttendanceStatusesAsync());
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AttendanceStatusDTO>> GetAttendanceStatusById(int id)
        {
            AttendanceStatusDTO? status = await _attendanceStatusService.GetAttendanceStatusByIdAsync(id);

            if (status == null)
                return NotFound();

            return Ok(status);
        }

        [HttpGet("Name/{statusName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AttendanceStatusDTO>> GetAttendanceStatusByName(string statusName)
        {
            AttendanceStatusDTO? status = await _attendanceStatusService.GetAttendanceStatusByNameAsync(statusName);

            if (status == null)
                return NotFound();

            return Ok(status);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<int>> AddAttendanceStatus(AttendanceStatusDTO statusDTO)
        {
            int statusId = await _attendanceStatusService.AddAttendanceStatusAsync(statusDTO);

            return CreatedAtAction(
                nameof(GetAttendanceStatusById),
                new { id = statusId },
                statusId);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateAttendanceStatus(AttendanceStatusDTO statusDTO)
        {
            await _attendanceStatusService.UpdateAttendanceStatusAsync(statusDTO);

            return Ok();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteAttendanceStatus(int id)
        {
            await _attendanceStatusService.DeleteAttendanceStatusAsync(id);

            return NoContent();
        }
    }
}