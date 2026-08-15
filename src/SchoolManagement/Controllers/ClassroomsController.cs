using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.BLL.Interfaces;
using School.DTO.ClassroomDTOs.Requests;
using School.DTO.ClassroomDTOs.Responses;

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
        [Authorize(Policy = "Classrooms.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ClassroomResponse>>> GetAllClassrooms()
        {
            return Ok(await _classroomService.GetAllClassroomsAsync());
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "Classrooms.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClassroomResponse>> GetClassroomById(int id)
        {
            ClassroomResponse classroom =
                await _classroomService.GetClassroomByIdAsync(id);

            return Ok(classroom);
        }

        [HttpGet("Search")]
        [Authorize(Policy = "Classrooms.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClassroomResponse>> GetClassroomByRoomName([FromQuery] string roomName)
        {
            ClassroomResponse classroom =
                await _classroomService.GetClassroomByRoomNameAsync(roomName);

            return Ok(classroom);
        }

        [HttpPost]
        [Authorize(Policy = "Classrooms.Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<int>> AddClassroom(CreateClassroomRequest classroom)
        {
            int classroomId =
                await _classroomService.AddClassroomAsync(classroom);

            return CreatedAtAction(
                nameof(GetClassroomById),
                new { id = classroomId },
                classroomId);
        }

        [HttpPut("{classroomId:int}")]
        [Authorize(Policy = "Classrooms.Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateClassroom(int classroomId, UpdateClassroomRequest classroom)
        {
            await _classroomService.UpdateClassroomAsync(classroomId, classroom);

            return Ok();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "Classrooms.Delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteClassroom(int id)
        {
            await _classroomService.DeleteClassroomAsync(id);

            return NoContent();
        }
    }
}