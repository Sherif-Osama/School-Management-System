using Microsoft.AspNetCore.Mvc;
using School.BLL;
using School.DTO.ClassroomDTOs;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassroomsController : ControllerBase
    {
        private readonly ClassroomService _classroomService;

        public ClassroomsController(ClassroomService classroomService)
        {
            _classroomService = classroomService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ClassroomDTO>>> GetAllClassrooms()
        {
            return Ok(await _classroomService.GetAllClassroomsAsync());
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClassroomDTO>> GetClassroomById(int id)
        {
            ClassroomDTO? classroom =
                await _classroomService.GetClassroomByIdAsync(id);

            if (classroom == null)
                return NotFound();

            return Ok(classroom);
        }

        [HttpGet("Search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClassroomDTO>> GetClassroomByRoomName(
            [FromQuery] string roomName)
        {
            ClassroomDTO? classroom =
                await _classroomService.GetClassroomByRoomNameAsync(roomName);

            if (classroom == null)
                return NotFound();

            return Ok(classroom);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<int>> AddClassroom(ClassroomDTO classroom)
        {
            int classroomId =
                await _classroomService.AddClassroomAsync(classroom);

            return CreatedAtAction(
                nameof(GetClassroomById),
                new { id = classroomId },
                classroomId);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateClassroom(ClassroomDTO classroom)
        {
            await _classroomService.UpdateClassroomAsync(classroom);

            return Ok();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteClassroom(int id)
        {
            await _classroomService.DeleteClassroomAsync(id);

            return NoContent();
        }
    }
}