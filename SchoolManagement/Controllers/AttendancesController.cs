using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.BLL.Interfaces;
using School.DTO.AttendanceDTOs;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendancesController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;
        public AttendancesController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        [HttpGet]
        [Authorize(Policy = "Attendance.View.All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<AttendanceDetailsDTO>>> GetAllAttendances()
        {
            return Ok(await _attendanceService.GetAllAttendancesAsync());
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "Attendance.View.All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AttendanceDetailsDTO>> GetAttendanceById(int id)
        {
            AttendanceDetailsDTO? attendance = await _attendanceService.GetAttendanceByIdAsync(id);

            if (attendance == null)
                return NotFound();

            return Ok(attendance);
        }

        [HttpGet("Student/{studentId:int}")]
        [Authorize(Policy = "Attendance.View.All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<AttendanceDetailsDTO>>> GetAttendancesByStudentId(int studentId)
        {
            return Ok(await _attendanceService.GetAttendancesByStudentIdAsync(studentId));
        }

        [HttpGet("Class/{classId:int}")]
        [Authorize(Policy = "Attendance.View.All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<AttendanceDetailsDTO>>> GetAttendancesByClassId(int classId)
        {
            return Ok(await _attendanceService.GetAttendancesByClassIdAsync(classId));
        }

        [HttpGet("Date/{attendanceDate:datetime}")]
        [Authorize(Policy = "Attendance.View.All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<AttendanceDetailsDTO>>> GetAttendancesByDate(DateOnly attendanceDate)
        {
            return Ok(await _attendanceService.GetAttendancesByDateAsync(attendanceDate));
        }

        [HttpGet("Status/{statusId:int}")]
        [Authorize(Policy = "Attendance.View.All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<AttendanceDetailsDTO>>> GetAttendancesByStatusId(int statusId)
        {
            return Ok(await _attendanceService.GetAttendancesByStatusIdAsync(statusId));
        }

        [HttpPost]
        [Authorize(Policy = "Attendance.Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<int>> AddAttendance(AttendanceDTO attendanceDTO)
        {
            int attendanceId = await _attendanceService.AddAttendanceAsync(attendanceDTO);

            return CreatedAtAction(
                nameof(GetAttendanceById),
                new { id = attendanceId },
                attendanceId);
        }

        [HttpPut]
        [Authorize(Policy = "Attendance.Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateAttendance(AttendanceDTO attendanceDTO)
        {
            await _attendanceService.UpdateAttendanceAsync(attendanceDTO);

            return Ok();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "Attendance.Delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteAttendance(int id)
        {
            await _attendanceService.DeleteAttendanceAsync(id);

            return NoContent();
        }
    }
}