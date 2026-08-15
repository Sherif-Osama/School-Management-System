using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.API.Authorization.OwnedResources;
using School.API.Authorization.Requirements;
using School.BLL.Authentication;
using School.BLL.Interfaces;
using School.DTO.ParentsDTOs.Requests;
using School.DTO.ParentsDTOs.Responses;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParentsController : ControllerBase
    {
        private readonly IParentService _parentService;
        private readonly IAuthorizationService _authorizationService;

        public ParentsController(IParentService parentService, IAuthorizationService authorizationService)
        {
            _parentService = parentService;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        [Authorize(Policy = "Parents.View.All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ParentResponse>>> GetAllParents()
        {
            return Ok(await _parentService.GetAllParentsAsync());
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "Parents.View.Own")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ParentResponse>> GetParentById(int id)
        {
            ParentResponse parent = await _parentService.GetParentByIdAsync(id);

            if (!User.HasClaim(CustomClaimTypes.Permission, "Parents.View.All"))
            {
                var authResult = await _authorizationService.AuthorizeAsync(
                    User, new ParentOwnedResource(parent.ParentID), new OwnershipRequirement());

                if (!authResult.Succeeded)
                    return Forbid();
            }

            return Ok(parent);
        }

        [HttpGet("Person/{personId:int}")]
        [Authorize(Policy = "Parents.View.Own")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ParentResponse>> GetParentByPersonId(int personId)
        {
            ParentResponse parent = await _parentService.GetParentByPersonIdAsync(personId);

            if (!User.HasClaim(CustomClaimTypes.Permission, "Parents.View.All"))
            {
                var authResult = await _authorizationService.AuthorizeAsync(
                    User, new ParentOwnedResource(parent.ParentID), new OwnershipRequirement());

                if (!authResult.Succeeded)
                    return Forbid();
            }

            return Ok(parent);
        }

        [HttpGet("NationalID/{nationalId}")]
        [Authorize(Policy = "Parents.View.All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ParentResponse>> GetParentByNationalId(string nationalId)
        {
            ParentResponse parent = await _parentService.GetParentByNationalIdAsync(nationalId);

            return Ok(parent);
        }

        [HttpPost]
        [Authorize(Policy = "Parents.Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<int>> AddParent(CreateParentRequest parent)
        {
            int parentId = await _parentService.AddParentAsync(parent);

            return CreatedAtAction(
                nameof(GetParentById),
                new { id = parentId },
                parentId);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "Parents.Delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteParent(int id)
        {
            await _parentService.DeleteParentAsync(id);

            return NoContent();
        }
    }
}