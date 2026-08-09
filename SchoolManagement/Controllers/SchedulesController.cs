using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.BLL.Interfaces;
using School.DTO.ScheduleDTOs;

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
        public async Task<ActionResult<List<ScheduleDetailsDTO>>> GetAllSchedules()
        {
            return Ok(await _scheduleService.GetAllSchedulesAsync());
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "Schedules.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ScheduleDetailsDTO>> GetScheduleById(int id)
        {
            ScheduleDetailsDTO? schedule =
                await _scheduleService.GetScheduleByIdAsync(id);

            if (schedule is null)
                return NotFound();

            return Ok(schedule);
        }

        [HttpGet("Class/{classId:int}")]
        [Authorize(Policy = "Schedules.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ScheduleDetailsDTO>>> GetSchedulesByClassId(int classId)
        {
            return Ok(await _scheduleService.GetSchedulesByClassIdAsync(classId));
        }

        [HttpGet("Teacher/{teacherId:int}")]
        [Authorize(Policy = "Schedules.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ScheduleDetailsDTO>>> GetSchedulesByTeacherId(int teacherId)
        {
            return Ok(await _scheduleService.GetSchedulesByTeacherIdAsync(teacherId));
        }

        [HttpGet("Classroom/{classroomId:int}")]
        [Authorize(Policy = "Schedules.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ScheduleDetailsDTO>>> GetSchedulesByClassroomId(int classroomId)
        {
            return Ok(await _scheduleService.GetSchedulesByClassroomIdAsync(classroomId));
        }

        [HttpGet("ClassSubject/{classSubjectId:int}")]
        [Authorize(Policy = "Schedules.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ScheduleDetailsDTO>>> GetSchedulesByClassSubjectId(int classSubjectId)
        {
            return Ok(await _scheduleService.GetSchedulesByClassSubjectIdAsync(classSubjectId));
        }

        [HttpPost]
        [Authorize(Policy = "Schedules.Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<int>> AddSchedule(ScheduleDTO scheduleDTO)
        {
            int scheduleId = await _scheduleService.AddScheduleAsync(scheduleDTO);

            return CreatedAtAction(
                nameof(GetScheduleById),
                new { id = scheduleId },
                scheduleId);
        }

        [HttpPut]
        [Authorize(Policy = "Schedules.Update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateSchedule(ScheduleDTO scheduleDTO)
        {
            await _scheduleService.UpdateScheduleAsync(scheduleDTO);
            return NoContent();
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