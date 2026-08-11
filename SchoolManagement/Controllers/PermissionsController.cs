using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.BLL.Interfaces;
using School.DTO.PermissionDTOs.Requests;
using School.DTO.PermissionDTOs.Responses;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionsController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpGet]
        [Authorize(Policy = "Permissions.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<PermissionResponse>>> GetAllPermissions()
        {
            return Ok(await _permissionService.GetAllPermissionsAsync());
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "Permissions.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PermissionResponse>> GetPermissionById(int id)
        {
            PermissionResponse? permission = await _permissionService.GetPermissionByIdAsync(id);

            if (permission == null)
                return NotFound();

            return Ok(permission);
        }

        [HttpGet("Name/{permissionName}")]
        [Authorize(Policy = "Permissions.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PermissionResponse>> GetPermissionByName(string permissionName)
        {
            PermissionResponse? permission = await _permissionService.GetPermissionByNameAsync(permissionName);

            if (permission == null)
                return NotFound();

            return Ok(permission);
        }

        [HttpPost]
        [Authorize(Policy = "Permissions.Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<int>> AddPermission(CreatePermissionRequest permission)
        {
            int permissionId = await _permissionService.AddPermissionAsync(permission);

            return CreatedAtAction(
                nameof(GetPermissionById),
                new { id = permissionId },
                permissionId);
        }

        [HttpPut("{permissionId:int}")]
        [Authorize(Policy = "Permissions.Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdatePermission(int permissionId, UpdatePermissionRequest permission)
        {
            await _permissionService.UpdatePermissionAsync(permissionId, permission);

            return Ok();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "Permissions.Delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeletePermission(int id)
        {
            await _permissionService.DeletePermissionAsync(id);

            return NoContent();
        }
    }
}