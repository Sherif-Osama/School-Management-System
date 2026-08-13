using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.API.Authorization.OwnedResources;
using School.API.Authorization.Requirements;
using School.BLL.Authentication;
using School.BLL.Interfaces;
using School.DTO.AttendanceDTOs.Requests;
using School.DTO.AttendanceDTOs.Responses;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendancesController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;
        private readonly IAuthorizationService _authorizationService;

        public AttendancesController(IAttendanceService attendanceService, IAuthorizationService authorizationService)
        {
            _attendanceService = attendanceService;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        [Authorize(Policy = "Attendance.View.All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<AttendanceResponse>>> GetAllAttendances()
        {
            return Ok(await _attendanceService.GetAllAttendancesAsync());
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "Attendance.View.Own")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<AttendanceResponse>> GetAttendanceById(int id)
        {
            AttendanceResponse attendance = await _attendanceService.GetAttendanceByIdAsync(id);

            if (!User.HasClaim(CustomClaimTypes.Permission, "Attendance.View.All"))
            {
                var authResult = await _authorizationService.AuthorizeAsync(
                    User, new StudentOwnedResource(attendance.StudentID), new OwnershipRequirement());

                if (!authResult.Succeeded)
                    return Forbid();
            }

            return Ok(attendance);
        }

        [HttpGet("Student/{studentId:int}")]
        [Authorize(Policy = "Attendance.View.Own")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<List<AttendanceResponse>>> GetAttendancesByStudentId(int studentId)
        {
            if (!User.HasClaim(CustomClaimTypes.Permission, "Attendance.View.All"))
            {
                var authResult = await _authorizationService.AuthorizeAsync(
                    User, new StudentOwnedResource(studentId), new OwnershipRequirement());

                if (!authResult.Succeeded)
                    return Forbid();
            }

            return Ok(await _attendanceService.GetAttendancesByStudentIdAsync(studentId));
        }

        [HttpGet("Class/{classId:int}")]
        [Authorize(Policy = "Attendance.View.All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<AttendanceResponse>>> GetAttendancesByClassId(int classId)
        {
            return Ok(await _attendanceService.GetAttendancesByClassIdAsync(classId));
        }

        [HttpGet("Date/{attendanceDate}")]
        [Authorize(Policy = "Attendance.View.All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<AttendanceResponse>>> GetAttendancesByDate(DateOnly attendanceDate)
        {
            return Ok(await _attendanceService.GetAttendancesByDateAsync(attendanceDate));
        }

        [HttpGet("Status/{statusId:int}")]
        [Authorize(Policy = "Attendance.View.All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<AttendanceResponse>>> GetAttendancesByStatusId(int statusId)
        {
            return Ok(await _attendanceService.GetAttendancesByStatusIdAsync(statusId));
        }

        [HttpPost]
        [Authorize(Policy = "Attendance.Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<int>> AddAttendance(CreateAttendanceRequest attendance)
        {
            int attendanceId = await _attendanceService.AddAttendanceAsync(attendance);

            return CreatedAtAction(
                nameof(GetAttendanceById),
                new { id = attendanceId },
                attendanceId);
        }

        [HttpPut("student/{studentId:int}/attendance/{attendanceID:int}")]
        [Authorize(Policy = "Attendance.Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateAttendance(int studentId, int attendanceID, UpdateAttendanceRequest attendance)
        {
            await _attendanceService.UpdateAttendanceAsync(studentId, attendanceID, attendance);

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