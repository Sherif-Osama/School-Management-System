using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.BLL.Interfaces;
using School.DTO.ClassroomDTOs;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassroomsController : ControllerBase
    {
        private readonly IClassroomService _classroomService;

        public ClassroomsController(IClassroomService classroomService)
        {
            _classroomService = classroomService;
        }

        [HttpGet]
        [Authorize(Policy = "Classes.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ClassroomDTO>>> GetAllClassrooms()
        {
            return Ok(await _classroomService.GetAllClassroomsAsync());
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "Classes.View")]
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
        [Authorize(Policy = "Classes.View")]
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
        [Authorize(Policy = "Classes.Create")]
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
        [Authorize(Policy = "Classes.Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateClassroom(ClassroomDTO classroom)
        {
            await _classroomService.UpdateClassroomAsync(classroom);

            return Ok();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "Classes.Delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteClassroom(int id)
        {
            await _classroomService.DeleteClassroomAsync(id);

            return NoContent();
        }
    }
}