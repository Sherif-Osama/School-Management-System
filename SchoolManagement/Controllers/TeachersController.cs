using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.API.Authorization.OwnedResources;
using School.API.Authorization.Requirements;
using School.BLL.Authentication;
using School.BLL.Interfaces;
using School.DTO.TeachersDTOs.Requests;
using School.DTO.TeachersDTOs.Responses;

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
        public async Task<ActionResult<List<TeacherResponse>>> GetAllTeachers()
        {
            return Ok(await _teacherService.GetAllTeachersAsync());
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "Teachers.View.Own")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<TeacherResponse>> GetTeacherById(int id)
        {
            TeacherResponse teacher = await _teacherService.GetTeacherByIdAsync(id);

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
        public async Task<ActionResult<TeacherResponse>> GetTeacherByPersonId(int personId)
        {
            if (!User.HasClaim(CustomClaimTypes.Permission, "Teachers.View.All"))
            {
                var authResult = await _authorizationService.AuthorizeAsync(
                    User, new PersonOwnedResource(personId), new OwnershipRequirement());

                if (!authResult.Succeeded)
                    return Forbid();
            }

            TeacherResponse teacher = await _teacherService.GetTeacherByPersonIdAsync(personId);

            return Ok(teacher);
        }

        [HttpGet("NationalID/{nationalId}")]
        [Authorize(Policy = "Teachers.View.All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TeacherResponse>> GetTeacherByNationalId(string nationalId)
        {
            TeacherResponse teacher = await _teacherService.GetTeacherByNationalIdAsync(nationalId);

            return Ok(teacher);
        }

        [HttpPost]
        [Authorize(Policy = "Teachers.Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<int>> AddTeacher(CreateTeacherRequest teacher)
        {
            int teacherId = await _teacherService.AddTeacherAsync(teacher);

            return CreatedAtAction(
                nameof(GetTeacherById),
                new { id = teacherId },
                teacherId);
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "Teachers.Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateTeacher(int id, UpdateTeacherRequest teacher)
        {
            await _teacherService.UpdateTeacherAsync(id, teacher);

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