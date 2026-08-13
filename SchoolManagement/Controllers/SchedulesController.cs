using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.BLL.Interfaces;
using School.DTO.ScheduleDTOs.Requests;
using School.DTO.ScheduleDTOs.Responses;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchedulesController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;

        public SchedulesController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [HttpGet]
        [Authorize(Policy = "Schedules.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ScheduleResponse>>> GetAllSchedules()
        {
            return Ok(await _scheduleService.GetAllSchedulesAsync());
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "Schedules.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ScheduleResponse>> GetScheduleById(int id)
        {
            ScheduleResponse schedule =
                await _scheduleService.GetScheduleByIdAsync(id);

            return Ok(schedule);
        }

        [HttpGet("Class/{classId:int}")]
        [Authorize(Policy = "Schedules.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ScheduleResponse>>> GetSchedulesByClassId(int classId)
        {
            return Ok(await _scheduleService.GetSchedulesByClassIdAsync(classId));
        }

        [HttpGet("Teacher/{teacherId:int}")]
        [Authorize(Policy = "Schedules.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ScheduleResponse>>> GetSchedulesByTeacherId(int teacherId)
        {
            return Ok(await _scheduleService.GetSchedulesByTeacherIdAsync(teacherId));
        }

        [HttpGet("Classroom/{classroomId:int}")]
        [Authorize(Policy = "Schedules.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ScheduleResponse>>> GetSchedulesByClassroomId(int classroomId)
        {
            return Ok(await _scheduleService.GetSchedulesByClassroomIdAsync(classroomId));
        }

        [HttpGet("ClassSubject/{classSubjectId:int}")]
        [Authorize(Policy = "Schedules.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ScheduleResponse>>> GetSchedulesByClassSubjectId(int classSubjectId)
        {
            return Ok(await _scheduleService.GetSchedulesByClassSubjectIdAsync(classSubjectId));
        }

        [HttpPost]
        [Authorize(Policy = "Schedules.Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<int>> AddSchedule(CreateScheduleRequest schedule)
        {
            int scheduleId = await _scheduleService.AddScheduleAsync(schedule);

            return CreatedAtAction(
                nameof(GetScheduleById),
                new { id = scheduleId },
                scheduleId);
        }

        [HttpPut("{scheduleId:int}")]
        [Authorize(Policy = "Schedules.Update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateSchedule(int scheduleId, UpdateScheduleRequest schedule)
        {
            await _scheduleService.UpdateScheduleAsync(scheduleId, schedule);
            return Ok();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "Schedules.Delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            await _scheduleService.DeleteScheduleAsync(id);

            return NoContent();
        }
    }
}