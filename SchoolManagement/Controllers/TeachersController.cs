using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.API.Authorization.OwnedResources;
using School.API.Authorization.Requirements;
using School.BLL.Authentication;
using School.BLL.Interfaces;
using School.DTO.TeachersDTOs;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachersController : ControllerBase
    {
        private readonly ITeacherService _teacherService;
        private readonly IAuthorizationService _authorizationService;

        public TeachersController(ITeacherService teacherService, IAuthorizationService authorizationService)
        {
            _teacherService = teacherService;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        [Authorize(Policy = "Teachers.View.All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<TeacherDetailsDTO>>> GetAllTeachers()
        {
            return Ok(await _teacherService.GetAllTeachersAsync());
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "Teachers.View.Own")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<TeacherDetailsDTO>> GetTeacherById(int id)
        {
            TeacherDetailsDTO? teacher = await _teacherService.GetTeacherByIdAsync(id);

            if (teacher == null)
                return NotFound();

            if (!User.HasClaim(CustomClaimTypes.Permission, "Teachers.View.All"))
            {
                var authResult = await _authorizationService.AuthorizeAsync(
                    User, new PersonOwnedResource(teacher.PersonID), new OwnershipRequirement());

                if (!authResult.Succeeded)
                    return Forbid();
            }

            return Ok(teacher);
        }

        [HttpGet("Person/{personId:int}")]
        [Authorize(Policy = "Teachers.View.Own")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<TeacherDetailsDTO>> GetTeacherByPersonId(int personId)
        {
            if (!User.HasClaim(CustomClaimTypes.Permission, "Teachers.View.All"))
            {
                var authResult = await _authorizationService.AuthorizeAsync(
                    User, new PersonOwnedResource(personId), new OwnershipRequirement());

                if (!authResult.Succeeded)
                    return Forbid();
            }

            TeacherDetailsDTO? teacher = await _teacherService.GetTeacherByPersonIdAsync(personId);

            if (teacher == null)
                return NotFound();

            return Ok(teacher);
        }

        [HttpGet("NationalID/{nationalId}")]
        [Authorize(Policy = "Teachers.View.All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TeacherDetailsDTO>> GetTeacherByNationalId(string nationalId)
        {
            TeacherDetailsDTO? teacher = await _teacherService.GetTeacherByNationalIdAsync(nationalId);

            if (teacher == null)
                return NotFound();

            return Ok(teacher);
        }

        [HttpPost]
        [Authorize(Policy = "Teachers.Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<int>> AddTeacher(TeacherDTO teacherDTO)
        {
            int teacherId = await _teacherService.AddTeacherAsync(teacherDTO);

            return CreatedAtAction(
                nameof(GetTeacherById),
                new { id = teacherId },
                teacherId);
        }

        [HttpPut]
        [Authorize(Policy = "Teachers.Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateTeacher(TeacherDTO teacherDTO)
        {
            await _teacherService.UpdateTeacherAsync(teacherDTO);

            return Ok();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "Teachers.Delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            await _teacherService.DeleteTeacherAsync(id);

            return NoContent();
        }
    }
}