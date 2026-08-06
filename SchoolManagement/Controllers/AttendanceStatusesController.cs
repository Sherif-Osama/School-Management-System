using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.BLL.Interfaces;
using School.DTO.AttendanceStatusDTOs;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceStatusesController : ControllerBase
    {
        private readonly IAttendanceStatusService _attendanceStatusService;

        public AttendanceStatusesController(IAttendanceStatusService attendanceStatusService)
        {
            _attendanceStatusService = attendanceStatusService;
        }

        [HttpGet]
        [Authorize(Policy = "Attendance.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<AttendanceStatusDTO>>> GetAllAttendanceStatuses()
        {
            return Ok(await _attendanceStatusService.GetAllAttendanceStatusesAsync());
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "Attendance.View")]
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
        [Authorize(Policy = "Attendance.View")]
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
        [Authorize(Policy = "Attendance.Create")]
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
        [Authorize(Policy = "Attendance.Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateAttendanceStatus(AttendanceStatusDTO statusDTO)
        {
            await _attendanceStatusService.UpdateAttendanceStatusAsync(statusDTO);

            return Ok();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "Attendance.Delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteAttendanceStatus(int id)
        {
            await _attendanceStatusService.DeleteAttendanceStatusAsync(id);

            return NoContent();
        }
    }
}