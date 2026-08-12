using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.API.Authorization.OwnedResources;
using School.API.Authorization.Requirements;
using School.BLL.Authentication;
using School.BLL.Interfaces;
using School.DTO.StudentsDTOs.Requests;
using School.DTO.StudentsDTOs.Responses;
using System.Security.Claims;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger<StudentsController> _logger;

        public StudentsController(IStudentService studentService, IAuthorizationService authorizationService, ILogger<StudentsController> logger)
        {
            _studentService = studentService;
            _authorizationService = authorizationService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Policy = "Students.View.All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<StudentResponse>>> GetAllStudents()
        {
            return Ok(await _studentService.GetAllStudentsAsync());
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "Students.View.Own")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<StudentResponse>> GetStudentById(int id)
        {
            StudentResponse? student = await _studentService.GetStudentByIdAsync(id);

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
        public async Task<ActionResult<StudentResponse>> GetStudentByPersonId(int personId)
        {
            StudentResponse? student = await _studentService.GetStudentByPersonIdAsync(personId);

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
        public async Task<ActionResult<int>> AddStudent(CreateStudentRequest student)
        {
            int studentId = await _studentService.AddStudentAsync(student);

            return CreatedAtAction(
                nameof(GetStudentById),
                new { id = studentId },
                studentId);
        }

        [HttpPut("{studentId:int}")]
        [Authorize(Policy = "Students.Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateStudent(int studentId, UpdateStudentRequest student)
        {
            await _studentService.UpdateStudentAsync(studentId, student);

            return Ok();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "Students.Delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            await _studentService.DeleteStudentAsync(id);
            _logger.LogWarning("Student {StudentId} was deleted by {Username}.", id, User.FindFirstValue(ClaimTypes.Name));
            return NoContent();
        }
    }
}