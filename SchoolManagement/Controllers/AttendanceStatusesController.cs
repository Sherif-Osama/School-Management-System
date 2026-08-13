using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.BLL.Interfaces;
using School.DTO.AttendanceStatusDTOs.Requests;
using School.DTO.AttendanceStatusDTOs.Responses;

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
        [Authorize(Policy = "Attendance.View.All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<AttendanceStatusResponse>>> GetAllAttendanceStatuses()
        {
            return Ok(await _attendanceStatusService.GetAllAttendanceStatusesAsync());
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "Attendance.View.All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AttendanceStatusResponse>> GetAttendanceStatusById(int id)
        {
            AttendanceStatusResponse status = await _attendanceStatusService.GetAttendanceStatusByIdAsync(id);

            return Ok(status);
        }

        [HttpGet("Name/{statusName}")]
        [Authorize(Policy = "Attendance.View.All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AttendanceStatusResponse>> GetAttendanceStatusByName(string statusName)
        {
            AttendanceStatusResponse status = await _attendanceStatusService.GetAttendanceStatusByNameAsync(statusName);
            return Ok(status);
        }

        [HttpPost]
        [Authorize(Policy = "Attendance.Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<int>> AddAttendanceStatus(AttendanceStatusRequest status)
        {
            int statusId = await _attendanceStatusService.AddAttendanceStatusAsync(status);

            return CreatedAtAction(
                nameof(GetAttendanceStatusById),
                new { id = statusId },
                statusId);
        }

        [HttpPut("{statusId:int}")]
        [Authorize(Policy = "Attendance.Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateAttendanceStatus(int statusId, AttendanceStatusRequest status)
        {
            await _attendanceStatusService.UpdateAttendanceStatusAsync(statusId, status);

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