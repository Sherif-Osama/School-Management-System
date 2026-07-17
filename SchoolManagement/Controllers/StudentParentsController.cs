using Microsoft.AspNetCore.Mvc;
using School.BLL.Interfaces;
using School.DTO.AssociationsDTOs.StudentParentDTOs;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentParentsController : ControllerBase
    {
        private readonly IStudentParentService _studentParentService;

        public StudentParentsController(IStudentParentService studentParentService)
        {
            _studentParentService = studentParentService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<StudentParentDetailsDTO>>> GetAll()
        {
            return Ok(await _studentParentService.GetAllStudentParentsAsync());
        }

        [HttpGet("Student/{studentId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<StudentParentDetailsDTO>>> GetParentsByStudentId(int studentId)
        {
            return Ok(await _studentParentService.GetParentsByStudentIdAsync(studentId));
        }

        [HttpGet("Parent/{parentId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<StudentParentDetailsDTO>>> GetStudentsByParentId(int parentId)
        {
            return Ok(await _studentParentService.GetStudentsByParentIdAsync(parentId));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Add(StudentParentDTO relation)
        {
            await _studentParentService.AddStudentParentAsync(relation);

            return Ok();
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(StudentParentDTO relation)
        {
            await _studentParentService.DeleteStudentParentAsync(relation);

            return NoContent();
        }
    }
}