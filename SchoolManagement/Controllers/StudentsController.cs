using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.API.Authorization.OwnedResources;
using School.API.Authorization.Requirements;
using School.BLL.Authentication;
using School.BLL.Interfaces;
using School.DTO.StudentsDTOs;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly IAuthorizationService _authorizationService;

        public StudentsController(IStudentService studentService, IAuthorizationService authorizationService)
        {
            _studentService = studentService;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        [Authorize(Policy = "Students.View.All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<StudentDetailsDTO>>> GetAllStudents()
        {
            return Ok(await _studentService.GetAllStudentsAsync());
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "Students.View.Own")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<StudentDetailsDTO>> GetStudentById(int id)
        {
            StudentDetailsDTO? student = await _studentService.GetStudentByIdAsync(id);

            if (student == null)
                return NotFound();

            if (!User.HasClaim(CustomClaimTypes.Permission, "Students.View.All"))
            {
                var authResult = await _authorizationService.AuthorizeAsync(
                    User, new StudentOwnedResource(student.StudentID), new OwnershipRequirement());

                if (!authResult.Succeeded)
                    return Forbid();
            }

            return Ok(student);
        }

        [HttpGet("Person/{personId:int}")]
        [Authorize(Policy = "Students.View.Own")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<StudentDetailsDTO>> GetStudentByPersonId(int personId)
        {
            StudentDetailsDTO? student = await _studentService.GetStudentByPersonIdAsync(personId);

            if (student == null)
                return NotFound();

            if (!User.HasClaim(CustomClaimTypes.Permission, "Students.View.All"))
            {
                var authResult = await _authorizationService.AuthorizeAsync(
                    User, new StudentOwnedResource(student.StudentID), new OwnershipRequirement());

                if (!authResult.Succeeded)
                    return Forbid();
            }

            return Ok(student);
        }

        [HttpPost]
        [Authorize(Policy = "Students.Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<int>> AddStudent(StudentDTO studentDTO)
        {
            int studentId = await _studentService.AddStudentAsync(studentDTO);

            return CreatedAtAction(
                nameof(GetStudentById),
                new { id = studentId },
                studentId);
        }

        [HttpPut]
        [Authorize(Policy = "Students.Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateStudent(StudentDTO studentDTO)
        {
            await _studentService.UpdateStudentAsync(studentDTO);

            return Ok();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "Students.Delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            await _studentService.DeleteStudentAsync(id);

            return NoContent();
        }
    }
}