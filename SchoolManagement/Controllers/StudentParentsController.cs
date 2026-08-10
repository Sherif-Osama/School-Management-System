using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.API.Authorization.OwnedResources;
using School.API.Authorization.Requirements;
using School.BLL.Authentication;
using School.BLL.Interfaces;
using School.DTO.AssociationsDTOs.StudentParentDTOs;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentParentsController : ControllerBase
    {
        private readonly IStudentParentService _studentParentService;
        private readonly IAuthorizationService _authorizationService;

        public StudentParentsController(IStudentParentService studentParentService, IAuthorizationService authorizationService)
        {
            _studentParentService = studentParentService;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        [Authorize(Policy = "Parents.View.All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<StudentParentDetailsDTO>>> GetAll()
        {
            return Ok(await _studentParentService.GetAllStudentParentsAsync());
        }

        [HttpGet("Student/{studentId:int}")]
        [Authorize(Policy = "Parents.View.Own")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<List<StudentParentDetailsDTO>>> GetParentsByStudentId(int studentId)
        {
            if (!User.HasClaim(CustomClaimTypes.Permission, "Parents.View.All"))
            {
                var authResult = await _authorizationService.AuthorizeAsync(
                    User, new StudentOwnedResource(studentId), new OwnershipRequirement());

                if (!authResult.Succeeded)
                    return Forbid();
            }

            return Ok(await _studentParentService.GetParentsByStudentIdAsync(studentId));
        }

        [HttpGet("Parent/{parentId:int}")]
        [Authorize(Policy = "Parents.View.Own")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<List<StudentParentDetailsDTO>>> GetStudentsByParentId(int parentId)
        {
            if (!User.HasClaim(CustomClaimTypes.Permission, "Parents.View.All"))
            {
                var authResult = await _authorizationService.AuthorizeAsync(
                    User, new ParentOwnedResource(parentId), new OwnershipRequirement());

                if (!authResult.Succeeded)
                    return Forbid();
            }

            return Ok(await _studentParentService.GetStudentsByParentIdAsync(parentId));
        }

        [HttpPost]
        [Authorize(Policy = "Parents.Create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Add(StudentParentDTO relation)
        {
            await _studentParentService.AddStudentParentAsync(relation);

            return Ok();
        }

        [HttpDelete]
        [Authorize(Policy = "Parents.Delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(StudentParentDTO relation)
        {
            await _studentParentService.DeleteStudentParentAsync(relation);

            return NoContent();
        }
    }
}