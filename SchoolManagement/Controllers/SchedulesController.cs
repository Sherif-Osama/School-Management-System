using Microsoft.AspNetCore.Mvc;
using School.BLL;
using School.DTO.ScheduleDTOs;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchedulesController : ControllerBase
    {
        private readonly ScheduleService _scheduleService;

        public SchedulesController(ScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ScheduleDetailsDTO>>> GetAllSchedules()
        {
            return Ok(await _scheduleService.GetAllSchedulesAsync());
        }

        [HttpGet("{id:int}")]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ScheduleDetailsDTO>>> GetSchedulesByClassId(int classId)
        {
            return Ok(await _scheduleService.GetSchedulesByClassIdAsync(classId));
        }

        [HttpGet("Teacher/{teacherId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ScheduleDetailsDTO>>> GetSchedulesByTeacherId(int teacherId)
        {
            return Ok(await _scheduleService.GetSchedulesByTeacherIdAsync(teacherId));
        }

        [HttpGet("Classroom/{classroomId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ScheduleDetailsDTO>>> GetSchedulesByClassroomId(int classroomId)
        {
            return Ok(await _scheduleService.GetSchedulesByClassroomIdAsync(classroomId));
        }

        [HttpGet("ClassSubject/{classSubjectId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ScheduleDetailsDTO>>> GetSchedulesByClassSubjectId(int classSubjectId)
        {
            return Ok(await _scheduleService.GetSchedulesByClassSubjectIdAsync(classSubjectId));
        }

        [HttpPost]
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateSchedule(ScheduleDTO scheduleDTO)
        {
            await _scheduleService.UpdateScheduleAsync(scheduleDTO);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
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